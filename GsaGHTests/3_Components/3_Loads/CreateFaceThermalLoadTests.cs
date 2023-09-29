using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Components.Loads {
  [Collection("GrasshopperFixture collection")]
  public class CreateFaceThermalLoadTests {
    [Fact]
    public void CreateUniformTest() {
      var comp = new CreateFaceThermalLoad();
      ComponentTestHelper.SetInput(comp, 7, 0);
      ComponentTestHelper.SetInput(comp, "All", 1);
      ComponentTestHelper.SetInput(comp, "myThermalLoad", 2);
      ComponentTestHelper.SetInput(comp, -5, 3);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (GsaFaceThermalLoad)output.Value;
      Assert.Equal(7, load.LoadCase.Id);
      Assert.Equal("myThermalLoad", load.ApiLoad.Name);
      Assert.Equal(-5, load.ApiLoad.UniformTemperature);
      Assert.Equal(ReferenceType.None, load.ReferenceType);
    }

    [Fact]
    public void EntityListTypeErrorTest() {
      var comp = new CreateFaceThermalLoad();
      var list = new GsaList("test", "1 2 3", GsaAPI.EntityType.Node);
      ComponentTestHelper.SetInput(comp, new GsaListGoo(list), 1);
      ComponentTestHelper.SetInput(comp, -5, 3);

      var output = (GsaLoadGoo)ComponentTestHelper.GetOutput(comp);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }
  }
}
