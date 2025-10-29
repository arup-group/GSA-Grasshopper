using System;

using Commander.NET;

using DocsGeneration;

namespace DocsGeneratorCLI {
  public static class Program {
    private static int Main(string[] args) {
#if DEBUG
      args = new[] {
        "--project",
        "GsaGH",
        "--generate-e2e",
      };
#endif

      Console.WriteLine("Hello, let's generate some documentation!");
      var parser = new CommanderParser<CommandArguments>();

      if (args.Length == 0) {
        PrintUsage(parser);
        return 0;
      }

      try {
        CommandArguments commandArguments = ParseArguments(parser, args);
        Configuration config = BuildConfig(commandArguments);
        RunGeneration(config);
        return 0;
      } catch (Exception ex) {
        PrintError(ex, parser);
        return 1;
      }
    }

    private static CommandArguments ParseArguments(CommanderParser<CommandArguments> parser, string[] args) {
      Console.WriteLine("Parsing arguments...");
      return parser.Add(args).Parse();
    }

    private static Configuration BuildConfig(CommandArguments args) {
      Console.WriteLine("Building configuration...");
      return ConfigurationBuilder.BuildConfiguration(args);
    }

    private static void RunGeneration(Configuration config) {
      Console.WriteLine("Generating documentation...");
      GenerateDocumentation.Generate(config);
    }

    private static void PrintUsage(CommanderParser<CommandArguments> parser) {
      Console.WriteLine(parser.Usage());
    }

    private static void PrintError(Exception ex, CommanderParser<CommandArguments> parser) {
      Console.Error.WriteLine($"Error: {ex.Message}\n\nStackTrace:\n{ex.StackTrace}\n");
      PrintUsage(parser);
    }
  }
}
