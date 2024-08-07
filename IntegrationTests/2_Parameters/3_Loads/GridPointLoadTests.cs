using System.IO;
using System.Reflection;

using Grasshopper.Kernel;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GridPointLoadTests {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Theory]
    [InlineData("LoadIsEqual", 0)]
    [InlineData("PointIsEqual", 0)]
    public void TestTxtInput(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
    }

    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty)
       .Replace("Tests", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Parameters",
        "3_Loads",
      });
      var io = new GH_DocumentIO();
      Assert.True(File.Exists(Path.Combine(path, fileName)));
      Assert.True(io.Open(Path.Combine(path, fileName)));
      io.Document.NewSolution(true);

      return io.Document;
    }
  }
}
