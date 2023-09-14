using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class CreateGridLineTest {

    public static GH_OasysComponent GridLineComponentMother() {
      var comp = new CreateGridLine();
      comp.CreateAttributes();

      var line = new Line {
        FromX = 0,
        FromY = 0,
        FromZ = 0,
        ToX = 1,
        ToY = 1,
        ToZ = 0
      };
      ComponentTestHelper.SetInput(comp, line, 0);
      ComponentTestHelper.SetInput(comp, "Line", 1);

      return comp;
    }

    public static GH_OasysComponent GridArcComponentMother() {
      var comp = new CreateGridLine();
      comp.CreateAttributes();

      var arc = new Arc(new Point3d(0, 0, 0), new Point3d(1, 1, 0), new Point3d(2, 0, 0));
      ComponentTestHelper.SetInput(comp, arc, 0);
      ComponentTestHelper.SetInput(comp, "Arc", 1);

      return comp;
    }

    [Fact]
    public void CreateGridLineComponent() {
      GH_OasysComponent comp = GridLineComponentMother();

      var output = (GsaGridLineGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(output.Value.GridLine);
    }

    [Fact]
    public void CreateGridArcComponent() {
      GH_OasysComponent comp = GridArcComponentMother();

      var output = (GsaGridLineGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(output.Value.GridLine);
    }
  }
}
