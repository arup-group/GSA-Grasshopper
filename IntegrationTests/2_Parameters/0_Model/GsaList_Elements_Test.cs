using System.IO;
using System.Reflection;

using Grasshopper.Kernel;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]

  public class GsaList_Elements_Test {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
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
    [InlineData("Test4Vertices", 252)]
    [InlineData("Test4Faces", 63)]
    [InlineData("Test5Vertices", 72)]
    [InlineData("Test5Faces", 18)]
    [InlineData("Test6IdsAreEqual", 0)]
    [InlineData("Test7Verticies", 120)]
    [InlineData("Test7Faces", 90)]
    [InlineData("Test8IdsAreEqual", 0)]
    [InlineData("Test9Result1", true)]
    [InlineData("Test9Result2", true)]
    [InlineData("Test9Result3", true)]
    [InlineData("Test10Result1", true)]
    [InlineData("Test10Result2", true)]
    [InlineData("Test10Result3", true)]
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
