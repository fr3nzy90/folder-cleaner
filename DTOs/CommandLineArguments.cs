namespace FolderCleaner.DTOs;

internal class CommandLineArguments
{
  public string? Root { get; init; }
  public string? ConfigurationFile { get; init; }
  public IList<string>? Profiles { get; init; }
  public bool Simulate { get; init; }
  public CommandLineCommand Command { get; init; }
  public LoggingType Logger { get; init; }
}