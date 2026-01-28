using FolderCleaner.DTOs;
using FolderCleaner.Services;
using FolderCleaner.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FolderCleaner.Extensions;

internal static class SetupExtensions
{
  extension(ILoggingBuilder instance)
  {
    public ILoggingBuilder Setup(LoggingType type) =>
      instance
        .SetMinimumLevel(type switch
        {
          LoggingType.Silent => LogLevel.None,
          LoggingType.Normal => LogLevel.Information,
          LoggingType.Verbose => LogLevel.Trace,
          _ => throw new NotSupportedException($"{nameof(LoggingType)}.{type} not supported")
        })
        .SetupCustom(type);

    public ILoggingBuilder SetupCustom(LoggingType type) =>
      instance
        .AddConsole(options => options.FormatterName = nameof(CustomConsoleFormatter))
        .AddConsoleFormatter<CustomConsoleFormatter, CustomConsoleFormatterOptions>(options =>
        {
          options.Type = type;
          options.IncludeScopes = true;
          options.UseUtcTimestamp = true;
          options.TimestampFormat = "yyyy-MM-dd HH:mm:ss";
        });

    public ILoggingBuilder SetupSimple() =>
      instance.AddSimpleConsole(options =>
      {
        options.SingleLine = true;
        options.IncludeScopes = true;
        options.UseUtcTimestamp = true;
        options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
      });
  }

  extension(IServiceCollection instance)
  {
    public IServiceCollection Setup(string configurationFile, LoggingType loggingType) =>
      instance
        .AddOptions()
        .AddLogging(builder => builder.Setup(loggingType))
        .Configure<CleanerConfiguration>(new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile(configurationFile, optional: false)
          .Build())
        .AddSingleton<CleanerService>();
  }
}