using System.IO;
using System.Linq;
using System.Reflection;

using DocsGeneration;

namespace DocsGeneratorCLI {
  public static class ConfigurationBuilder {
    public static Configuration BuildConfiguration(Options args) {
      var projectTarget = ProjectTarget.LoadProjectTargetFromString(args.ProjectName);

      string outputPath = ResolveOutputPath(args);

      return new Configuration {
        Assembly = projectTarget.Assembly,
        ResultNotes = projectTarget.Notes.ToList(),
        XmlDocument = projectTarget.XmlDoc,
        GenerateE2ETestData = args.GenerateE2ETestData,
        IsBeta = projectTarget.IsBeta,
        CustomOutputPath = outputPath,
      };
    }

    private static string ResolveOutputPath(Options args) {
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
      return dir == null ? throw new DirectoryNotFoundException("Couldn't find GSA-Grasshopper root directory.") :
        dir.FullName;
    }
  }
}
