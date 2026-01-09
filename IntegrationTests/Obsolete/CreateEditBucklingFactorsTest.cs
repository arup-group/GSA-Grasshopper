using System.IO;
using System.Reflection;

using GsaGH.Helpers;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Xunit;

namespace IntegrationTests.Obsolete {
  [Collection("GrasshopperFixture collection")]
  public class CreateEditBucklingFactorsTest {
    private static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeWarningTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning);
    }

    [Fact]
    public void OutputTest() {
      GH_Document doc = Document;

      IGH_Param y = Helper.FindParameter(doc, "TestLsy");
      var outputY = (GH_Number)y.VolatileData.get_Branch(0)[0];
      Assert.Equal(0.9, outputY.Value, DoubleComparer.Default);

      IGH_Param z = Helper.FindParameter(doc, "TestLsz");
      var outputZ = (GH_Number)z.VolatileData.get_Branch(0)[0];
      Assert.Equal(1.5, outputZ.Value, DoubleComparer.Default);

      IGH_Param lt = Helper.FindParameter(doc, "TestLtb");
      var outputLtb = (GH_Number)lt.VolatileData.get_Branch(0)[0];
      Assert.Equal(2.0, outputLtb.Value, DoubleComparer.Default);

      IGH_Param integ = Helper.FindParameter(doc, "null tests");
      var outputInteger = (GH_Integer)integ.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputInteger.Value);
    }

    private static GH_Document OpenDocument() {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Obsolete.", string.Empty);
      fileName = fileName.Replace("Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Obsolete"
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }
  }
}
