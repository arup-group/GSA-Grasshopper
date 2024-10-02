using System;

using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Data;
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
      var output = (GsaAnalysisCaseGoo)ComponentTestHelper.GetOutput(CreateAnalysisCaseTests.ComponentMother());
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

    [Theory]
    [InlineData("Z", 1, 1, 1, typeof(WalkingOnFloorAISC))]
    [InlineData("X", 2, 2, 2, typeof(WalkingOnFloorAISC2ndEdition))]
    [InlineData("Y", 3, 3, 3, typeof(WalkingOnFloorCCIP))]
    [InlineData("XY", 1, 4, 4, typeof(WalkingOnFloorSCI))]
    public void CreateFootfallSelfComponentTest1(
      string responseDirection, int weightingCurve, int excitationForces, int expectedResponsDirection,
      Type excitationForcesType) {
      var comp = new CreateAnalysisTask();
      comp.CreateAttributes();

      comp.SetSelected(0, 2);
      comp.SetSelected(1, 0);
      ComponentTestHelper.SetInput(comp, 1, 0);
      ComponentTestHelper.SetInput(comp, "my Task", 1);
      ComponentTestHelper.SetInput(comp, 2, 2);
      ComponentTestHelper.SetInput(comp, "All", 3);
      ComponentTestHelper.SetInput(comp, 100, 4);
      ComponentTestHelper.SetInput(comp, 76.5, 5);
      ComponentTestHelper.SetInput(comp, responseDirection, 6);
      ComponentTestHelper.SetInput(comp, weightingCurve, 7);
      ComponentTestHelper.SetInput(comp, excitationForces, 8);
      ComponentTestHelper.SetInput(comp, 2.2, 9);

      var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp);
      var parameter = new FootfallAnalysisTaskParameter(output.Value.ApiTask);

      Assert.Equal(1, output.Value.Id);
      Assert.Equal("my Task", output.Value.ApiTask.Name);
      Assert.Equal(0, (int)parameter.ExcitationMethod);
      Assert.Equal(2, parameter.ModalAnalysisTaskId);
      Assert.Equal("all", parameter.ResponseNodes);
      Assert.Equal(100, ((ConstantFootfallsForAllModes)parameter.NumberOfFootfalls).NumberOfFootfalls);
      Assert.Equal(76.5, parameter.WalkerMass);
      Assert.Equal(expectedResponsDirection, (int)parameter.ResponseDirection);
      Assert.Equal(weightingCurve * -1, (int)parameter.FrequencyWeightingCurve);
      Assert.Equal(excitationForcesType, parameter.ExcitationForces.GetType());
      Assert.Equal(2.2, ((ConstantDampingOption)parameter.DampingOption).ConstantDamping);
      Assert.Equal((int)AnalysisTaskType.Footfall, output.Value.ApiTask.Type);
      Assert.Equal(GH_RuntimeMessageLevel.Blank, comp.RuntimeMessageLevel);
    }

    [Theory]
    [InlineData(1, 2, 5, 1, typeof(WalkingOnStairAISC2ndEdition))]
    [InlineData(2, 3, 6, 2, typeof(WalkingOnStairArup))]
    [InlineData(3, 1, 7, 3, typeof(WalkingOnStairSCI))]
    [InlineData(4, 2, 8, 4, typeof(RunningOnFloorAISC2ndEdition))]
    public void CreateFootfallSelfComponentTest2(
      int responseDirection, int weightingCurve, int excitationForces, int expectedResponsDirection,
      Type excitationForcesType) {
      var comp = new CreateAnalysisTask();
      comp.CreateAttributes();

      comp.SetSelected(0, 2);
      comp.SetSelected(1, 0);
      ComponentTestHelper.SetInput(comp, 1, 0);
      ComponentTestHelper.SetInput(comp, "my Task", 1);
      ComponentTestHelper.SetInput(comp, 2, 2);
      ComponentTestHelper.SetInput(comp, "All", 3);
      ComponentTestHelper.SetInput(comp, 100, 4);
      ComponentTestHelper.SetInput(comp, 76.5, 5);
      ComponentTestHelper.SetInput(comp, responseDirection, 6);
      ComponentTestHelper.SetInput(comp, weightingCurve, 7);
      ComponentTestHelper.SetInput(comp, excitationForces, 8);
      ComponentTestHelper.SetInput(comp, 2.2, 9);

      var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp);
      var parameter = new FootfallAnalysisTaskParameter(output.Value.ApiTask);

      Assert.Equal(1, output.Value.Id);
      Assert.Equal("my Task", output.Value.ApiTask.Name);
      Assert.Equal(0, (int)parameter.ExcitationMethod);
      Assert.Equal(2, parameter.ModalAnalysisTaskId);
      Assert.Equal("all", parameter.ResponseNodes);
      Assert.Equal(100, ((ConstantFootfallsForAllModes)parameter.NumberOfFootfalls).NumberOfFootfalls);
      Assert.Equal(76.5, parameter.WalkerMass);
      Assert.Equal(expectedResponsDirection, (int)parameter.ResponseDirection);
      Assert.Equal(weightingCurve * -1, (int)parameter.FrequencyWeightingCurve);
      Assert.Equal(excitationForcesType, parameter.ExcitationForces.GetType());
      Assert.Equal(2.2, ((ConstantDampingOption)parameter.DampingOption).ConstantDamping);
      Assert.Equal((int)AnalysisTaskType.Footfall, output.Value.ApiTask.Type);
      Assert.Equal(GH_RuntimeMessageLevel.Blank, comp.RuntimeMessageLevel);
    }

    [Fact]
    public void CreateFootfallRigorousComponentTest() {
      var comp = new CreateAnalysisTask();
      comp.CreateAttributes();

      comp.SetSelected(0, 2);
      comp.SetSelected(1, 1);
      ComponentTestHelper.SetInput(comp, 1, 0);
      ComponentTestHelper.SetInput(comp, "my Task", 1);
      ComponentTestHelper.SetInput(comp, 2, 2);
      ComponentTestHelper.SetInput(comp, "All", 3);
      ComponentTestHelper.SetInput(comp, "All", 4);
      ComponentTestHelper.SetInput(comp, 100, 5);
      ComponentTestHelper.SetInput(comp, 76.5, 6);
      ComponentTestHelper.SetInput(comp, 1, 7);
      ComponentTestHelper.SetInput(comp, 2, 8);
      ComponentTestHelper.SetInput(comp, 3, 9);
      ComponentTestHelper.SetInput(comp, 2.2, 10);

      var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp);
      var parameter = new FootfallAnalysisTaskParameter(output.Value.ApiTask);

      Assert.Equal(1, output.Value.Id);
      Assert.Equal("my Task", output.Value.ApiTask.Name);
      Assert.Equal(1, (int)parameter.ExcitationMethod);
      Assert.Equal(2, parameter.ModalAnalysisTaskId);
      Assert.Equal("all", parameter.ResponseNodes);
      Assert.Equal("all", parameter.ExcitationNodes);
      Assert.Equal(100, ((ConstantFootfallsForAllModes)parameter.NumberOfFootfalls).NumberOfFootfalls);
      Assert.Equal(76.5, parameter.WalkerMass);
      Assert.Equal(1, (int)parameter.ResponseDirection);
      Assert.Equal(-2, (int)parameter.FrequencyWeightingCurve);
      Assert.Equal(typeof(WalkingOnFloorCCIP), parameter.ExcitationForces.GetType());
      Assert.Equal(2.2, ((ConstantDampingOption)parameter.DampingOption).ConstantDamping);
      Assert.Equal((int)AnalysisTaskType.Footfall, output.Value.ApiTask.Type);
      Assert.Equal(GH_RuntimeMessageLevel.Blank, comp.RuntimeMessageLevel);
    }

    [Fact]
    public void CreateFootfallFastComponentTest() {
      var comp = new CreateAnalysisTask();
      comp.CreateAttributes();

      comp.SetSelected(0, 2);
      comp.SetSelected(1, 3);
      ComponentTestHelper.SetInput(comp, 1, 0);
      ComponentTestHelper.SetInput(comp, "my Task", 1);
      ComponentTestHelper.SetInput(comp, 2, 2);
      ComponentTestHelper.SetInput(comp, "All", 3);
      ComponentTestHelper.SetInput(comp, "All", 4);
      ComponentTestHelper.SetInput(comp, 100, 5);
      ComponentTestHelper.SetInput(comp, 76.5, 6);
      ComponentTestHelper.SetInput(comp, 1, 7);
      ComponentTestHelper.SetInput(comp, 2, 8);
      ComponentTestHelper.SetInput(comp, 3, 9);
      ComponentTestHelper.SetInput(comp, 2.2, 10);

      var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp);
      var parameter = new FootfallAnalysisTaskParameter(output.Value.ApiTask);

      Assert.Equal(1, output.Value.Id);
      Assert.Equal("my Task", output.Value.ApiTask.Name);
      Assert.Equal(2, (int)parameter.ExcitationMethod);
      Assert.Equal(2, parameter.ModalAnalysisTaskId);
      Assert.Equal("all", parameter.ResponseNodes);
      Assert.Equal("all", parameter.ExcitationNodes);
      Assert.Equal(100, ((ConstantFootfallsForAllModes)parameter.NumberOfFootfalls).NumberOfFootfalls);
      Assert.Equal(76.5, parameter.WalkerMass);
      Assert.Equal(1, (int)parameter.ResponseDirection);
      Assert.Equal(-2, (int)parameter.FrequencyWeightingCurve);
      Assert.Equal(typeof(WalkingOnFloorCCIP), parameter.ExcitationForces.GetType());
      Assert.Equal(2.2, ((ConstantDampingOption)parameter.DampingOption).ConstantDamping);
      Assert.Equal((int)AnalysisTaskType.Footfall, output.Value.ApiTask.Type);
      Assert.Equal(GH_RuntimeMessageLevel.Blank, comp.RuntimeMessageLevel);
    }

    [Fact]
    public void CreateFootfallFastRigorousComponentTest() {
      var comp = new CreateAnalysisTask();
      comp.CreateAttributes();

      comp.SetSelected(0, 2);
      comp.SetSelected(1, 2);
      ComponentTestHelper.SetInput(comp, 1, 0);
      ComponentTestHelper.SetInput(comp, "my Task", 1);
      ComponentTestHelper.SetInput(comp, 2, 2);
      ComponentTestHelper.SetInput(comp, "All", 3);
      ComponentTestHelper.SetInput(comp, "All", 4);
      ComponentTestHelper.SetInput(comp, 100, 5);
      ComponentTestHelper.SetInput(comp, 76.5, 6);
      ComponentTestHelper.SetInput(comp, 1, 7);
      ComponentTestHelper.SetInput(comp, 2, 8);
      ComponentTestHelper.SetInput(comp, 3, 9);
      ComponentTestHelper.SetInput(comp, 2.2, 10);

      var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp);
      var parameter = new FootfallAnalysisTaskParameter(output.Value.ApiTask);

      Assert.Equal(1, output.Value.Id);
      Assert.Equal("my Task", output.Value.ApiTask.Name);
      Assert.Equal(5, (int)parameter.ExcitationMethod);
      Assert.Equal(2, parameter.ModalAnalysisTaskId);
      Assert.Equal("all", parameter.ResponseNodes);
      Assert.Equal("all", parameter.ExcitationNodes);
      Assert.Equal(100, ((ConstantFootfallsForAllModes)parameter.NumberOfFootfalls).NumberOfFootfalls);
      Assert.Equal(76.5, parameter.WalkerMass);
      Assert.Equal(1, (int)parameter.ResponseDirection);
      Assert.Equal(-2, (int)parameter.FrequencyWeightingCurve);
      Assert.Equal(typeof(WalkingOnFloorCCIP), parameter.ExcitationForces.GetType());
      Assert.Equal(2.2, ((ConstantDampingOption)parameter.DampingOption).ConstantDamping);
      Assert.Equal((int)AnalysisTaskType.Footfall, output.Value.ApiTask.Type);
      Assert.Equal(GH_RuntimeMessageLevel.Blank, comp.RuntimeMessageLevel);
    }

    [Fact]
    public void FootfallInputWithExcitationTrueShouldIncludeExcitationNode() {
      var footfallInputManager = new FootfallInputManager(true);
      Assert.Contains(FootfallInputManager._excitationNodesAttributes, footfallInputManager.GetInputs());
    }

    [Fact]
    public void FootfallInputWithExcitationFalseShouldNotIncludeExcitationNode() {
      var footfallInputManager = new FootfallInputManager(false);
      Assert.DoesNotContain(FootfallInputManager._excitationNodesAttributes, footfallInputManager.GetInputs());
    }
  }

}
