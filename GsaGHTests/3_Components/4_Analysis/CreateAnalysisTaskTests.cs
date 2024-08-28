using System;

using Grasshopper.Kernel;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Analysis {
  [Collection("GrasshopperFixture collection")]
  public class CreateAnalysisTaskTests {
    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateAnalysisTask();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, 1, 0);
      ComponentTestHelper.SetInput(comp, "my Task", 1);
      var output = (GsaAnalysisCaseGoo)ComponentTestHelper.GetOutput(
        CreateAnalysisCaseTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, output, 2);

      return comp;
    }

    [Fact]
    public void CreateStaticComponentTest() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(1, output.Value.Id);
      Assert.Equal("my Task", output.Value.ApiTask.Name);
      Assert.Equal((int)AnalysisTaskType.Static, output.Value.ApiTask.Type);
      Assert.Equal("my Case", output.Value.Cases[0].Name);
      Assert.Equal("1.4L1 + 0.8L3", output.Value.Cases[0].Definition);
    }

    [Fact]
    public void CreateStaticPDeltaComponentTest1() {
      var comp = new CreateAnalysisTask();
      comp.CreateAttributes();

      comp.SetSelected(0, 1);
      ComponentTestHelper.SetInput(comp, 1, 0);
      ComponentTestHelper.SetInput(comp, "my Task", 1);

      var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(1, output.Value.Id);
      Assert.Equal("my Task", output.Value.ApiTask.Name);
      Assert.Equal((int)AnalysisTaskType.StaticPDelta, output.Value.ApiTask.Type);
      Assert.Equal(GH_RuntimeMessageLevel.Remark, comp.RuntimeMessageLevel);
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
    }

    [Fact]
    public void CreateStaticPDeltaComponentTest2() {
      var comp = new CreateAnalysisTask();
      comp.CreateAttributes();

      comp.SetSelected(0, 1);
      comp.SetSelected(1, 1);
      ComponentTestHelper.SetInput(comp, 1, 0);
      ComponentTestHelper.SetInput(comp, "my Task", 1);
      ComponentTestHelper.SetInput(comp, "1.2L1 + 1.2L2", 2);

      var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(1, output.Value.Id);
      Assert.Equal("my Task", output.Value.ApiTask.Name);
      Assert.Equal((int)AnalysisTaskType.StaticPDelta, output.Value.ApiTask.Type);
      Assert.Equal(GH_RuntimeMessageLevel.Remark, comp.RuntimeMessageLevel);
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
    }

    [Fact]
    public void CreateStaticPDeltaComponentTest3() {
      var comp = new CreateAnalysisTask();
      comp.CreateAttributes();

      comp.SetSelected(0, 1);
      comp.SetSelected(1, 2);
      ComponentTestHelper.SetInput(comp, 1, 0);
      ComponentTestHelper.SetInput(comp, "my Task", 1);
      ComponentTestHelper.SetInput(comp, 2, 2);

      var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(1, output.Value.Id);
      Assert.Equal("my Task", output.Value.ApiTask.Name);
      Assert.Equal((int)AnalysisTaskType.StaticPDelta, output.Value.ApiTask.Type);
      Assert.Equal(GH_RuntimeMessageLevel.Remark, comp.RuntimeMessageLevel);
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
    }

    //[Theory]
    //[InlineData(3)]
    //[InlineData(8)]
    //public void ChangeUnitExceptionsEquationTest(int analysisTaskType) {
    //  var comp = new CreateFaceLoad();
    //  comp.CreateAttributes();
    //  comp.SetSelected(0, 3); // Equation
    //  ComponentTestHelper.SetInput(comp, "All", 1);
    //  ComponentTestHelper.SetInput(comp, "myLoad", 2);
    //  ComponentTestHelper.SetInput(comp, "4*x+7*y-z", 7);
    //  comp.SetSelected(1, i);
    //  Assert.Throws<ArgumentOutOfRangeException>(() => ComponentTestHelper.GetOutput(comp));
    //  Assert.Equal(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error, comp.RuntimeMessageLevel);
    //}
  }
}
