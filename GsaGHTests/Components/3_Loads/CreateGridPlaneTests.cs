using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace ComponentsTest
{
  [Collection("GrasshopperFixture collection")]
  public class CreateGridPlaneTests
  {
    public static GH_OasysDropDownComponent ComponentMother()
    {
      var comp = new CreateGridPlane();
      comp.CreateAttributes();

      comp.SetSelected(1, 4); // set unit dropdown to "ft"
      return comp;
    }

    [Fact]
    public void CreateGridPlaneTest()
    {
      var comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, 42, 1);
      ComponentTestHelper.SetInput(comp, 10, 2);
      ComponentTestHelper.SetInput(comp, "test", 3);

      GsaGridPlaneSurfaceGoo output = (GsaGridPlaneSurfaceGoo)ComponentTestHelper.GetOutput(comp);

      GsaGridPlaneSurface gridPlane = null;
      output.CastTo(ref gridPlane);

      Assert.Equal(42, gridPlane.GridPlaneID);
      //Assert.Equal(10, gridPlane.Elevation);
      //Assert.Equal("test", gridPlane.Name);
    }
  }
}
