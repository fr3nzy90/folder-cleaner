using FolderCleaner.DTOs;
using FolderCleaner.Extensions;
using FolderCleaner.Services;
using FolderCleaner.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.Setup(LoggingType.Verbose));
ILogger logger = loggerFactory.CreateLogger<Program>();

try
{
  CommandLineArguments arguments = CommandLineArgumentsFactory.Create(args);

  if (CommandLineCommand.NoAction == arguments.Command)
  {
    return;
  }

  loggerFactory.Dispose();
  loggerFactory = LoggerFactory.Create(builder => builder.Setup(arguments.Logger));
  logger = loggerFactory.CreateLogger<Program>();

  ServiceProvider serviceProvider = new ServiceCollection()
    .Setup(arguments.ConfigurationFile!, arguments.Logger)
    .BuildServiceProvider();

  CleanerService cleanerService = serviceProvider.GetRequiredService<CleanerService>();

  switch (arguments.Command)
  {
    default:
    case CommandLineCommand.NoAction:
      break;
    case CommandLineCommand.Clean:
      cleanerService.Clean(arguments.Root!, arguments.Profiles!, arguments.Simulate);
      break;
    case CommandLineCommand.ListProfiles:
      Console.WriteLine("Configured profiles:");
      cleanerService.GetProfiles()
        .ToList()
        .ForEach(key => Console.WriteLine($"  - {key}"));
      break;
  }
}
catch (Exception e)
{
  logger.LogCritical("Error while executing program. Error: {0}", e.Message);
}
finally
{
  loggerFactory.Dispose();
}