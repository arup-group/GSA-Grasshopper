using System.Diagnostics;

namespace DocsGeneration.E2ETests {
  public class DocsGenerationE2ETest {
    public class MarkdownGenerationTests {
      private static readonly string[] separator = {
        "\r\n",
        "\n",
      };
      private readonly string expectedDir = Path.Combine(AppContext.BaseDirectory, "TestReferences");
      private readonly string generatedDir = Path.Combine(AppContext.BaseDirectory, "test-artifacts", "generated");

      [Fact]
      public void Should_Generate_Files_Matching_Expected() {
        PrepareOutputDirectory();

        try {
          RunGenerator();

          string[] expectedFiles = GetRelativeMarkdownFilePaths(expectedDir);
          string[] generatedFiles = GetRelativeMarkdownFilePaths(generatedDir);

          AssertFilesCountMatch(expectedFiles, generatedFiles);
          AssertNoMissingFiles(expectedFiles, generatedFiles);
          AssertNoUnexpectedFiles(expectedFiles, generatedFiles);
          AssertFilesContentEqual(expectedDir, generatedDir, expectedFiles);
        } finally {
          CleanupGeneratedDirectory();
        }
      }

      private void PrepareOutputDirectory() {
        if (Directory.Exists(generatedDir)) {
          Directory.Delete(generatedDir, true);
        }

        Directory.CreateDirectory(generatedDir);
      }

      private void CleanupGeneratedDirectory() {
        if (Directory.Exists(generatedDir)) {
          Directory.Delete(generatedDir, true);
        }
      }

      private void RunGenerator() {
        string generatorExePath = GetGeneratorPath();

        if (!File.Exists(generatorExePath)) {
          throw new FileNotFoundException("Couldn't find DocsGeneration.exe", generatorExePath);
        }

        var startInfo = new ProcessStartInfo {
          FileName = generatorExePath,
          Arguments = $"--output {generatedDir}",
          RedirectStandardOutput = true,
          RedirectStandardError = true,
          UseShellExecute = false,
        };

        using var process = new Process {
          StartInfo = startInfo,
        };

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
          throw new Exception($"DocsGeneration.exe exited with code: {process.ExitCode}");
        }
      }

      private static string GetGeneratorPath() {
        DirectoryInfo directoryInfo = GetGsaGrasshopperRoot();

        string generatorPath = Path.Combine(directoryInfo.FullName, "DocsGeneration", "bin", "x64", "Debug", "net48",
          "DocsGeneration.exe");

        return !File.Exists(generatorPath) ?
          throw new FileNotFoundException("Couldn't find: DocsGeneration.exe", generatorPath) : generatorPath;
      }

      private static DirectoryInfo GetGsaGrasshopperRoot() {
        var directoryInfo = new DirectoryInfo(AppContext.BaseDirectory);
        while (directoryInfo != null && directoryInfo.Name != "GSA-Grasshopper") {
          directoryInfo = directoryInfo.Parent; // find the GSA-Grasshopper directory
        }

        return directoryInfo ?? throw new DirectoryNotFoundException("Couldn't find GSA-Grasshopper in path provided.");
      }

      private static string[] GetRelativeMarkdownFilePaths(string rootDirectory) {
        return Directory.GetFiles(rootDirectory, "*.*", SearchOption.AllDirectories)
         .Select(path => Path.GetRelativePath(rootDirectory, path)).OrderBy(path => path).ToArray();
      }

      private static void AssertFilesCountMatch(string[] expectedFiles, string[] generatedFiles) {
        Assert.Equal(expectedFiles.Length, generatedFiles.Length);
      }

      private static void AssertNoMissingFiles(string[] expectedFiles, string[] generatedFiles) {
        string[] missing = expectedFiles.Except(generatedFiles).ToArray();
        Assert.True(missing.Length == 0, $"Missing files: {string.Join(", ", missing)}");
      }

      private static void AssertNoUnexpectedFiles(string[] expectedFiles, string[] generatedFiles) {
        string[] unexpected = generatedFiles.Except(expectedFiles).ToArray();
        Assert.True(unexpected.Length == 0, $"Unexpected files: {string.Join(", ", unexpected)}");
      }

      private static void AssertFilesContentEqual(string expectedDir, string generatedDir, string[] expectedFiles) {
        foreach (string relativePath in expectedFiles) {
          string expectedPath = Path.Combine(expectedDir, relativePath);
          string generatedPath = Path.Combine(generatedDir, relativePath);

          string expectedContent = File.ReadAllText(expectedPath);
          string actualContent = File.ReadAllText(generatedPath);

          if (expectedContent == actualContent) {
            continue;
          }

          string[] expectedLines = expectedContent.Split(separator, StringSplitOptions.None);
          string[] actualLines = actualContent.Split(separator, StringSplitOptions.None);

          FindDifferencesBetweenFiles(expectedLines, actualLines, relativePath);
        }
      }

      private static void FindDifferencesBetweenFiles(
        string[] expectedLines, string[] actualLines, string relativePath) {
        int maxLines = Math.Max(expectedLines.Length, actualLines.Length);

        for (int i = 0; i < maxLines; i++) {
          string expectedLine = i < expectedLines.Length ? expectedLines[i] : "(line missing)";
          string actualLine = i < actualLines.Length ? actualLines[i] : "(line missing)";

          if (expectedLine == actualLine) {
            continue;
          }

          int diffLine = i + 1;
          string message
            = $"Difference in file: '{relativePath}'\nLine {diffLine}:\nExpected: '{expectedLine}'\nFound:    '{actualLine}'";
          Assert.True(false, message);
        }
      }
    }
  }
}
