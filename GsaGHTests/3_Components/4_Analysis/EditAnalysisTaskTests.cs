﻿using Grasshopper.Kernel.Types;
using GsaAPI;
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

      Assert.Equal("my Task", output.Value.ApiTask.Name);
      Assert.Equal((int)AnalysisTaskType.Static, output.Value.ApiTask.Type);
      Assert.Equal("my Case", output.Value.Cases[0].Name);
      Assert.Equal("1.4L1 + 0.8L3", output.Value.Cases[0].Definition);
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
      var comp = new EditAnalysisTask();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, taskGoo);

      // Assert
      var output0 = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp); // task
      var output1 = (GH_String)ComponentTestHelper.GetOutput(comp, 1); // name
      var output2 = (GsaAnalysisCaseGoo)ComponentTestHelper.GetOutput(comp, 2); // cases
      var output3 = (GH_String)ComponentTestHelper.GetOutput(comp, 3); // type
      var output4 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4); // task id

      Duplicates.AreEqual(taskGoo.Value, output0.Value);
      Assert.Equal("Task 1", output1.Value);
      Assert.Equal(1, output2.Value.Id);
      Assert.Equal("L1", output2.Value.Definition);
      Assert.Equal("DL", output2.Value.Name);
      Assert.Equal("Static", output3.Value);
      Assert.Equal(1, output4.Value);
    }
  }
}
