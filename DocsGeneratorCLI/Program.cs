using System;
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
          
          // Set overall timeout for the entire documentation generation process
          const int overallTimeoutMinutes = 30;
          using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(overallTimeoutMinutes))) {
            var task = Task.Run(() => {
              try {
                GenerateDocumentation.Generate(config);
                return 0;
              } catch (Exception e) {
                Console.Error.WriteLine($"Error during documentation generation: {e.Message}\n{e.StackTrace}");
                return -1;
              }
            }, cts.Token);

            try {
              task.Wait(cts.Token);
              exitCode = task.Result;
            } catch (OperationCanceledException) {
              Console.Error.WriteLine($"Documentation generation timed out after {overallTimeoutMinutes} minutes");
              exitCode = -1;
            } catch (AggregateException ae) {
              foreach (var ex in ae.InnerExceptions) {
                Console.Error.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
              }
              exitCode = -1;
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
  }
}
