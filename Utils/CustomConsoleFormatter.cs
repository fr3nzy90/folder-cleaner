using FolderCleaner.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace FolderCleaner.Utils;

internal class CustomConsoleFormatter : ConsoleFormatter
{
  private CustomConsoleFormatterOptions _options;

  public CustomConsoleFormatter(IOptionsSnapshot<CustomConsoleFormatterOptions> options)
    : base(nameof(CustomConsoleFormatter))
  {
    _options = options.Value;
  }

  public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
  {
    string? message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);

    if (message is null)
    {
      return;
    }

    switch (_options.Type)
    {
      default:
      case LoggingType.Silent:
      case LoggingType.Normal:
        break;
      case LoggingType.Verbose:
        message = $"{GetTimestamp()} [{logEntry.LogLevel}] {logEntry.Category}: {message}";
        break;
    }

    textWriter.WriteLine(message);
  }

  private string GetTimestamp() =>
    (_options.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now).ToString(_options.TimestampFormat);
}