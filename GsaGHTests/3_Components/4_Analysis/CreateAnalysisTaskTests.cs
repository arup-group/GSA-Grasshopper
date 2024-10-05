﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Data;
using GsaGH.Helpers;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Components.Analysis {
  [Collection("GrasshopperFixture collection")]
  public class CreateAnalysisTaskTests {
    private readonly CreateAnalysisTask _component;

    public static CreateAnalysisTask CreateAnalysisTaskComponent() {
      var component = new CreateAnalysisTask();
      component.CreateAttributes();

      // Set minimum inputs
      ComponentTestHelper.SetInput(component, 1, 0);
      ComponentTestHelper.SetInput(component, "my Task", 1);
      ComponentTestHelper.SetInput(component, GetDummyAnalysisCase(), 2);

      return component;
    }

    private static GsaAnalysisCaseGoo GetDummyAnalysisCase() {
      return (GsaAnalysisCaseGoo)ComponentTestHelper.GetOutput(CreateAnalysisCaseTests.ComponentMother());
    }

    public CreateAnalysisTaskTests() { _component = CreateAnalysisTaskComponent(); }

    [Fact]
    public void CreateStaticComponentTest() {
      var output = (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(_component);

      Assert.Equal(1, output.Value.Id);
      Assert.Equal("my Task", output.Value.ApiTask.Name);
      Assert.Equal((int)AnalysisTaskType.Static, output.Value.ApiTask.Type);
      Assert.Equal("my Case", output.Value.Cases[0].Name);
      Assert.Equal("1.4L1 + 0.8L3", output.Value.Cases[0].Definition);
    }

    [Fact]
    public void ShouldAddWarningForGhWrapperWithNullValue() {
      SetToStatic();
      ComponentTestHelper.SetListInput(_component, GetInvalidList(), 2);
      _ = ComputeAndGetOutput();
      Assert.True(_component.RuntimeMessages(GH_RuntimeMessageLevel.Warning).Count > 0);
    }

    private static List<object> GetInvalidList() {
      return new List<object>() { new GH_ObjectWrapper() { Value = null } };
    }

    [Fact]
    public void ShouldAddRemarkForMissingAnalysisCase() {
      SetToStatic();
      _ = ComputeAndGetOutput();
      Assert.True(_component.RuntimeMessages(GH_RuntimeMessageLevel.Remark).Count > 0);
      Assert.Single(_component.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
    }

    [Fact]
    public void ShouldAddErrorForInvalidDirection() {
      SetFootfall();
      ComponentTestHelper.SetInput(_component, 2, 2);
      ComponentTestHelper.SetInput(_component, GetInvalidList(),
        FootfallInputManager._responseDirectionAttributes.Name);
      ComponentTestHelper.ComputeOutput(_component);
      AssertInvalidDirectionError(_component);
    }

    private static void AssertInvalidDirectionError(CreateAnalysisTask component) {
      IList<string> runtimeMessages = component.RuntimeMessages(GH_RuntimeMessageLevel.Error);
      Assert.True(runtimeMessages.Contains("Unable to convert response direction input"));
    }

    [Fact]
    public void ShouldAddErrorForInvalidStringDirection() {
      SetFootfall();
      ComponentTestHelper.SetInput(_component, 2, 2);
      ComponentTestHelper.SetInput(_component, new List<string>() { "AnInvalidString" },
        FootfallInputManager._responseDirectionAttributes.Name);
      ComponentTestHelper.ComputeOutput(_component);
      AssertInvalidDirectionError(_component);
    }

    [Fact]
    public void ShouldAddErrorForInvalidIntgerDirection() {
      SetFootfall();
      ComponentTestHelper.SetInput(_component, 2, 2);
      var anInvalidString = 5;
      ComponentTestHelper.SetInput(_component, new List<int>() { anInvalidString },
        FootfallInputManager._responseDirectionAttributes.Name);
      ComponentTestHelper.ComputeOutput(_component);
      IList<string> runtimeMessages = _component.RuntimeMessages(GH_RuntimeMessageLevel.Error);
      AssertInvalidDirectionError(_component);
    }

    private GsaAnalysisTaskGoo ComputeAndGetOutput() {
      return (GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(_component);
    }

    private void ComputeOutput() {
      ComponentTestHelper.ComputeOutput(_component);
    }

    [Fact]
    public void CreateStaticPDeltaComponentTest1() {
      SetToStaticPDelta();

      var output = ComputeAndGetOutput();

      Assert.Equal(1, output.Value.Id);
      Assert.Equal("my Task", output.Value.ApiTask.Name);
      Assert.Equal((int)AnalysisTaskType.StaticPDelta, output.Value.ApiTask.Type);
    }

    private void SetToStatic() {
      _component.SetSelected(0, 0);
    }

    private void SetToStaticPDelta() {
      _component.SetSelected(0, 1);
    }

    private void SetFootfall() {
      _component.SetSelected(0, 2);
    }

    private void SetExcitationRigorous() {
      _component.SetSelected(1, 1);
    }

    private void SetExcitationToFastAndRigorous() {
      _component.SetSelected(1, 2);
    }

    [Fact]
    public void CreateStaticPDeltaComponentTest2() {
      SetToStaticPDelta();
      SetExcitationRigorous();
      ComponentTestHelper.SetInput(_component, "1.2L1 + 1.2L2", 2);

      var output = ComputeAndGetOutput();

      Assert.Equal((int)AnalysisTaskType.StaticPDelta, output.Value.ApiTask.Type);
    }

    [Fact]
    public void CreateStaticPDeltaComponentTest3() {
      SetToStaticPDelta();
      SetExcitationToFastAndRigorous();
      ComponentTestHelper.SetInput(_component, 2, 2);

      var output = ComputeAndGetOutput();

      Assert.Equal((int)AnalysisTaskType.StaticPDelta, output.Value.ApiTask.Type);
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
                _component.CreateAttributes();
                SetFootfall();
                _component.SetSelected(1, excitationSelectedIndex);
                ComponentTestHelper.SetInput(_component, 2, 2);
                ComponentTestHelper.SetInput(_component, "All", 3);
                int index = 4;
                if (excitation > 0) {
                  ComponentTestHelper.SetInput(_component, "1 2", 4);
                  index++;
                }

                ComponentTestHelper.SetInput(_component, 100, index++);
                ComponentTestHelper.SetInput(_component, 76.5, index++);
                if (directionOption > 0) {
                  ComponentTestHelper.SetInput(_component, ResponseDirection(direction), index++);
                } else {
                  ComponentTestHelper.SetInput(_component, direction, index++);
                }

                ComponentTestHelper.SetInput(_component, weighingOption * -1, index++);
                ComponentTestHelper.SetInput(_component, excitationForce, index++);
                ComponentTestHelper.SetInput(_component, 2.2, index++);

                var output = ComputeAndGetOutput();
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
                Assert.Equal(GH_RuntimeMessageLevel.Blank, _component.RuntimeMessageLevel);
              }
            }
          }
        }

        excitationSelectedIndex++;
      }
    }

    [Fact]
    public void CreateFootfallRigorousComponentTest() {
      SetFootfall();
      SetExcitationRigorous();
      ComponentTestHelper.SetInput(_component, 2, 2);
      ComponentTestHelper.SetInput(_component, "All", 3);
      ComponentTestHelper.SetInput(_component, "All", 4);
      ComponentTestHelper.SetInput(_component, 100, 5);
      ComponentTestHelper.SetInput(_component, 76.5, 6);
      ComponentTestHelper.SetInput(_component, 1, 7);
      ComponentTestHelper.SetInput(_component, 2, 8);
      ComponentTestHelper.SetInput(_component, 3, 9);
      ComponentTestHelper.SetInput(_component, 2.2, 10);

      var output = ComputeAndGetOutput();
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
      Assert.Equal(GH_RuntimeMessageLevel.Blank, _component.RuntimeMessageLevel);
    }

    [Fact]
    public void CreateFootfallFastComponentTest() {
      SetFootfall();
      SetExcitationFast();
      ComponentTestHelper.SetInput(_component, 2, 2);
      ComponentTestHelper.SetInput(_component, "All", 3);
      ComponentTestHelper.SetInput(_component, "All", 4);
      ComponentTestHelper.SetInput(_component, 100, 5);
      ComponentTestHelper.SetInput(_component, 76.5, 6);
      ComponentTestHelper.SetInput(_component, 1, 7);
      ComponentTestHelper.SetInput(_component, 2, 8);
      ComponentTestHelper.SetInput(_component, 3, 9);
      ComponentTestHelper.SetInput(_component, 2.2, 10);

      var output = ComputeAndGetOutput();
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
      Assert.Equal(GH_RuntimeMessageLevel.Blank, _component.RuntimeMessageLevel);
    }

    private void SetExcitationFast() {
      _component.SetSelected(1, 3);
    }

    [Fact]
    public void CreateFootfallFastRigorousComponentTest() {
      SetFootfall();
      _component.SetSelected(1, 2);
      ComponentTestHelper.SetInput(_component, 2, 2);
      ComponentTestHelper.SetInput(_component, "All", 3);
      ComponentTestHelper.SetInput(_component, "All", 4);
      ComponentTestHelper.SetInput(_component, 100, 5);
      ComponentTestHelper.SetInput(_component, 76.5, 6);
      ComponentTestHelper.SetInput(_component, 1, 7);
      ComponentTestHelper.SetInput(_component, 2, 8);
      ComponentTestHelper.SetInput(_component, 3, 9);
      ComponentTestHelper.SetInput(_component, 2.2, 10);

      var output = ComputeAndGetOutput();
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
      Assert.Equal(GH_RuntimeMessageLevel.Blank, _component.RuntimeMessageLevel);
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
