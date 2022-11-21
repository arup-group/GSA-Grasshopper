using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using Xunit;

namespace IntegrationTests.Components
{
  [Collection("GrasshopperFixture collection")]
  public class GridPlaneSurfaceTests
  {
    public static GH_Document Document()
    {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("Tests", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Components"});
      
      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Fact]
    public void GridPlaneSurfaceTest()
    {
      GH_Document doc = Document();
      IGH_Param param = Helper.FindParameter(doc, "gps");
      GsaGridPlaneSurfaceGoo output = (GsaGridPlaneSurfaceGoo)param.VolatileData.get_Branch(0)[0];
      GsaGridPlaneSurface gps = output.Value;
      Assert.Equal(42, gps.GridSurfaceID);
    }
  }
}
