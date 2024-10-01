using System.IO;
using System.Reflection;

using Grasshopper.Kernel;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]

  public class GsaList_Properties_Test {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error, "Error");
    }

    [Fact]
    public void NoRuntimeWarningsTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning, "Warning");
    }

    [Theory]
    [InlineData("Test1StartYs", 0.0)]
    [InlineData("Test1StartZs", 7.0)]
    [InlineData("Test1EndYs", 0.0)]
    [InlineData("Test1EndZs", 10.0)]
    [InlineData("Test2StartYs", new double[] { 0.0, 10.0, })]
    [InlineData("Test2StartZs", new double[] { 0.0, 7.0, })]
    [InlineData("Test2EndYs", new double[] { 0.0, 0.0, })]
    [InlineData("Test2EndZs", new double[] { 0.0, 10.0, })]
    [InlineData("Test3Result1", true)]
    [InlineData("Test3Result2", true)]
    [InlineData("Test4Vertices", new int[] { 252, 72 })]
    [InlineData("Test4Faces", new int[] { 63, 18 })]
    [InlineData("Test5Vertices", 72)]
    [InlineData("Test5Faces", 18)]
    [InlineData("Test6IdsAreEqual", 0)]
    [InlineData("Test7Id", 1)]
    [InlineData("Test7Name", "Beams Material list")]
    [InlineData("Test7Type", "Member")]
    [InlineData("Test7Count", 105)]
    [InlineData("Test8Id", 2)]
    [InlineData("Test8Name", "Raft Property list")]
    [InlineData("Test8Type", "Member")]
    [InlineData("Test8Count", 1)]
    [InlineData("Test9Id", 3)]
    [InlineData("Test9Name", "Column Property list")]
    [InlineData("Test9Type", "Element")]
    [InlineData("Test9Count", 60)]
    [InlineData("Test10Id", 4)]
    [InlineData("Test10Name", "Slabs Material list")]
    [InlineData("Test10Type", "Element")]
    [InlineData("Test10Count", 6)]
    [InlineData("Test11Id", 1)]
    [InlineData("Test11Name", "E3d list")]
    [InlineData("Test11Type", "Element")]
    [InlineData("Test11Count", 1)]
    public void Test(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
    }

    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);
      fileName = fileName.Replace("_Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Parameters",
        "0_Model",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }
  }
}
