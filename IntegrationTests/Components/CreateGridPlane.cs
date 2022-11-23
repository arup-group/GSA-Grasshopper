using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace IntegrationTests.Components
{
  [Collection("GrasshopperFixture collection")]
  public class CreateGridPlaneTests
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
      GH_Component comp = Helper.FindComponent(doc, "gps");
      Assert.NotNull(comp);
      // Todo fix: referencing GsaGH directly causes tests to fail
      // Not only this one but tests that otherwise work
      GsaGridPlaneSurfaceGoo output = (GsaGridPlaneSurfaceGoo)ComponentTestHelper.GetOutput(comp);
      GsaGridPlaneSurface gps = output.Value;
      Assert.Equal(42, gps.GridPlaneId);
      Assert.Equal(10, gps.Elevation);
      Assert.Equal("test", gps.GridPlane.Name);
    }
  }
}
