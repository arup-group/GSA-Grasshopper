using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Components.Loads {
  [Collection("GrasshopperFixture collection")]
  public class CreateGridAreaLoadTests {
    [Fact]
    public void CreateGridAreaLoadTest() {
      var comp = new CreateGridAreaLoad();
      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, "myGridAreaLoad", 6);
      ComponentTestHelper.SetInput(comp, -5, 7);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaGridAreaLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal("myGridAreaLoad", load.ApiLoad.Name);
      Assert.Equal(-5000, load.ApiLoad.Value);
      Assert.Equal(ReferenceType.None, load.ReferenceType);
    }

    [Theory]
    [InlineData("X")]
    [InlineData("Y")]
    [InlineData("Z")]
    public void DirectionTest(string direction) {
      var comp = new CreateGridAreaLoad();
      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, direction, 3);
      ComponentTestHelper.SetInput(comp, -5, 7);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaGridAreaLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal(direction, load.ApiLoad.Direction.ToString());
      Assert.Equal(-5000, load.ApiLoad.Value);
      Assert.Equal(ReferenceType.None, load.ReferenceType);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ProjectedTest(bool project) {
      var comp = new CreateGridAreaLoad();
      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, project, 5);
      ComponentTestHelper.SetInput(comp, -5, 7);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaGridAreaLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal(project, load.ApiLoad.IsProjected);
      Assert.Equal(-5000, load.ApiLoad.Value);
      Assert.Equal(ReferenceType.None, load.ReferenceType);
    }
  }
}
