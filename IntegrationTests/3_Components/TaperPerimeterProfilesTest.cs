using System;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class TaperPerimeterProfilesTest {
    private static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void IncorrectProfilesTest() {
      GH_Document doc = Document;
      GH_Component comp = Helper.FindComponent(doc, "Test2");

      object output = comp.Params.Output[0].VolatileData.get_Branch(0)[0];
      Assert.Null(output);

      Assert.Equal(GH_RuntimeMessageLevel.Error, comp.RuntimeMessageLevel);

      string expected = "Start and End Profile must contain similar number of points";
      Assert.Equal(expected, comp.RuntimeMessages(GH_RuntimeMessageLevel.Error)[0]);
    }

    [Fact]
    public void PerimeterProfilesTaperTest() {
      GH_Document doc = Document;
      IGH_Param param = Helper.FindParameter(doc, "Test1");
      var output = (GH_String)param.VolatileData.get_Branch(0)[0];

      string expected
        = "GEO P(m) M(-0.01|0.015) L(-0.01|-0.015) L(0.01|-0.015) L(0.01|0.015) " +
          ": M(0.015|0.01) L(0.015|-0.01) L(-0.015|-0.01) L(-0.015|0.01)";
      Assert.Equal(expected, output.Value);
    }

    private static GH_Document OpenDocument() {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("Test", string.Empty);

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
