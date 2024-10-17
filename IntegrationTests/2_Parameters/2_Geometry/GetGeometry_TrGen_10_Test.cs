using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class GetGeometry_TrGen_10_Test {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
    }

    [Theory]
    [InlineData("NodeCount", 3027)]
    [InlineData("DamperProp", 0)]
    [InlineData("MassProp", 0)]
    [InlineData("SpringProp", 0)]
    [InlineData("Elem1dCount", 98)]
    [InlineData("Elem1dIDs", new bool[] {
      false,
      true,
    })]
    [InlineData("Elem1dType", "Beam")]
    [InlineData("Elem1dGrp", 4)]
    [InlineData("RotationAngle", 30.0)]
    [InlineData("Elem1dTopo", 45)]
    [InlineData("Elem2dCount", 3)]
    [InlineData("Elem2dsCount", new int[] {
      574,
      288,
      87,
    })]
    [InlineData("Elem2dType", "QUAD8")]
    [InlineData("Elem2dTopo", 87)]
    [InlineData("Mem1dCount", 94)]
    [InlineData("Mem1dGroup", 3)]
    [InlineData("Mem1dType", "Generic 1D")]
    [InlineData("Mem1dElemType", "Beam")]
    [InlineData("Mem1dTopo", "1 13")]
    [InlineData("Mem2dCount", 3)]
    [InlineData("Mem2dType", "Generic 2D")]
    [InlineData("Mem2dAnalysisType", "Linear")]
    [InlineData("Mem2dTopo", "13 16 19 22 23 24 21 18 15 14 V(59 60 61 62) L(19 59) L(22 60)")]
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
        "2_Geometry",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }
  }
}
