using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using CommandLine;

using DocsGeneration;

namespace DocsGeneratorCLI {
  public static class Program {

    private static int Main(string[] args) {
      Console.WriteLine("Documentation Generator, based on code!");
      int exitCode = 1;

      try {
        Parser.Default.ParseArguments<Options>(args).WithParsed(o => {
          var config = ConfigurationBuilder.BuildConfiguration(o);
          const int overallTimeoutMinutes = 2; // Set overall timeout to 2 minutes
          using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(overallTimeoutMinutes))) {
            var task = CreateTask(config, cts);
            exitCode = RunTask(task, overallTimeoutMinutes);
            if (o.UpdateTestReferences) {
              Console.WriteLine("Copying generated files to E2E test references...");
              var referencePath = ConfigurationBuilder.GetTestReferencePath(o.ProjectName);
              CopyDirectory(o.Output, referencePath);
            }
          }
        });
      } finally {
        // Cleanup Grasshopper fixture to release DLL handles
        try {
          GsaGhDll.Cleanup();
        } catch (Exception ex) {
          Console.Error.WriteLine($"Error during cleanup: {ex.Message}");
        }

        // Ensure all console output is flushed before exiting
        Console.Out.Flush();
        Console.Error.Flush();
      }

      return exitCode;
    }

    public static void CopyDirectory(string sourceDir, string destinationDir) {
      var dir = new DirectoryInfo(sourceDir);
      if (!dir.Exists) {
        throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");
      }

      Directory.CreateDirectory(destinationDir);

      foreach (FileInfo file in dir.GetFiles()) {
        string targetFilePath = Path.Combine(destinationDir, file.Name);
        file.CopyTo(targetFilePath, true);
      }
    }

    private static int RunTask(Task<int> task, int overallTimeoutMinutes) {
      int exitCode;
      try {
        task.Wait();
        exitCode = task.Result;
      } catch (OperationCanceledException) {
        Console.Error.WriteLine($"Documentation generation timed out after {overallTimeoutMinutes} minutes");
        exitCode = -1;
      } catch (AggregateException ae) {
        foreach (var ex in ae.InnerExceptions) {
          if (ex is OperationCanceledException) {
            Console.Error.WriteLine($"Documentation generation timed out after {overallTimeoutMinutes} minutes");
          } else {
            Console.Error.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
          }
        }

        exitCode = -1;
      }

      return exitCode;
    }

    private static Task<int> CreateTask(Configuration config, CancellationTokenSource cts) {
      return Task.Run(() => {
        try {
          GenerateDocumentation.Generate(config);
          return 0;
        } catch (Exception e) {
          Console.Error.WriteLine($"Error during documentation generation: {e.Message}\n{e.StackTrace}");
          return -1;
        }
      }, cts.Token);
    }
  }
}
