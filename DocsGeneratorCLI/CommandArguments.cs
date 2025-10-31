using Commander.NET.Attributes;

namespace DocsGeneratorCLI {
  public class CommandArguments {
    [Parameter("-o", "--output", Description = "Generate documentation into custom  output")]
    public string CustomOutputPath { get; set; } = "Output";

    [Parameter("-e", "--generate-e2e", Description = "Generate files DocsGeneration.E2ETests.TestReferences directory")]
    public bool GenerateE2ETestData { get; set; } = false;

    [Parameter("-p", "--project",
      Description = "Name of the project to generate documentation for. Currently, only 'GsaGH' is supported.",
      Required = Required.Yes)]
    public string ProjectName { get; set; }
  }
}
