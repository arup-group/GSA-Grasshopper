using System.IO;
using System.Reflection;

using Grasshopper.Kernel;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaList_Members_Test {
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
    [InlineData("Test1Id", 1)]
    [InlineData("Test1Name", "Columns")]
    [InlineData("Test1Type", "Member")]
    [InlineData("Test1Count", 12)]
    [InlineData("Test2Id", 2)]
    [InlineData("Test2Name", "Typical Floors")]
    [InlineData("Test2Type", "Member")]
    [InlineData("Test2Count", 5)]
    [InlineData("Test3Id", 3)]
    [InlineData("Test3Name", "Primaries")]
    [InlineData("Test3Type", "Member")]
    [InlineData("Test3Count", 45)]
    [InlineData("Test4Id", 4)]
    [InlineData("Test4Name", "Member List [4]")]
    [InlineData("Test4Type", "Member")]
    [InlineData("Test4Count", 60)]
    [InlineData("Test5Id", 11)]
    [InlineData("Test5Name", "Ground floor")]
    [InlineData("Test5Type", "Member")]
    [InlineData("Test5Count", 1)]
    [InlineData("Test6Id", 99)]
    [InlineData("Test6Name", "3d list")]
    [InlineData("Test6Type", "Member")]
    [InlineData("Test6Count", 1)]
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
