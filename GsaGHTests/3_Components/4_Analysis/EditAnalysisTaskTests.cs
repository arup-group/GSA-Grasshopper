using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace GsaGHTests.Components.Analysis {
  [Collection("GrasshopperFixture collection")]
  public class EditAnalysisTaskTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new EditAnalysisTask();
      comp.CreateAttributes();

      var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(
        CreateAnalysisTaskTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, output);

      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysComponent comp = ComponentMother();

      var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal("my Task", output.Value.Name);
      Assert.Equal(AnalysisTaskType.Static, output.Value.Type);
      Assert.Equal("my Case", output.Value.Cases[0].Name);
      Assert.Equal("1.4L1 + 0.8L3", output.Value.Cases[0].Definition);
    }

    [Fact]
    public void GetTaskInfoFromModelTest() {
      var getModelAnalysis = new GetModelAnalysis();
      var model = new GsaModel();
      model.ApiModel.Open(GsaFile.SteelDesignSimple);
      ComponentTestHelper.SetInput(getModelAnalysis, new GsaModelGoo(model));
      var taskGoo = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(getModelAnalysis);

      var comp = new EditAnalysisTask();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, taskGoo);

      var output1 = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp);
      var output2 = (GH_String)ComponentTestHelper.GetOutput(comp, 1);
      var output3 = (GsaAnalysisCaseGoo)ComponentTestHelper.GetOutput(comp, 2);
      var output4 = (GH_String)ComponentTestHelper.GetOutput(comp, 3);
      var output5 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);

      Duplicates.AreEqual(taskGoo.Value, output1.Value);
      Assert.Equal("Task 1", output2.Value);
      Assert.Equal(1, output3.Value.Id);
      Assert.Equal("L1", output3.Value.Definition);
      Assert.Equal("DL", output3.Value.Name);
      Assert.Equal("Static", output4.Value);
      Assert.Equal(1, output5.Value);
    }
  }
}
