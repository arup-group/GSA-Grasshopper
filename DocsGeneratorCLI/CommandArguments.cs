using CommandLine;

namespace DocsGeneratorCLI {
  public class Options {
    [Option('o', "output", Required = false, HelpText = "Generate documentation into custom  output")]
    public string Output { get; set; } = "Output";

    [Option('e', "generate-e2e", HelpText = "Generate files DocsGeneration.E2ETests.TestReferences directory")]
    public bool UpdateTestReferences { get; set; } = false;

    [Option('p', "project",
      HelpText = "Name of the project to generate documentation for. Currently, only 'GsaGH' is supported.",
      Required = true)]
    public string ProjectName { get; set; }
  }
}
