namespace FolderCleaner.DTOs;

internal record CleanerConfiguration
{
  public IDictionary<string, CleanupProfile> Profiles { get; init; } = default!;
}