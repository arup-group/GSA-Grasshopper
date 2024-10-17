using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaList_Materials_Test {
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
    [InlineData("List from material Definitions", new string[] {
      "MC1",
      "MC1",
      "MS1",
      "MS1",
      "MP1",
      "MP1",
      "M1",
      "M1",
    })]
    public void MaterialDefinitionTest(string groupIdentifier, string[] expectedVals) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      var output = (List<GH_String>)param.VolatileData.get_Branch(0);
      for (int i = 0; i < expectedVals.Length; i++) {
        Assert.Equal(expectedVals[i], output[i].Value);
      }
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
