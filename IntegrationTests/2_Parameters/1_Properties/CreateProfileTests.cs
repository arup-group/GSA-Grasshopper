using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class CreateProfileTests {
    public static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeErrorTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
    }

    [Fact]
    public void NoRuntimeWarningTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning);
    }

    [Theory]
    [InlineData("Angle", "STD A(mm) 500 100 10 20 [R(0)]")]
    [InlineData("Channel", "STD CH(cm) 50 10 1 2")]
    [InlineData("CircleHollow", "STD CHS(m) 5 0.1")]
    [InlineData("Circle", "STD C(in) 11")]
    [InlineData("CruciformSymmetrical", "STD X(mm) 500 100 10 20")]
    [InlineData("EllipseHollow", "STD OVAL(mm) 500 100 10")]
    [InlineData("GeneralC", "STD GC(mm) 500 100 10 20")]
    [InlineData("GeneralZ", "STD GZ(mm) 500 100 10 20 30 30")]
    [InlineData("IBeamAsymmetrical", "STD GI(mm) 500 100 10 20 30 30")]
    [InlineData("IBeamCellular", "STD CB(mm) 500 100 10 20 300 3000")]
    [InlineData("IBeamSymmetrical", "STD I(mm) 500 100 10 20")]
    [InlineData("Perimeter", "GEO P(m) M(1|2) L(1|-2) L(-1|-2) L(-1|2)")]
    [InlineData("RectangleHollow", "STD RHS(mm) 500 100 10 20")]
    [InlineData("Rectangle", "STD R(mm) 500 100")]
    [InlineData("RectoEllipse", "STD RE(mm) 500 400 350 300 2")]
    [InlineData("RectoCircle", "STD RC(mm) 500 100")]
    [InlineData("SecantPile", "STD SPW(mm) 500 100 10")]
    [InlineData("SheetPile", "STD SHT(mm) 500 100 10 20 30 30")]
    [InlineData("Trapezoid", "STD TR(mm) 500 100 10")]
    [InlineData("T", "STD T(mm) 500 100 10 20")]
    public void OutputTest(string groupIdentifier, string expected) {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, groupIdentifier);
      var output = (GH_String)param.VolatileData.get_Branch(0)[0];
      Assert.Equal(expected, output.Value);
    }

    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);
      fileName = fileName.Replace("Tests", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Parameters",
        "1_Properties",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }
  }
}
