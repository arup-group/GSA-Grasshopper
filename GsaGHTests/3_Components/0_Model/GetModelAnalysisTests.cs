using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class GetModelAnalysisTests {
    [Fact]
    public static void GetModelAnalysisTest() {
      // Assemble
      var comp = new GetModelAnalysis();
      var model = new GsaModel();
      model.ApiModel.Open(GsaFile.SteelDesignSimple);

      // Act
      ComponentTestHelper.SetInput(comp, new GsaModelGoo(model));
      var taskGoo = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp);
      var caseGoo = (GsaAnalysisCaseGoo)ComponentTestHelper.GetOutput(comp, 1);
      var combGoo = (GsaCombinationCaseGoo)ComponentTestHelper.GetOutput(comp, 2);

      // Assert
      Assert.NotNull(taskGoo);
      Assert.Equal(1, taskGoo.Value.Id);
      Assert.Equal(2, taskGoo.Value.Cases.Count);
      Assert.Equal("Task 1", taskGoo.Value.ApiTask.Name);
      Assert.Equal((int)AnalysisTaskType.Static, taskGoo.Value.ApiTask.Type);

      Assert.NotNull(caseGoo);
      Assert.Equal(1, caseGoo.Value.Id);
      Assert.Equal("L1", caseGoo.Value.ApiCase.Description);
      Assert.Equal("DL", caseGoo.Value.ApiCase.Name);

      Assert.NotNull(combGoo);
      Assert.Equal(1, combGoo.Value.Id);
      Assert.Equal("1.4A1 + 1.6A2", combGoo.Value.Definition);
      Assert.Equal("ULS", combGoo.Value.Name);
    }
  }
}
