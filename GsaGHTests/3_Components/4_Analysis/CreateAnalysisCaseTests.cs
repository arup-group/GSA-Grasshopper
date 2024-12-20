using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Analysis {
  [Collection("GrasshopperFixture collection")]
  public class CreateAnalysisCaseTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateAnalysisCase();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, "my Case", 0);
      ComponentTestHelper.SetInput(comp, "1.4L1 + 0.8L3", 1);

      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysComponent comp = ComponentMother();

      var output = (GsaAnalysisCaseGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal("my Case", output.Value.ApiCase.Name);
      Assert.Equal("1.4L1 + 0.8L3", output.Value.ApiCase.Description);
    }
  }
}
