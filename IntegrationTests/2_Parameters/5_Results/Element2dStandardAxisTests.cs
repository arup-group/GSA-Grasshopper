using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class Element2dStandardAxisTest {
    private static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeWarningTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning);
    }

    [Fact]
    public void Element2dDisplacementsTest() {
      GH_Document doc = Document;

      IGH_Param d11 = Helper.FindParameter(doc, "d11");
      var outputD11 = (GH_Number)d11.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD11.Value, DoubleComparer.Default);

      IGH_Param d12 = Helper.FindParameter(doc, "d12");
      var outputD12 = (GH_Number)d12.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD12.Value, DoubleComparer.Default);

      IGH_Param d13 = Helper.FindParameter(doc, "d13");
      var outputD13 = (GH_Number)d13.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD13.Value, DoubleComparer.Default);

      IGH_Param d14 = Helper.FindParameter(doc, "d14");
      var outputD14 = (GH_Number)d14.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD14.Value, DoubleComparer.Default);

      IGH_Param d15 = Helper.FindParameter(doc, "d15");
      var outputD15 = (GH_Number)d15.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD15.Value, DoubleComparer.Default);

      IGH_Param d16 = Helper.FindParameter(doc, "d16");
      var outputD16 = (GH_Number)d16.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD16.Value, DoubleComparer.Default);

      IGH_Param d17 = Helper.FindParameter(doc, "d17");
      var outputD17 = (GH_Number)d17.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD17.Value, DoubleComparer.Default);

      IGH_Param d18 = Helper.FindParameter(doc, "d18");
      var outputD18 = (GH_Number)d18.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD18.Value, DoubleComparer.Default);
    }

    [Fact]
    public void Element2dForcesAndMomentsTest() {
      GH_Document doc = Document;

      IGH_Param d21 = Helper.FindParameter(doc, "d21");
      var outputD21 = (GH_Number)d21.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD21.Value, DoubleComparer.Default);

      IGH_Param d22 = Helper.FindParameter(doc, "d22");
      var outputD22 = (GH_Number)d22.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD22.Value, DoubleComparer.Default);

      IGH_Param d23 = Helper.FindParameter(doc, "d23");
      var outputD23 = (GH_Number)d23.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD23.Value, DoubleComparer.Default);

      IGH_Param d24 = Helper.FindParameter(doc, "d24");
      var outputD24 = (GH_Number)d24.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD24.Value, DoubleComparer.Default);

      IGH_Param d25 = Helper.FindParameter(doc, "d25");
      var outputD25 = (GH_Number)d25.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD25.Value, DoubleComparer.Default);

      IGH_Param d26 = Helper.FindParameter(doc, "d26");
      var outputD26 = (GH_Number)d26.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD26.Value, DoubleComparer.Default);

      IGH_Param d27 = Helper.FindParameter(doc, "d27");
      var outputD27 = (GH_Number)d27.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD27.Value, DoubleComparer.Default);

      IGH_Param d28 = Helper.FindParameter(doc, "d28");
      var outputD28 = (GH_Number)d28.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD28.Value, DoubleComparer.Default);
    }

    [Fact]
    public void Element2dStressesTest() {
      GH_Document doc = Document;

      IGH_Param d31 = Helper.FindParameter(doc, "d31");
      var outputD31 = (GH_Number)d31.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD31.Value, DoubleComparer.Default);

      IGH_Param d32 = Helper.FindParameter(doc, "d32");
      var outputD32 = (GH_Number)d32.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD32.Value, DoubleComparer.Default);

      IGH_Param d33 = Helper.FindParameter(doc, "d33");
      var outputD33 = (GH_Number)d33.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD33.Value, DoubleComparer.Default);

      IGH_Param d34 = Helper.FindParameter(doc, "d34");
      var outputD34 = (GH_Number)d34.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD34.Value, DoubleComparer.Default);

      IGH_Param d35 = Helper.FindParameter(doc, "d35");
      var outputD35 = (GH_Number)d35.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD35.Value, DoubleComparer.Default);

      IGH_Param d36 = Helper.FindParameter(doc, "d36");
      var outputD36 = (GH_Number)d36.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD36.Value, DoubleComparer.Default);
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
        "5_Results"
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }
  }
}
