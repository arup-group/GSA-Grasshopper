using Commander.NET.Attributes;

namespace DocsGeneratorCLI {
  public class CommandArguments {

    [Parameter("-o", "--output", Description = "Generate documentation into custom  output")]
    public string CustomOutputPath = "Output";
    [Parameter("-e", "--generate-e2e",
      Description = "Generating files DocsGeneration.E2ETests.TestReferences  directory")]
    public bool GenerateE2ETestData = false;

    [Parameter("-p", "--project",
      Description = "\"Name of the project to generate documentation forCurrently,  only 'GsaGH' is supported.",
      Required = Required.Yes)]
    public string ProjectName;
  }
}
