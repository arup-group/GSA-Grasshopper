using System;
using System.IO;
using System.Linq;
using System.Reflection;

using DocsGeneration;

namespace DocsGeneratorCLI {
  public static class ConfigurationBuilder {
    public static Configuration BuildConfiguration(Options args) {
      if (string.IsNullOrWhiteSpace(args.ProjectName)) {
        throw new ArgumentException("Project name cannot be null or empty.");
      }

      var projectTarget = new GsaGhProject(args.ProjectName);

      return new Configuration {
        Assembly = projectTarget.Assembly,
        ResultNotes = projectTarget.Notes.ToList(),
        XmlDocument = projectTarget.XmlDoc,
        GenerateE2ETestData = args.UpdateTestReferences,
        IsBeta = projectTarget.IsBeta,
        OutputPath = args.Output,
      };
    }

    public static string GetTestReferencePath(string projectName)
    {
      return Path.Combine(GetGsaGrasshopperRoot(), "DocsGeneration.E2ETests", "TestReferences",
        projectName);
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
