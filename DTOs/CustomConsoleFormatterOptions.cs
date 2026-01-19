using Microsoft.Extensions.Logging.Console;

namespace FolderCleaner.DTOs;

internal class CustomConsoleFormatterOptions : ConsoleFormatterOptions
{
  public LoggingType Type { get; set; }
}