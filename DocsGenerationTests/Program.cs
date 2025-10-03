using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DocsGeneration.E2ETests {
  public static class Program {
    private static readonly string[] separator = { "\r\n", "\n" };
    private static readonly string expectedDir = Path.Combine(AppContext.BaseDirectory, "TestReferences");
    private static readonly string generatedDir = Path.Combine(AppContext.BaseDirectory, "test-artifacts", "generated");

    public static int Main() {
      Console.WriteLine("Starting DocsGeneration E2E Test...");

      bool success = true;
      try {
        PrepareOutputDirectory();

        if (!RunGenerator()) {
          Console.WriteLine("❌ Generator failed.");
          return 1;
        }

        string[] expectedFiles = GetRelativeMarkdownFilePaths(expectedDir);
        string[] generatedFiles = GetRelativeMarkdownFilePaths(generatedDir);

        success &= CheckFilesCountMatch(expectedFiles, generatedFiles);
        success &= CheckNoMissingFiles(expectedFiles, generatedFiles);
        success &= CheckNoUnexpectedFiles(expectedFiles, generatedFiles);
        success &= CheckFilesContentEqual(expectedDir, generatedDir, expectedFiles);
      }
      catch (Exception ex) {
        Console.WriteLine($"❌ Test failed with exception: {ex.Message}");
        success = false;
      }
      finally {
        // CleanupGeneratedDirectory();
      }

      if (success) {
        Console.WriteLine("✅ All tests passed successfully!");
        return 0;
      } else {
        Console.WriteLine("❌ Some tests failed.");
        return 1;
      }
    }

    private static void PrepareOutputDirectory() {
      if (Directory.Exists(generatedDir)) {
        Directory.Delete(generatedDir, true);
      }

      Directory.CreateDirectory(generatedDir);
      Console.WriteLine("Prepared output directory.");
    }

    private static void CleanupGeneratedDirectory() {
      if (Directory.Exists(generatedDir)) {
        Directory.Delete(generatedDir, true);
      }

      Console.WriteLine("Cleaned up generated directory.");
    }

    private static bool RunGenerator() {
      string generatorExePath = GetGeneratorPath();
      if (!File.Exists(generatorExePath)) {
        Console.WriteLine($"❌ Couldn't find DocsGeneration.exe at {generatorExePath}");
        return false;
      }

      Console.WriteLine($"Running generator: {generatorExePath}");
      var startInfo = new ProcessStartInfo {
        FileName = generatorExePath,
        Arguments = $"--output {generatedDir}",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = false,
      };

      using (var process = new Process { StartInfo = startInfo }) {
        process.OutputDataReceived += (s, e) => {
          if (!string.IsNullOrEmpty(e.Data)) {
            Console.WriteLine(e.Data);
          }
        };
        process.ErrorDataReceived += (s, e) => {
          if (!string.IsNullOrEmpty(e.Data)) {
            Console.Error.WriteLine(e.Data);
          }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        if (process.ExitCode != 0) {
          Console.WriteLine($"❌ DocsGeneration.exe exited with code {process.ExitCode}");
          return false;
        }

        Console.WriteLine("✅ Generator finished successfully.");
        return true;
      }
    }

    private static string GetGeneratorPath() {
#if DEBUG
      string config = "Debug";
#else
      string config = "Release";
#endif
      int maxLevelUp = 3;
      string root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
      string generatorPath = string.Empty;

      do {
        generatorPath = Path.GetFullPath(Path.Combine(root, "DocsGeneration", "bin", "x64", config, "net48", "DocsGeneration.exe"));
        if (File.Exists(generatorPath)) {
          return generatorPath;
        }

        root = Path.GetFullPath(Path.Combine(root, ".."));
        maxLevelUp--;
      } while (maxLevelUp >= 0);

      return generatorPath;
    }

    private static string[] GetRelativeMarkdownFilePaths(string rootDirectory) =>
      Directory.GetFiles(rootDirectory, "*.*", SearchOption.AllDirectories)
        .Select(path => GetRelativePath(rootDirectory, path)).OrderBy(path => path).ToArray();

    private static string GetRelativePath(string root, string fullPath) {
      var rootUri = new Uri(root + Path.DirectorySeparatorChar);
      var fileUri = new Uri(fullPath);
      Uri relativeUri = rootUri.MakeRelativeUri(fileUri);
      return Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar);
    }

    private static bool CheckFilesCountMatch(string[] expected, string[] generated) {
      if (expected.Length != generated.Length) {
        Console.WriteLine($"❌ File count mismatch. Expected {expected.Length}, got {generated.Length}");
        return false;
      }
      Console.WriteLine("✅ File count matches.");
      return true;
    }

    private static bool CheckNoMissingFiles(string[] expected, string[] generated) {
      string[] missing = expected.Except(generated).ToArray();
      if (missing.Length > 0) {
        Console.WriteLine($"❌ Missing files: {string.Join(", ", missing)}");
        return false;
      }
      Console.WriteLine("✅ No missing files.");
      return true;
    }

    private static bool CheckNoUnexpectedFiles(string[] expected, string[] generated) {
      string[] unexpected = generated.Except(expected).ToArray();
      if (unexpected.Length > 0) {
        Console.WriteLine($"❌ Unexpected files: {string.Join(", ", unexpected)}");
        return false;
      }
      Console.WriteLine("✅ No unexpected files.");
      return true;
    }

    private static bool CheckFilesContentEqual(string expectedDir, string generatedDir, string[] expectedFiles) {
      bool success = true;
      foreach (string relPath in expectedFiles) {
        string expectedPath = Path.Combine(expectedDir, relPath);
        string generatedPath = Path.Combine(generatedDir, relPath);

        string expectedContent = File.ReadAllText(expectedPath);
        string actualContent = File.ReadAllText(generatedPath);

        if (expectedContent == actualContent) {
          continue;
        }

        string[] expectedLines = expectedContent.Split(separator, StringSplitOptions.None);
        string[] actualLines = actualContent.Split(separator, StringSplitOptions.None);

        if (!CompareLines(expectedLines, actualLines, relPath)) {
          success = false;
        }
      }
      return success;
    }

    private static bool CompareLines(string[] expected, string[] actual, string relPath) {
      int max = Math.Max(expected.Length, actual.Length);
      bool match = true;

      for (int i = 0; i < max; i++) {
        string e = i < expected.Length ? expected[i] : "(missing)";
        string a = i < actual.Length ? actual[i] : "(missing)";
        if (e != a) {
          Console.WriteLine($"❌ Difference in '{relPath}' at line {i + 1}");
          Console.WriteLine($"   Expected: {e}");
          Console.WriteLine($"   Found:    {a}");
          match = false;
        }
      }
      if (match) {
        Console.WriteLine($"✅ {relPath} matches expected content.");
      }

      return match;
    }
  }
}
