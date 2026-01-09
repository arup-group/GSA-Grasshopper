using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Loads {
  [Collection("GrasshopperFixture collection")]
  public class LoadCaseInfoTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new LoadCaseInfo();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp,
        (GsaLoadCaseGoo)ComponentTestHelper.GetOutput(CreateLoadCaseTests.ComponentMother()));

      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysComponent comp = ComponentMother();

      var output0 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 0);
      var output1 = (GH_String)ComponentTestHelper.GetOutput(comp, 1);
      var output2 = (GH_String)ComponentTestHelper.GetOutput(comp, 2);

      Assert.Equal(5, output0.Value);
      Assert.Equal("Live load", output1.Value);
      Assert.Equal("Live", output2.Value);
    }
  }
}
