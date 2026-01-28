using FolderCleaner.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FolderCleaner.Services;

internal class CleanerService
{
  private readonly ILogger _logger;
  private readonly IOptionsMonitor<CleanerConfiguration> _configurationMonitor;

  public CleanerService(ILogger<CleanerService> logger, IOptionsMonitor<CleanerConfiguration> configurationMonitor)
  {
    _logger = logger;
    _configurationMonitor = configurationMonitor;
  }

  public IList<string> GetProfiles() =>
    (_configurationMonitor.CurrentValue?.Profiles ?? new Dictionary<string, CleanupProfile>())
      .OrderByDescending(kvp => kvp.Value.Priority)
      .Select(kvp => kvp.Key)
      .ToList();

  public void Clean(string root, IList<string> profileNames, bool simulate)
  {
    if (!File.GetAttributes(root).HasFlag(FileAttributes.Directory))
    {
      throw new ArgumentException("Invalid parameter", nameof(root));
    }

    _logger.LogDebug("Folder cleanup initiated for {0} with simulate={1}", root, simulate);

    IList<(string name, string? parent, CleanupProfile profile)> profiles = GetProfiles(profileNames, null,
      _configurationMonitor.CurrentValue?.Profiles ?? new Dictionary<string, CleanupProfile>())
      .OrderByDescending(o => o.profile.Priority)
      .ToList();

    if (0 == profiles.Count)
    {
      _logger.LogDebug("Nothing to do, no applicable cleanup profiles");
      return;
    }

    foreach ((string name, string? parent, CleanupProfile profile) in profiles)
    {
      if (parent is null)
      {
        _logger.LogDebug("Cleanup profile '{0}' started", name);
      }
      else
      {
        _logger.LogDebug("Linked cleanup profile '{0}' from '{1}' started", name, parent);
      }

      DeleteFiles(root, profile, simulate);
      DeleteDirectories(root, profile, simulate);

      if (parent is null)
      {
        _logger.LogDebug("Cleanup profile '{0}' completed", name);
      }
      else
      {
        _logger.LogDebug("Linked cleanup profile '{0}' from '{1}' completed", name, parent);
      }
    }
  }

  private IList<(string name, string? parent, CleanupProfile profile)> GetProfiles(IList<string>? profileNames, string? parent,
    IDictionary<string, CleanupProfile> configuredProfiles)
  {
    if (!(profileNames?.Count > 0))
    {
      return [];
    }

    List<(string name, string? parent, CleanupProfile profile)> result = new();

    foreach (string name in profileNames.Distinct())
    {
      if (!configuredProfiles.TryGetValue(name, out CleanupProfile? profile))
      {
        _logger.LogWarning("Skipping unconfigured cleanup profile '{0}'", name);
        continue;
      }

      result.Add((name, parent, profile));
      result.AddRange(GetProfiles(profile.LinkedProfiles, name, configuredProfiles)
        .Where(t1 => result.Find(t2 => t2.name == t1.name).name is null));
    }

    return result;
  }

  private void DeleteFiles(string root, CleanupProfile profile, bool simulate)
  {
    SearchOption option = profile.RecursiveSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
    Action<string> action = simulate ? path => { } : File.Delete;
    bool deleted = false;

    profile.FileSearchPatterns?
      .Select(pattern => Directory.EnumerateFiles(root, pattern, option))
      .Aggregate((l1, l2) => l1.Concat(l2))
      .Distinct()
      .Order()
      .ToList()
      .ForEach(file =>
      {
        try
        {
          if (!File.Exists(file))
          {
            return;
          }

          action.Invoke(file);
          deleted = true;
          _logger.LogInformation("Deleted file: {0}", file);
        }
        catch (Exception e)
        {
          _logger.LogError("Error while deleting file: {0}, Error: {1}", file, e.Message);
        }
      });

    if (!deleted)
    {
      _logger.LogDebug("No file to delete");
    }
  }

  private void DeleteDirectories(string root, CleanupProfile profile, bool simulate)
  {
    SearchOption option = profile.RecursiveSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
    Action<string> action = simulate ? path => { } : path => Directory.Delete(path, profile.RecursiveDelete);
    bool deleted = false;

    profile.DirectorySearchPatterns?
      .Select(pattern => Directory.EnumerateDirectories(root, pattern, option))
      .Aggregate((l1, l2) => l1.Concat(l2))
      .Distinct()
      .Order()
      .ToList()
      .ForEach(directory =>
      {
        try
        {
          if (!Directory.Exists(directory))
          {
            return;
          }

          action.Invoke(directory);
          deleted = true;
          _logger.LogInformation("Deleted directory: {0}", directory);
        }
        catch (Exception e)
        {
          _logger.LogError("Error while deleting directory: {0}, Error: {1}", directory, e.Message);
        }
      });

    if (!deleted)
    {
      _logger.LogDebug("No directory to delete");
    }
  }
}