using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Analysis {
  [Collection("GrasshopperFixture collection")]
  public class AnalysisTaskInfoTests {
    public static GH_OasysComponent ComponentMother() {
      var comp = new AnalysisTaskInfo();
      comp.CreateAttributes();

      var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(
        CreateAnalysisTaskTests.CreateAnalysisTaskComponent());
      ComponentTestHelper.SetInput(comp, output);

      return comp;
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysComponent comp = ComponentMother();

      var output0 = (GH_String)ComponentTestHelper.GetOutput(comp, 0);
      var output1 = (GsaAnalysisCaseGoo)ComponentTestHelper.GetOutput(comp, 1);
      var output2 = (GH_String)ComponentTestHelper.GetOutput(comp, 2);
      var output3 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 3);

      Assert.Equal("my Task", output0.Value);
      Assert.Equal(0, output1.Value.Id);
      Assert.Equal("1.4L1 + 0.8L3", output1.Value.Definition);
      Assert.Equal("my Case", output1.Value.Name);
      Assert.Equal("Static", output2.Value);
      Assert.Equal(1, output3.Value);
    }

    [Fact]
    public void GetTaskInfoFromModelTest() {
      // Assemble
      var getModelAnalysis = new GetModelAnalysis();
      var model = new GsaModel();
      model.ApiModel.Open(GsaFile.SteelDesignSimple);
      ComponentTestHelper.SetInput(getModelAnalysis, new GsaModelGoo(model));
      var taskGoo = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(getModelAnalysis);

      // Act
      var comp = new AnalysisTaskInfo();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, taskGoo);

      // Assert
      var output0 = (GH_String)ComponentTestHelper.GetOutput(comp, 0); // name
      var output1 = (GsaAnalysisCaseGoo)ComponentTestHelper.GetOutput(comp, 1); // cases
      var output2 = (GH_String)ComponentTestHelper.GetOutput(comp, 2); // type
      var output3 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 3); // task id

      Assert.Equal("Task 1", output0.Value);
      Assert.Equal(1, output1.Value.Id);
      Assert.Equal("L1", output1.Value.Definition);
      Assert.Equal("DL", output1.Value.Name);
      Assert.Equal("Static", output2.Value);
      Assert.Equal(1, output3.Value);
    }
  }
}
