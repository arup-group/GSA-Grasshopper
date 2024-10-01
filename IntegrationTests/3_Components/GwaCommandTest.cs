using System;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class GwaCommandTest {

    public static GH_Document Document() {
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

    [Fact]
    public void TestGwaCommands() {
      GH_Document doc = Document();
      IGH_Param param1 = Helper.FindParameter(doc, "R_CreateGwaModel");
      var r = (GH_Integer)param1.VolatileData.get_Branch(0)[0];
      Assert.Equal(1, r.Value);

      IGH_Param param2 = Helper.FindParameter(doc, "MatNames");
      var matNames1 = (GH_String)param2.VolatileData.get_Branch(0)[0];
      Assert.Equal("Concrete long term", matNames1.Value);
      var matNames2 = (GH_String)param2.VolatileData.get_Branch(0)[1];
      Assert.Equal("Steel", matNames2.Value);

      IGH_Param param3 = Helper.FindParameter(doc, "ExistingList");
      var exList = (GH_String)param3.VolatileData.get_Branch(0)[0];
      Assert.Equal("1st floor column positions", exList.Value);

      IGH_Param param4 = Helper.FindParameter(doc, "NewList");
      var newList = (GH_String)param4.VolatileData.get_Branch(0)[0];
      Assert.Equal("My list", newList.Value);
    }
  }
}
