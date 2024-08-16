using System.IO;
using System.Reflection;

using Grasshopper.Kernel;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]

  public class GsaList_Nodes_Test {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
    }

    [Theory]
    [InlineData("Test1Xs", new double[] { 4.0, 6.0, })]
    [InlineData("Test1Ys", new double[] { 4.0, 7.0, })]
    [InlineData("Test2Xs", new double[] { -6.0, -3.0, })]
    [InlineData("Test2Ys", new double[] { -9.0, -4.0, })]
    [InlineData("Test3X", -6.0)]
    [InlineData("Test3Y", 4.5)]
    [InlineData("Test4X", -6.0)]
    [InlineData("Test4Y", new double[] { 2.0, 10.0, 4.5 })]
    [InlineData("Test5X", -6.0)]
    [InlineData("Test5Y", new double[] { 2.0, 10.0, 4.5 })]
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
