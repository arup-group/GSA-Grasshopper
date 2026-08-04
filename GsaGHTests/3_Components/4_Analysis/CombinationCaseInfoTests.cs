using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Analysis {
  [Collection("GrasshopperFixture collection")]
  public class CombinationCaseInfoTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new CombinationCaseInfo();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp,
        (GsaCombinationCaseGoo)ComponentTestHelper.GetOutput(
          CreateCombinationCaseTests.ComponentMother()));

      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysComponent comp = ComponentMother();

      var output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 0);
      var output2 = (GH_String)ComponentTestHelper.GetOutput(comp, 1);
      var output3 = (GH_String)ComponentTestHelper.GetOutput(comp, 2);

      Assert.Equal(42, output1.Value);
      Assert.Equal("my Combination", output2.Value);
      Assert.Equal("1.35A1 + 1.5A2", output3.Value);
    }
  }
}
