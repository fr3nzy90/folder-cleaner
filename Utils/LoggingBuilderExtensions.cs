using FolderCleaner.DTOs;
using Microsoft.Extensions.Logging;

namespace FolderCleaner.Utils;

internal static class LoggingBuilderExtensions
{
  public static ILoggingBuilder SetupCustom(this ILoggingBuilder builder, LoggingType type) =>
    builder
      .SetMinimumLevel(type switch
      {
        LoggingType.Silent => LogLevel.None,
        LoggingType.Normal => LogLevel.Information,
        LoggingType.Verbose => LogLevel.Trace,
        _ => throw new NotSupportedException($"{nameof(LoggingType)}.{type} not supported")
      })
      .AddConsole(options => options.FormatterName = nameof(CustomConsoleFormatter))
      .AddConsoleFormatter<CustomConsoleFormatter, CustomConsoleFormatterOptions>(options =>
      {
        options.Type = type;
        options.IncludeScopes = true;
        options.UseUtcTimestamp = true;
        options.TimestampFormat = "yyyy-MM-dd HH:mm:ss";
      });

  public static ILoggingBuilder SetupSimple(this ILoggingBuilder builder) =>
    builder.AddSimpleConsole(options =>
    {
      options.SingleLine = true;
      options.IncludeScopes = true;
      options.UseUtcTimestamp = true;
      options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
    });
}