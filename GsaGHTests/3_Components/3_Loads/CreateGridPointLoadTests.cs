using GsaGH.Components;

using GsaGH.Parameters;

using GsaGH.Parameters.Enums;


using GsaGHTests.Helpers;


using Rhino.Geometry;


using GsaGH.Helpers;

using Xunit;
namespace GsaGHTests.Components.Loads {
  [Collection("GrasshopperFixture collection")]
  public class CreateGridPointLoadTests {
    [Fact]
    public void CreateGridPointLoadTest() {
      var comp = new CreateGridPointLoad();
      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, new Point3d(10, 5, -1), 1);
      ComponentTestHelper.SetInput(comp, "myGridPointLoad", 5);
      ComponentTestHelper.SetInput(comp, -5, 6);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaGridPointLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal(10, load.ApiLoad.X, DoubleComparer.Default);
      Assert.Equal(5, load.ApiLoad.Y, DoubleComparer.Default);
      Assert.Equal("myGridPointLoad", load.ApiLoad.Name);
      Assert.Equal(-5000, load.ApiLoad.Value);
      Assert.Equal(ReferenceType.None, load.ReferenceType);
    }


  }
}
