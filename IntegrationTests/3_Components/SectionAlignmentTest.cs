using System;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;

using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class SectionAlignmentTest {
    private static GH_Document Document => document ?? (document = OpenDocument());
    private static GH_Document document = null;

    [Fact]
    public void CalculatedOffsetsTest() {
      GH_Document doc = Document;
      IGH_Param param1 = Helper.FindParameter(doc, "Y");
      var output1 = (GH_Number)param1.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param1.VolatileData.get_Branch(0)[1];
      Assert.Equal(0, output1.Value, DoubleComparer.Default);
      Assert.Equal(-150, output2.Value, DoubleComparer.Default);
      IGH_Param param2 = Helper.FindParameter(doc, "Z");
      var output3 = (GH_Number)param2.VolatileData.get_Branch(0)[0];
      var output4 = (GH_Number)param2.VolatileData.get_Branch(0)[1];
      Assert.Equal(750, output3.Value, DoubleComparer.Default);
      Assert.Equal(350, output4.Value, DoubleComparer.Default);
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
