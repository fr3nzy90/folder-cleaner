using Microsoft.Extensions.Logging;

namespace FolderCleaner.Utils;

internal static class LoggingBuilderExtensions
{
  public static ILoggingBuilder Setup(this ILoggingBuilder builder) =>
    builder.AddSimpleConsole(options =>
    {
      options.SingleLine = true;
      options.IncludeScopes = true;
      options.UseUtcTimestamp = true;
      options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
    });
}