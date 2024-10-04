using System;
using System.Collections.Generic;

using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Data;
using GsaGH.Helpers;
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

    private static Type ExcitationForcesType(int excitationForceOption) {
      switch (excitationForceOption) {
        case 1: return typeof(WalkingOnFloorAISC);
        case 2: return typeof(WalkingOnFloorAISC2ndEdition);
        case 3: return typeof(WalkingOnFloorCCIP);
        case 4: return typeof(WalkingOnFloorSCI);
        case 5: return typeof(WalkingOnStairAISC2ndEdition);
        case 6: return typeof(WalkingOnStairArup);
        case 7: return typeof(WalkingOnStairSCI);
        case 8: return typeof(RunningOnFloorAISC2ndEdition);
        default: throw new ArgumentException("Not correct option");
      }
    }

    private static string ResponseDirection(int direction) {
      switch (direction) {
        case 1: return "Z";
        case 2: return "X";
        case 3: return "Y";
        case 4: return "XY";
        default: throw new ArgumentException("Not correct option");
      }
    }

    private static ExcitationMethod ExcitationOption(int selectedIndex) {
      switch (selectedIndex) {
        case 0: return ExcitationMethod.SelfExcitation;
        case 1: return ExcitationMethod.FullExcitationRigorous;
        case 2: return ExcitationMethod.FullExcitationFastExcludingResponseNode;
        case 3: return ExcitationMethod.FullExcitationFast;
        default: throw new ArgumentException("Not correct option");
      }
    }

    [Fact]
    public void CreateFootfallSelfComponentTest() {
      int excitationSelectedIndex = 0;
      foreach (int excitation in Enum.GetValues(typeof(ExcitationMethod))) {
        foreach (int direction in Enum.GetValues(typeof(ResponseDirection))) {
          for (int directionOption = 0; directionOption < 2; directionOption++) {
            foreach (int weighingOption in Enum.GetValues(typeof(WeightingOption))) {
              for (int excitationForce = 1; excitationForce < 9; excitationForce++) {
                var comp = new CreateAnalysisTask();
                comp.CreateAttributes();
                comp.SetSelected(0, 2);
                comp.SetSelected(1, excitationSelectedIndex);
                ComponentTestHelper.SetInput(comp, 1, 0);
                ComponentTestHelper.SetInput(comp, "my Task", 1);
                ComponentTestHelper.SetInput(comp, 2, 2);
                ComponentTestHelper.SetInput(comp, "All", 3);
                int index = 4;
                if (excitation > 0) {
                  ComponentTestHelper.SetInput(comp, "1 2", 4);
                  index++;
                }

                ComponentTestHelper.SetInput(comp, 100, index++);
                ComponentTestHelper.SetInput(comp, 76.5, index++);
                if (directionOption > 0) {
                  ComponentTestHelper.SetInput(comp, ResponseDirection(direction), index++);
                } else {
                  ComponentTestHelper.SetInput(comp, direction, index++);
                }

                ComponentTestHelper.SetInput(comp, weighingOption * -1, index++);
                ComponentTestHelper.SetInput(comp, excitationForce, index++);
                ComponentTestHelper.SetInput(comp, 2.2, index++);

                var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(comp);
                var parameter = new FootfallAnalysisTaskParameter(output.Value.ApiTask);

                Assert.Equal(ExcitationOption(excitationSelectedIndex), parameter.ExcitationMethod);
                Assert.Equal(1, output.Value.Id);
                Assert.Equal("my Task", output.Value.ApiTask.Name);
                Assert.Equal(2, parameter.ModalAnalysisTaskId);
                Assert.Equal("all", parameter.ResponseNodes);
                if (excitation > 1) {
                  Assert.Equal("1 2", parameter.ExcitationNodes);
                }

                Assert.Equal(100, ((ConstantFootfallsForAllModes)parameter.NumberOfFootfalls).NumberOfFootfalls);
                Assert.Equal(76.5, parameter.WalkerMass);
                Assert.Equal(direction, (int)parameter.ResponseDirection);
                Assert.Equal(weighingOption, (int)parameter.FrequencyWeightingCurve);
                Assert.Equal(ExcitationForcesType(excitationForce), parameter.ExcitationForces.GetType());
                Assert.Equal(2.2, ((ConstantDampingOption)parameter.DampingOption).ConstantDamping);
                Assert.Equal((int)AnalysisTaskType.Footfall, output.Value.ApiTask.Type);
                Assert.Equal(GH_RuntimeMessageLevel.Blank, comp.RuntimeMessageLevel);
              }
            }
          }
        }

        excitationSelectedIndex++;
      }
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
      Assert.Contains(FootfallInputManager._excitationNodesAttributes,
        footfallInputManager.GetInputsForSelfExcitation(true));
    }

    [Fact]
    public void FootfallInputWithExcitationFalseShouldNotIncludeExcitationNode() {
      var footfallInputManager = new FootfallInputManager(false);
      Assert.DoesNotContain(FootfallInputManager._excitationNodesAttributes,
        footfallInputManager.GetInputsForSelfExcitation(false));
    }

    [Fact]
    public void SearchingForValuesInDictionaryShouldReturnExceptionForFailedSearch() {
      var dictionary = new Dictionary<string, string> {
        { "key1", "value1" },
        { "key2", "value2" }
      };
      Assert.Throws<Exception>(() => dictionary.TryGetKeyFrom("value3"));
    }

    [Fact]
    public void SearchingForExistingValuesShouldReturnTheKey() {
      var dictionary = new Dictionary<string, string> {
        { "key1", "value1" },
        { "key2", "value2" }
      };
      Assert.Equal("key2", dictionary.TryGetKeyFrom("value2"));
    }

  }
}
