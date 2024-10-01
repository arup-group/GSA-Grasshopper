using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class SteelDesignAndTaskTest {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeWarningTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning, "Warning");
    }

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error, "Error");
    }

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
    [InlineData("ListDefinition3", "all")]
    public void SteelDesignTaskTest(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
    }

    [Theory]
    [InlineData("NoIterations", "Design Task succeeded with 2 changed section(s) and leaving 1 section(s) unchanged\nRemember to synchronise the changes to the Analysis layer!")]
    [InlineData("Converged1", "Optimisation converged after 1 iteration(s) with 2 changed section(s) and leaving 1 section(s) unchanged")]
    [InlineData("Converged2", "Optimisation converged after 2 iteration(s) with 3 changed section(s)")]
    [InlineData("NotConverged", "Optimisation did not converge within 1 iterations with 3 changed section(s)")]
    [InlineData("AnalysisError", "Section Library 1: material grade is undefined.")]
    [InlineData("InefficiencyWarning", "\tMember 1 is inefficient. (Section utilisation is less than 0.5)")]
    [InlineData("DesignTaskIdError", true)]
    public void SteelDesignTest(string groupIdentifier, object expected) {
      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
      Helper.TestGhPrimitives(param, expected);
    }

    [Fact]
    public void CheckOptionTest() {
      IGH_Param param = Helper.FindParameter(Document, "CheckOption");
      var valOut = (GH_String)param.VolatileData.get_Branch(0)[0];
      Assert.Contains("Option: Check", valOut.Value);
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
