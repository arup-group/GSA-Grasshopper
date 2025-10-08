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
      string testPath = Path.Combine(GetGsaGrasshopperRoot(), "DocsGeneration.E2ETests", "TestReferences",
        args.ProjectName);
      string customOutputPath = !string.IsNullOrWhiteSpace(args.CustomOutputPath) ? args.CustomOutputPath : "Output";

      return args.GenerateE2ETestData ? testPath : customOutputPath;
    }

    private static string GetGsaGrasshopperRoot() {
      string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

      var dir = new DirectoryInfo(assemblyLocation);
      while (dir != null && dir.Name != "DocsGeneratorCLI") {
        dir = dir.Parent;
      }

      dir = dir?.Parent; // move up to the root of the GSA-Grasshopper project
      return dir == null ? throw new DirectoryNotFoundException("Couldn't find GSA-Grasshopper in path provided.") :
        dir.FullName;
    }
  }
}
