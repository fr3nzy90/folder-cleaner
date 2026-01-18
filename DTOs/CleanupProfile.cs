namespace FolderCleaner.DTOs;

internal record CleanupProfile
{
  public IList<string>? LinkedProfiles { get; init; }
  public IList<string>? DirectorySearchPatterns { get; init; }
  public IList<string>? FileSearchPatterns { get; init; }
  public int Priority { get; init; }
  public bool RecursiveSearch { get; init; }
  public bool RecursiveDelete { get; init; }
}