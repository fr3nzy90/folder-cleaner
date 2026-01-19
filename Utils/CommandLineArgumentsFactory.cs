using FolderCleaner.DTOs;
using System.CommandLine;
using System.Reflection;

namespace FolderCleaner.Utils;

internal static class CommandLineArgumentsFactory
{
  public static CommandLineArguments Create(params string[] arguments)
  {
    CommandLineArguments result = new()
    {
      Command = CommandLineCommand.NoAction,
      Logger = LoggingType.Silent
    };

    Option<string> rootOption = new("--root", ["-r"])
    {
      Description = "The file to read and display on the console",
      AllowMultipleArgumentsPerToken = false,
      Arity = ArgumentArity.ZeroOrOne,
      Required = false,
      DefaultValueFactory = _ => Directory.GetCurrentDirectory()
    };
    Option<string> configOption = new("--configuration", ["-c"])
    {
      Description = "Name of configuration file placed in same folder as executable",
      AllowMultipleArgumentsPerToken = false,
      Arity = ArgumentArity.ZeroOrOne,
      Required = false,
      DefaultValueFactory = _ => "config.json"
    };
    Option<IList<string>> profileOption = new("--profile", ["-p"])
    {
      Description = "Cleanup profile(s)",
      AllowMultipleArgumentsPerToken = true,
      Arity = ArgumentArity.ZeroOrMore,
      Required = false,
      DefaultValueFactory = _ => ["root"],
      CustomParser = r => r.Tokens.Select(t => t.Value).ToList()
    };
    Option<bool> simulateOption = new("--simulate", ["-s"])
    {
      Description = "Simulate deletion of files without actual deletion",
      AllowMultipleArgumentsPerToken = false,
      Arity = ArgumentArity.ZeroOrOne,
      Required = false,
      DefaultValueFactory = _ => false
    };
    Option<bool> listProfilesOption = new("--listProfiles", ["-lp"])
    {
      Description = "Lists all configured profiles",
      AllowMultipleArgumentsPerToken = false,
      Arity = ArgumentArity.ZeroOrOne,
      Required = false,
      DefaultValueFactory = _ => false
    };
    Option<bool> silentOption = new("--silent")
    {
      Description = "Log as little as possible",
      AllowMultipleArgumentsPerToken = false,
      Arity = ArgumentArity.ZeroOrOne,
      Required = false,
      DefaultValueFactory = _ => false
    };
    Option<bool> verboseOption = new("--verbose")
    {
      Description = "Log as much as possible",
      AllowMultipleArgumentsPerToken = false,
      Arity = ArgumentArity.ZeroOrOne,
      Required = false,
      DefaultValueFactory = _ => false
    };

    rootOption.AcceptLegalFilePathsOnly();
    configOption.AcceptLegalFileNamesOnly();

    RootCommand rootCommand = new($"{AppDomain.CurrentDomain.FriendlyName} {Assembly.GetEntryAssembly()?.GetName().Version} console application");
    rootCommand.Options.Add(rootOption);
    rootCommand.Options.Add(configOption);
    rootCommand.Options.Add(profileOption);
    rootCommand.Options.Add(simulateOption);
    rootCommand.Options.Add(listProfilesOption);
    rootCommand.Options.Add(silentOption);
    rootCommand.Options.Add(verboseOption);
    rootCommand.SetAction(r =>
      result = new()
      {
        Root = r.GetValue(rootOption),
        ConfigurationFile = r.GetValue(configOption),
        Profiles = r.GetValue(profileOption),
        Simulate = r.GetValue(simulateOption),
        Command = r.GetValue(listProfilesOption) ? CommandLineCommand.ListProfiles : CommandLineCommand.Clean,
        Logger = r.GetValue(silentOption) ? LoggingType.Silent : r.GetValue(verboseOption) ? LoggingType.Verbose : LoggingType.Normal
      });

    rootCommand
      .Parse(arguments)
      .Invoke();

    return result;
  }
}