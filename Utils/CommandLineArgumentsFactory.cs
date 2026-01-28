using FolderCleaner.DTOs;
using System.CommandLine;
using System.Reflection;

namespace FolderCleaner.Utils;

internal static class CommandLineArgumentsFactory
{
  extension(IList<Option> instance)
  {
    public Option<string> AddRootOption()
    {
      Option<string> option = new("--root", ["-r"])
      {
        Description = "The file to read and display on the console",
        AllowMultipleArgumentsPerToken = false,
        Arity = ArgumentArity.ZeroOrOne,
        Required = false,
        DefaultValueFactory = _ => Directory.GetCurrentDirectory()
      };
      option.AcceptLegalFilePathsOnly();
      instance.Add(option);
      return option;
    }

    public Option<string> AddConfigOption()
    {
      Option<string> option = new("--configuration", ["-c"])
      {
        Description = "Name of configuration file placed in same folder as executable",
        AllowMultipleArgumentsPerToken = false,
        Arity = ArgumentArity.ZeroOrOne,
        Required = false,
        DefaultValueFactory = _ => "config.json"
      };
      option.AcceptLegalFileNamesOnly();
      instance.Add(option);
      return option;
    }

    public Option<IList<string>> AddProfileOption()
    {
      Option<IList<string>> option = new("--profile", ["-p"])
      {
        Description = "Cleanup profile(s)",
        AllowMultipleArgumentsPerToken = true,
        Arity = ArgumentArity.ZeroOrMore,
        Required = false,
        DefaultValueFactory = _ => ["root"],
        CustomParser = r => r.Tokens.Select(t => t.Value).ToList()
      };
      instance.Add(option);
      return option;
    }

    public Option<bool> AddSimulateOption()
    {
      Option<bool> option = new("--simulate", ["-s"])
      {
        Description = "Simulate deletion of files without actual deletion",
        AllowMultipleArgumentsPerToken = false,
        Arity = ArgumentArity.ZeroOrOne,
        Required = false,
        DefaultValueFactory = _ => false
      };
      instance.Add(option);
      return option;
    }

    public Option<bool> AddListProfilesOption()
    {
      Option<bool> option = new("--listProfiles", ["-lp"])
      {
        Description = "Lists all configured profiles",
        AllowMultipleArgumentsPerToken = false,
        Arity = ArgumentArity.ZeroOrOne,
        Required = false,
        DefaultValueFactory = _ => false
      };
      instance.Add(option);
      return option;
    }

    public Option<bool> AddSilentOption()
    {
      Option<bool> option = new("--silent")
      {
        Description = "Log as little as possible",
        AllowMultipleArgumentsPerToken = false,
        Arity = ArgumentArity.ZeroOrOne,
        Required = false,
        DefaultValueFactory = _ => false
      };
      instance.Add(option);
      return option;
    }

    public Option<bool> AddVerboseOption()
    {
      Option<bool> option = new("--verbose")
      {
        Description = "Log as much as possible",
        AllowMultipleArgumentsPerToken = false,
        Arity = ArgumentArity.ZeroOrOne,
        Required = false,
        DefaultValueFactory = _ => false
      };
      instance.Add(option);
      return option;
    }
  }

  public static CommandLineArguments Create(params string[] arguments)
  {
    CommandLineArguments result = new()
    {
      Command = CommandLineCommand.NoAction,
      Logger = LoggingType.Silent
    };

    RootCommand rootCommand = new($"{AppDomain.CurrentDomain.FriendlyName} {Assembly.GetEntryAssembly()?.GetName().Version} console application");

    Option<string> rootOption = rootCommand.Options.AddRootOption();
    Option<string> configOption = rootCommand.Options.AddConfigOption();
    Option<IList<string>> profileOption = rootCommand.Options.AddProfileOption();
    Option<bool> simulateOption = rootCommand.Options.AddSimulateOption();
    Option<bool> listProfilesOption = rootCommand.Options.AddListProfilesOption();
    Option<bool> silentOption = rootCommand.Options.AddSilentOption();
    Option<bool> verboseOption = rootCommand.Options.AddVerboseOption();

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