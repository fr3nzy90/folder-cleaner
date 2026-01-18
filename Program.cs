using FolderCleaner.DTOs;
using FolderCleaner.Services;
using FolderCleaner.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

ILogger logger = CustomLoggerFactory.CreateLogger<Program>();

try
{
  CommandLineArguments arguments = CommandLineArgumentsFactory.Create(args);

  if (arguments.Command == CommandLineCommand.NoAction)
  {
    return;
  }

  ServiceProvider serviceProvider = new ServiceCollection()
    .Setup(arguments.ConfigurationFile!)
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
      cleanerService.ListProfiles();
      break;
  }
}
catch (Exception e)
{
  logger.LogCritical("Error while executing program. Error: {0}", e.Message);
}