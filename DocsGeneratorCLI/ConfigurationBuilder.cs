using System.IO;
using System.Reflection;

using DocsGeneration;

namespace DocsGeneratorCLI {
  public static class ConfigurationBuilder {
    public static Configuration BuildConfiguration(CommandArguments args) {
      var projectTarget = ProjectTarget.FromString(args.ProjectName);
      Assembly assembly = projectTarget.LoadAssembly();

      string outputPath = ResolveOutputPath(args);

      return new Configuration {
        Assembly = assembly,
        GenerateE2ETestData = args.GenerateE2ETestData,
        CustomOutputPath = outputPath,
      };
    }

    private static string ResolveOutputPath(CommandArguments args) {
      return args.GenerateE2ETestData ?
        Path.Combine("DocsGeneration.E2ETests", "TestReferences", args.ProjectName) :
        !string.IsNullOrWhiteSpace(args.CustomOutputPath) ?
          args.CustomOutputPath :
          "Output";
    }
  }
}
