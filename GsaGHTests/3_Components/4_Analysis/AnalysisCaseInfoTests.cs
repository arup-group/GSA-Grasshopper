using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Analysis {
  [Collection("GrasshopperFixture collection")]
  public class AnalysisCaseInfoTests {
    public static GH_OasysComponent ComponentMother() {
      var comp = new AnalysisCaseInfo();
      comp.CreateAttributes();
      var output = (GsaAnalysisCaseGoo)ComponentTestHelper.GetOutput(
        CreateAnalysisCaseTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, output);
      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysComponent comp = ComponentMother();

      var output1 = (GH_String)ComponentTestHelper.GetOutput(comp, 0);
      var output2 = (GH_String)ComponentTestHelper.GetOutput(comp, 1);

      Assert.Equal("my Case", output1.Value);
      Assert.Equal("1.4L1 + 0.8L3", output2.Value);
    }
  }
}
