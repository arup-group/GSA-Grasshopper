using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class GlobalPerformance_BucklingTests {
    private static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void LoadFactorTest() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "LoadFactors");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      Assert.Equal(-0.4183, output1.Value, 4);
      Assert.Equal(-0.5784, output2.Value, 4);
      Assert.Equal(-0.8993, output3.Value, 4);
    }

    [Fact]
    public void ModesTest() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "Modes");
      var output1 = (GH_Integer)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Integer)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Integer)param.VolatileData.get_Branch(0)[2];
      Assert.Equal(1, output1.Value);
      Assert.Equal(2, output2.Value);
      Assert.Equal(3, output3.Value);
    }

    private static GH_Document OpenDocument() {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("Tests", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Components",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }
  }
}
