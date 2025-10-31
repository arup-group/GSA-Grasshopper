using System;

using CommandLine;

using DocsGeneration;

namespace DocsGeneratorCLI {
  public static class Program {
    private static int Main(string[] args) {
      Console.WriteLine("Documentation Generator, based on code!");
      bool success = true;
      Parser.Default.ParseArguments<Options>(args).WithParsed(o => {
        var config = ConfigurationBuilder.BuildConfiguration(o);
        try {
          GenerateDocumentation.Generate(config);
          success = true;
        } catch (Exception e) {
          Console.Error.WriteLine($"Error during documentation generation: {e.Message}\n{e.StackTrace}");
          success = false;
        }
      });

      return success ? 0 : 1;
    }
  }
}
