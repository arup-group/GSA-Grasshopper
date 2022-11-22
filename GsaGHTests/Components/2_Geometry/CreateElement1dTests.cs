using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Rhino.Geometry;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Components.Properties
{
  [Collection("GrasshopperFixture collection")]
  public class CreateElement1dTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new CreateElement1d();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, new Line(new Point3d(0, -1, 0), new Point3d(7, 3, 1)), 0);


      return comp;
    }

    [Fact]
    public void CreateComponentTest()
    {
      var comp = ComponentMother();

      GsaMaterialGoo output = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, output.Value.AnalysisProperty);
      Assert.Equal(1, output.Value.GradeProperty);
      Assert.Equal(MatType.TIMBER, output.Value.MaterialType);
    }
  }
}
