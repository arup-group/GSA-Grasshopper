using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Xunit;

namespace IntegrationTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class Element1dStandardAxisTest {
    private static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void NoRuntimeWarningTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning);
    }

    [Fact]
    public void Member1dDisplacementsTest() {
      GH_Document doc = Document;

      IGH_Param d11 = Helper.FindParameter(doc, "d11");
      var outputD11 = (GH_Number)d11.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD11.Value);

      IGH_Param d12 = Helper.FindParameter(doc, "d12");
      var outputD12 = (GH_Number)d12.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD12.Value);

      IGH_Param d13 = Helper.FindParameter(doc, "d13");
      var outputD13 = (GH_Number)d13.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD13.Value);

      IGH_Param d14 = Helper.FindParameter(doc, "d14");
      var outputD14 = (GH_Number)d14.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD14.Value);
    }

    [Fact]
    public void BeamDisplacementsTest() {
      GH_Document doc = Document;

      IGH_Param d21 = Helper.FindParameter(doc, "d21");
      var outputD21 = (GH_Number)d21.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD21.Value);

      IGH_Param d22 = Helper.FindParameter(doc, "d22");
      var outputD22 = (GH_Number)d22.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD22.Value);

      IGH_Param d23 = Helper.FindParameter(doc, "d23");
      var outputD23 = (GH_Number)d23.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD23.Value);

      IGH_Param d24 = Helper.FindParameter(doc, "d24");
      var outputD24 = (GH_Number)d24.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD24.Value);
    }

    [Fact]
    public void NodeDisplacementsTest() {
      GH_Document doc = Document;

      IGH_Param d31 = Helper.FindParameter(doc, "d31");
      var outputD31 = (GH_Number)d31.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD31.Value);

      IGH_Param d32 = Helper.FindParameter(doc, "d32");
      var outputD32 = (GH_Number)d32.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD32.Value);

      IGH_Param d33 = Helper.FindParameter(doc, "d33");
      var outputD33 = (GH_Number)d33.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD33.Value);

      IGH_Param d34 = Helper.FindParameter(doc, "d34");
      var outputD34 = (GH_Number)d34.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD34.Value);
    }

    [Fact]
    public void ReactionForcesTest() {
      GH_Document doc = Document;

      IGH_Param d41 = Helper.FindParameter(doc, "d41");
      var outputD41 = (GH_Number)d41.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD41.Value);

      IGH_Param d42 = Helper.FindParameter(doc, "d42");
      var outputD42 = (GH_Number)d42.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD42.Value);

      IGH_Param d43 = Helper.FindParameter(doc, "d43");
      var outputD43 = (GH_Number)d43.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD43.Value);

      IGH_Param d44 = Helper.FindParameter(doc, "d44");
      var outputD44 = (GH_Number)d44.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD44.Value);

      IGH_Param d45 = Helper.FindParameter(doc, "d45");
      var outputD45 = (GH_Number)d45.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD45.Value);

      IGH_Param d46 = Helper.FindParameter(doc, "d46");
      var outputD46 = (GH_Number)d46.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD46.Value);

      IGH_Param d47 = Helper.FindParameter(doc, "d47");
      var outputD47 = (GH_Number)d47.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD47.Value);

      IGH_Param d48 = Helper.FindParameter(doc, "d48");
      var outputD48 = (GH_Number)d48.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD48.Value);
    }

    [Fact]
    public void SpringReactionForcesTest() {
      GH_Document doc = Document;

      IGH_Param d51 = Helper.FindParameter(doc, "d51");
      var outputD51 = (GH_Number)d51.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD51.Value);

      IGH_Param d52 = Helper.FindParameter(doc, "d52");
      var outputD52 = (GH_Number)d52.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD52.Value);

      IGH_Param d53 = Helper.FindParameter(doc, "d53");
      var outputD53 = (GH_Number)d53.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD53.Value);

      IGH_Param d54 = Helper.FindParameter(doc, "d54");
      var outputD54 = (GH_Number)d54.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD54.Value);

      IGH_Param d55 = Helper.FindParameter(doc, "d55");
      var outputD55 = (GH_Number)d55.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD55.Value);

      IGH_Param d56 = Helper.FindParameter(doc, "d56");
      var outputD56 = (GH_Number)d56.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD56.Value);

      IGH_Param d57 = Helper.FindParameter(doc, "d57");
      var outputD57 = (GH_Number)d57.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD57.Value);

      IGH_Param d58 = Helper.FindParameter(doc, "d58");
      var outputD58 = (GH_Number)d58.VolatileData.get_Branch(0)[0];
      Assert.Equal(0, outputD58.Value);
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
