using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class SteelDesignTaskTest {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Theory]
    [InlineData("Name1", "Steel design task")]
    [InlineData("Id1", 1)]
    [InlineData("ListDefinition1", "all")]
    [InlineData("Case1", 1)]
    [InlineData("eta1", 0.9)]
    [InlineData("etamin1", 0.05)]
    [InlineData("Group1", false)]
    [InlineData("Primary1", "MinWeight")]
    [InlineData("Secondary1", "MinDepth")]
    [InlineData("Name2", "Steel design task main beams")]
    [InlineData("Id2", 4)] 
    [InlineData("ListDefinition2", "2 1")]
    [InlineData("Case2", 1)] 
    [InlineData("eta2", 0.85)]
    [InlineData("etamin2", 0.65)]
    [InlineData("Group2", true)]
    [InlineData("Primary2", "MinCost")]
    [InlineData("Secondary2", "MaxSustainability")]
    public void Test(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
    }

    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);
      fileName = fileName.Replace("Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Parameters",
        "4_Analysis",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    private IGH_Param TestHelper(string groupIdentifier) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      return param;
    }
  }
}
