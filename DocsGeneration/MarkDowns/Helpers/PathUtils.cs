using System;
using System.IO;
using System.Linq;

namespace DocsGeneration.MarkDowns.Helpers {
  /// <summary>
  ///   Supports different modes based on command line arguments:
  ///   - (--test) sets the output path to the TestReferences directory inside the E2E test project.
  ///   - (--output &lt;path&gt;) sets a custom output directory.
  ///   - by default, output path is set to "Output".
  /// </summary>
  internal static class PathUtils {
    public static string OutputPath { get; private set; } = GetOutputPath(Environment.GetCommandLineArgs());

    private static string GetOutputPath(string[] args) {
      args = args.Skip(1).ToArray(); // skip the first argument which is the executable path

      if (IsTestMode(args)) { // --test
        return GetTestReferencesPath();
      }

      string outputPath = GetCustomOutputPath(args);
      return !string.IsNullOrEmpty(outputPath) ? outputPath : GetDefaultOutputPath();
    }

    private static bool IsTestMode(string[] args) {
      return args.Contains("--test");
    }

    private static string GetTestReferencesPath() {
      string rootDir = GetGsaGrasshopperRoot();
      string testReferencesPath = Path.Combine(rootDir, "DocsGeneration.E2ETests", "TestReferences");
      Console.WriteLine($"Generating files into TestReferences directory: {testReferencesPath}");
      return testReferencesPath;
    }

    private static string GetCustomOutputPath(string[] args) {
      int outputIndex = Array.IndexOf(args, "--output");
      if (outputIndex < 0 || outputIndex + 1 >= args.Length) {
        return null;
      }

      string outputPath = args[outputIndex + 1];
      Console.WriteLine($"Generating files into (custom output): {outputPath}");
      return outputPath;
    }

    private static string GetDefaultOutputPath() {
      Console.WriteLine("Generating files into: Output");
      return "Output";
    }

    private static string GetGsaGrasshopperRoot() {
      var dir = new DirectoryInfo(AppContext.BaseDirectory);
      while (dir != null && dir.Name != "GSA-Grasshopper") {
        dir = dir.Parent; // find the GSA-Grasshopper directory
      }

      return dir == null ? throw new DirectoryNotFoundException("Couldn't find GSA-Grasshopper in path provided.") :
        dir.FullName;
    }
  }
}
