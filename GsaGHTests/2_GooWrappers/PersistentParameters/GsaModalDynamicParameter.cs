
using System.Linq;

using Grasshopper.Kernel;

using GsaGH;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;

using GsaGHTests.Components.Analysis;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaModalDynamicParameterTests {

    [Fact]
    public void GsaModalDynamicParameterPreferredCastTest() {
      GH_OasysComponent comp = CreateAnalysisTaskTests.CreateAnalysisTaskComponent(ModeCalculationMethod.FrquencyRange);
      object output = ComponentTestHelper.GetOutput(comp);
      var param = new GsaModalDynamicParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, output);
      Assert.NotNull(param.VolatileData.AllData(false).First());
      Assert.Contains("FrquencyRange", ComponentTestHelper.GetOutput(param).ToString());
    }

    [Fact]
    public void ToStringTest() {
      GH_OasysComponent comp = CreateAnalysisTaskTests.CreateAnalysisTaskComponent(ModeCalculationMethod.FrquencyRange);
      object output = ComponentTestHelper.GetOutput(comp);
      var param = new GsaModalDynamicParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, output);
      Assert.NotNull(param.VolatileData.AllData(false).First());
    }

    [Fact]
    public void GsaModalDynamicParameterPreferredCastErrorTest() {
      int i = 0;
      var param = new GsaModalDynamicParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, i);
      Assert.False(param.VolatileData.AllData(false).First().IsValid);
      Assert.Single(param.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void ParameterPluginInfoValueShouldBeValid() {
      GH_OasysComponent comp = CreateModalDynamicParameterByFrquencyRangeTest.ComponentMother();
      var output = (GsaModalDynamicGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(PluginInfo.Instance, output.PluginInfo);
    }

  }
}
