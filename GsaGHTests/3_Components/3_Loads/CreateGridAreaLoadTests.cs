using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Components.Loads {
  [Collection("GrasshopperFixture collection")]
  public class CreateGridAreaLoadTests {
    [Fact]
    public void CreateLoadTest() {
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
  }
}
