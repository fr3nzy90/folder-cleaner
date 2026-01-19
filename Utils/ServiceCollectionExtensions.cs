using FolderCleaner.DTOs;
using FolderCleaner.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FolderCleaner.Utils;

internal static class ServiceCollectionExtensions
{
  public static IServiceCollection Setup(this IServiceCollection services, string configurationFile, LoggingType loggingType) =>
    services
      .AddOptions()
      .AddLogging(builder => builder.SetupCustom(loggingType))
      .Configure<CleanerConfiguration>(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(configurationFile, optional: false)
        .Build())
      .AddSingleton<CleanerService>();
}