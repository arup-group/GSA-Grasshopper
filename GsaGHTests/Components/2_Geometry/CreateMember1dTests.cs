using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Rhino.Geometry;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Components.Geometry
{
  [Collection("GrasshopperFixture collection")]
  public class CreateMember1dTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new CreateMember1d();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, new LineCurve(new Point3d(0, -1, 0), new Point3d(7, 3, 1)), 0);
      ComponentTestHelper.SetInput(comp, "STD CH(ft) 1 2 3 4", 1);
      ComponentTestHelper.SetInput(comp, 0.5, 2);

      return comp;
    }

    [Fact]
    public void CreateComponentTest()
    {
      // Arrange & Act
      var comp = ComponentMother();

      // Assert
      GsaMember1dGoo output = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, output.Value.PolyCurve.PointAtStart.X);
      Assert.Equal(-1, output.Value.PolyCurve.PointAtStart.Y);
      Assert.Equal(0, output.Value.PolyCurve.PointAtStart.Z);
      Assert.Equal(7, output.Value.PolyCurve.PointAtEnd.X);
      Assert.Equal(3, output.Value.PolyCurve.PointAtEnd.Y);
      Assert.Equal(1, output.Value.PolyCurve.PointAtEnd.Z);
      Assert.Equal("STD CH(ft) 1 2 3 4", output.Value.Section.Profile);
      Assert.Equal(0.5, output.Value.MeshSize);
    }
  }
}
