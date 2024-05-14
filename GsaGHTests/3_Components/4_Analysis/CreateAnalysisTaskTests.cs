using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Components.Analysis {
  [Collection("GrasshopperFixture collection")]
  public class CreateAnalysisTaskTests {
    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateAnalysisTask();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, "my Task", 1);
      var output = (GsaAnalysisCaseGoo)ComponentTestHelper.GetOutput(
        CreateAnalysisCaseTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, output, 2);

      return comp;
    }

    [Fact]
    public void CreateStaticComponentTest() {
      GH_OasysComponent comp = ComponentMother();

      var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal("my Task", output.Value.ApiTask.Name);
      Assert.Equal((int)AnalysisTaskType.Static, output.Value.ApiTask.Type);
      Assert.Equal("my Case", output.Value.Cases[0].Name);
      Assert.Equal("1.4L1 + 0.8L3", output.Value.Cases[0].Definition);
    }

    [Fact]
    public void CreateStaticPDeltaComponentTest() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, "my Task", 0);

      var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal("my Task", output.Value.ApiTask.Name);
      Assert.Equal((int)AnalysisTaskType.Static, output.Value.ApiTask.Type);
      Assert.Equal("my Case", output.Value.Cases[0].Name);
      Assert.Equal("1.4L1 + 0.8L3", output.Value.Cases[0].Definition);
    }
  }
}
