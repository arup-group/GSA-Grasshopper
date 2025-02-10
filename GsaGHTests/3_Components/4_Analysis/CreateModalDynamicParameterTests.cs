using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Components.Analysis {

  public class CreateModalDynamicParameterTest {
    public CreateModalDynamicParameter _component;
    public GsaModalDynamic _modalDynamicAnalysis;
    public ModeCalculationMethod modelCalculationMethod;
    internal virtual void PrepareOutPut() { }

    public void SetNumberOfMode(double value) {
      ComponentTestHelper.SetInput(_component, value, 0);
      PrepareOutPut();
    }


    public void SetTargetMass(double x, double y, double z) {
      ComponentTestHelper.SetInput(_component, x, 0);
      ComponentTestHelper.SetInput(_component, y, 1);
      ComponentTestHelper.SetInput(_component, z, 2);
      PrepareOutPut();
    }

    public void SetLimitingMode(double value) {
      switch (modelCalculationMethod) {
        case ModeCalculationMethod.FrquencyRange:
          ComponentTestHelper.SetInput(_component, value, 2);
          break;
        case ModeCalculationMethod.TargetMassRatio:
          ComponentTestHelper.SetInput(_component, value, 3);
          break;
        default:
          break;
      }
      PrepareOutPut();
    }

    public void SetDampingStiffness(double value) {
      switch (modelCalculationMethod) {
        case ModeCalculationMethod.NumberOfMode:
          ComponentTestHelper.SetInput(_component, value, 4);
          break;
        case ModeCalculationMethod.FrquencyRange:
          ComponentTestHelper.SetInput(_component, value, 6);
          break;
        case ModeCalculationMethod.TargetMassRatio:
          ComponentTestHelper.SetInput(_component, value, 8);
          break;
        default:
          break;
      }
      PrepareOutPut();
    }

    public void SetLoadScaleFactor(double loadScaleFactor) {
      switch (modelCalculationMethod) {
        case ModeCalculationMethod.NumberOfMode:
          ComponentTestHelper.SetInput(_component, loadScaleFactor, 2);
          break;
        case ModeCalculationMethod.FrquencyRange:
          ComponentTestHelper.SetInput(_component, loadScaleFactor, 4);
          break;
        case ModeCalculationMethod.TargetMassRatio:
          ComponentTestHelper.SetInput(_component, loadScaleFactor, 6);
          break;
        default:
          break;
      }
      PrepareOutPut();
    }

    public void SetMassScaleFactor(double massScalefactor) {
      switch (modelCalculationMethod) {
        case ModeCalculationMethod.NumberOfMode:
          ComponentTestHelper.SetInput(_component, massScalefactor, 3);
          break;
        case ModeCalculationMethod.FrquencyRange:
          ComponentTestHelper.SetInput(_component, massScalefactor, 5);
          break;
        case ModeCalculationMethod.TargetMassRatio:
          ComponentTestHelper.SetInput(_component, massScalefactor, 7);
          break;
        default:
          break;
      }
      PrepareOutPut();
    }

    public void SetLoadCase(string loadCase) {
      switch (modelCalculationMethod) {
        case ModeCalculationMethod.NumberOfMode:
          ComponentTestHelper.SetInput(_component, loadCase, 1);
          break;
        case ModeCalculationMethod.FrquencyRange:
          ComponentTestHelper.SetInput(_component, loadCase, 3);
          break;
        case ModeCalculationMethod.TargetMassRatio:
          ComponentTestHelper.SetInput(_component, loadCase, 5);
          break;
        default:
          break;
      }
      PrepareOutPut();
    }

    public void SetMasil(bool value) {
      ComponentTestHelper.SetInput(_component, value, 4);
      PrepareOutPut();
    }

    public void SetFrequency(double lower, double upper) {
      ComponentTestHelper.SetInput(_component, lower, 0);
      ComponentTestHelper.SetInput(_component, upper, 1);
      PrepareOutPut();
    }

    public void SetDirection(Direction direction) {
      switch (direction) {
        case Direction.X:
          _component.SetSelected(2, 0);
          break;
        case Direction.Y:
          _component.SetSelected(2, 1);
          break;
        case Direction.Z:
          _component.SetSelected(2, 2);
          break;
        default:
          break;
      }
      PrepareOutPut();
    }

    public void SetMassOption(ModalMassOption massOption) {
      switch (massOption) {
        case ModalMassOption.LumpMassAtNode:
          _component.SetSelected(1, 0);
          break;
        case ModalMassOption.MassFromElementShapeFunction:
          _component.SetSelected(1, 1);
          break;
        case ModalMassOption.NodalMass:
          _component.SetSelected(1, 2);
          break;
        default:
          break;
      }
      PrepareOutPut();
    }

  }

  [Collection("GrasshopperFixture collection")]
  public class CreateModalDynamicParameterByNumberOfModesTests : CreateModalDynamicParameterTest {
    public ModeCalculationStrategyByNumberOfModes _modeCalculationStrategy;

    public CreateModalDynamicParameterByNumberOfModesTests() {
      _component = ComponentMother();
      modelCalculationMethod = ModeCalculationMethod.NumberOfMode;
    }

    internal override void PrepareOutPut() {
      var _modalDynamicAnalysisGoo = (GsaModalDynamicGoo)ComponentTestHelper.GetOutput(_component);
      if (_modalDynamicAnalysisGoo != null) {
        _modalDynamicAnalysis = _modalDynamicAnalysisGoo.Value;
        _modeCalculationStrategy = _modalDynamicAnalysis.ModeCalculationStrategy as ModeCalculationStrategyByNumberOfModes;
      }
    }

    public static CreateModalDynamicParameter ComponentMother() {
      var comp = new CreateModalDynamicParameter();
      comp.CreateAttributes();
      comp.SetSelected(0, 0);
      return comp;
    }

    [Fact]
    public void ComponentShouldReturnCorrectNumberOfMode() {
      SetNumberOfMode(5);
      Assert.Equal(5, _modeCalculationStrategy.NumberOfModes);
    }

    [Fact]
    public void ComponentShouldReportErrorWhenNumberOfModeIsNegativeValue() {
      SetNumberOfMode(-1);
      Assert.Single(_component.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void ComponentShouldReportErrorWhenStiffnessIsNotInRange() {
      SetDampingStiffness(2);
      Assert.Single(_component.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void ComponentShouldReportErrorWhenLoadScaleFactorsAreNegative() {
      SetLoadScaleFactor(-1);
      Assert.Single(_component.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void ComponentShouldReportErrorWhenMassScaleFactorsAreNegative() {
      SetMassScaleFactor(-1);
      Assert.Single(_component.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void ComponentShouldReturnCorrectCaseDefinition() {
      SetLoadCase("L1");
      Assert.Equal("L1", _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.CaseDefinition);
    }

    [Fact]
    public void ComponentShouldReturnCorrectLoadScaleFactor() {
      SetLoadScaleFactor(1.1);
      Assert.Equal(1.1, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.ScaleFactor);
    }

    [Fact]
    public void ComponentShouldReturnCorrectMassScaleFactor() {
      SetMassScaleFactor(1.2);
      Assert.Equal(1.2, _modalDynamicAnalysis.MassOption.ScaleFactor);
    }

    [Fact]
    public void ComponentShouldReturnCorrectStiffnessProportion() {
      SetDampingStiffness(1);
      Assert.Equal(1, _modalDynamicAnalysis.ModalDamping.StiffnessProportion);
    }

    [Fact]
    public void ComponentShouldReturnCorrectXDirecion() {
      SetDirection(Direction.X);
      Assert.Equal(Direction.X, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectYDirecion() {
      SetDirection(Direction.Y);
      Assert.Equal(Direction.Y, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectZDirecion() {
      SetDirection(Direction.Z);
      Assert.Equal(Direction.Z, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectNodalMassOption() {
      SetMassOption(ModalMassOption.NodalMass);
      Assert.Equal(ModalMassOption.NodalMass, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ComponentShouldReturnCorrectLumpMassOption() {
      SetMassOption(ModalMassOption.LumpMassAtNode);
      Assert.Equal(ModalMassOption.LumpMassAtNode, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ComponentShouldReturnCorrectElementShapeFunctionOption() {
      SetMassOption(ModalMassOption.MassFromElementShapeFunction);
      Assert.Equal(ModalMassOption.MassFromElementShapeFunction, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ModalDynamicCreatedFromTaskReturnCorrectMethod() {
      CreateAnalysisTask analysisTaskComponent = CreateAnalysisTaskTests.CreateAnalysisTaskComponent(ModeCalculationMethod.NumberOfMode);
      GsaAnalysisTask task = ((GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(analysisTaskComponent)).Value;
      var modaldynamic = new GsaModalDynamic(task.ApiTask);
      Assert.Equal(ModeCalculationMethod.NumberOfMode, modaldynamic.ModeCalculationOption());
    }

  }

  [Collection("GrasshopperFixture collection")]
  public class CreateModalDynamicParameterByFrquencyRangeTest : CreateModalDynamicParameterTest {
    private ModeCalculationStrategyByFrequency _modeCalculationStrategy;
    public CreateModalDynamicParameterByFrquencyRangeTest() {
      _component = ComponentMother();
      modelCalculationMethod = ModeCalculationMethod.FrquencyRange;
    }

    internal override void PrepareOutPut() {
      var _modalDynamicAnalysisGoo = (GsaModalDynamicGoo)ComponentTestHelper.GetOutput(_component);
      if (_modalDynamicAnalysisGoo != null) {
        _modalDynamicAnalysis = _modalDynamicAnalysisGoo.Value;
        _modeCalculationStrategy = _modalDynamicAnalysis.ModeCalculationStrategy as ModeCalculationStrategyByFrequency;
      }
    }

    public static CreateModalDynamicParameter ComponentMother() {
      var comp = new CreateModalDynamicParameter();
      comp.CreateAttributes();
      comp.SetSelected(0, 1);
      return comp;
    }

    [Fact]
    public void ComponentShouldReturnCorrectNumberOfLimitingMode() {
      SetLimitingMode(30);
      Assert.Equal(30, _modeCalculationStrategy.MaximumNumberOfModes);
    }

    [Fact]
    public void ComponentShouldReturnCorrectLowerFrequency() {
      SetFrequency(5, 10);
      Assert.Equal(5, _modeCalculationStrategy.LowerFrequency);
    }

    [Fact]
    public void ComponentShouldReturnCorrectUpperFrequency() {
      SetFrequency(0, 10);
      Assert.Equal(10, _modeCalculationStrategy.HigherFrequency);
    }

    [Fact]
    public void ComponentShouldReportErrorIfFrequenciesAreNotCorrect() {
      SetFrequency(6, 5);
      Assert.Equal(2,_component.RuntimeMessages(GH_RuntimeMessageLevel.Error).Count);
    }

    [Fact]
    public void ComponentShouldReportErrorIfLowerFrequencyIsEqualLowerFrequencyCorrect() {
      SetFrequency(5, 5);
      Assert.Equal(2, _component.RuntimeMessages(GH_RuntimeMessageLevel.Error).Count);
    }

    [Fact]
    public void ComponentShouldReturnCorrectCaseDefinition() {
      SetLoadCase("L1");
      Assert.Equal("L1", _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.CaseDefinition);
    }

    [Fact]
    public void ComponentShouldReturnCorrectLoadScaleFactor() {
      SetLoadScaleFactor(1.1);
      Assert.Equal(1.1, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.ScaleFactor);
    }

    [Fact]
    public void ComponentShouldReturnCorrectMassScaleFactor() {
      SetMassScaleFactor(1.2);
      Assert.Equal(1.2, _modalDynamicAnalysis.MassOption.ScaleFactor);
    }

    [Fact]
    public void ComponentShouldReturnCorrectStiffnessProportion() {
      SetDampingStiffness(1);
      Assert.Equal(1, _modalDynamicAnalysis.ModalDamping.StiffnessProportion);
    }

    [Fact]
    public void ComponentShouldReturnCorrectXDirecion() {
      SetDirection(Direction.X);
      Assert.Equal(Direction.X, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectYDirecion() {
      SetDirection(Direction.Y);
      Assert.Equal(Direction.Y, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectZDirecion() {
      SetDirection(Direction.Z);
      Assert.Equal(Direction.Z, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectNodalMassOption() {
      SetMassOption(ModalMassOption.NodalMass);
      Assert.Equal(ModalMassOption.NodalMass, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ComponentShouldReturnCorrectLumpMassOption() {
      SetMassOption(ModalMassOption.LumpMassAtNode);
      Assert.Equal(ModalMassOption.LumpMassAtNode, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ComponentShouldReturnCorrectElementShapeFunctionOption() {
      SetMassOption(ModalMassOption.MassFromElementShapeFunction);
      Assert.Equal(ModalMassOption.MassFromElementShapeFunction, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ModalDynamicCreatedFromTaskReturnCorrectMethod() {
      CreateAnalysisTask analysisTaskComponent = CreateAnalysisTaskTests.CreateAnalysisTaskComponent(ModeCalculationMethod.FrquencyRange);
      GsaAnalysisTask task = ((GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(analysisTaskComponent)).Value;
      var modaldynamic = new GsaModalDynamic(task.ApiTask);
      Assert.Equal(ModeCalculationMethod.FrquencyRange, modaldynamic.ModeCalculationOption());
    }

  }

  [Collection("GrasshopperFixture collection")]
  public class CreateModalDynamicParameterByTargetMassParticipationTest : CreateModalDynamicParameterTest {
    private ModeCalculationStrategyByMassParticipation _modeCalculationStrategy;
    public CreateModalDynamicParameterByTargetMassParticipationTest() {
      _component = ComponentMother();
      modelCalculationMethod = ModeCalculationMethod.TargetMassRatio;
    }

    internal override void PrepareOutPut() {
      var _modalDynamicAnalysisGoo = (GsaModalDynamicGoo)ComponentTestHelper.GetOutput(_component);
      if (_modalDynamicAnalysisGoo != null) {
        _modalDynamicAnalysis = _modalDynamicAnalysisGoo.Value;
        _modeCalculationStrategy = _modalDynamicAnalysis.ModeCalculationStrategy as ModeCalculationStrategyByMassParticipation;
      }
    }

    public static CreateModalDynamicParameter ComponentMother() {
      var comp = new CreateModalDynamicParameter();
      comp.CreateAttributes();
      comp.SetSelected(0, 2);
      return comp;
    }

    [Fact]
    public void ComponentShouldReportErrorIfTargetMassFactorLessThanZeroInXDirection() {
      SetTargetMass(-1, 100, 100);
      Assert.Single(_component.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void ComponentShouldReportErrorIfTargetMassFactorGreaterThan100InXDirection() {
      SetTargetMass(101, 100, 100);
      Assert.Single(_component.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void ComponentShouldReportErrorIfTargetMassFactorLessThanZeroInYDirection() {
      SetTargetMass(100, -1, 100);
      Assert.Single(_component.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void ComponentShouldReportErrorIfTargetMassFactorGreaterThan100InYDirection() {
      SetTargetMass(100, 101, 100);
      Assert.Single(_component.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void ComponentShouldReportErrorIfTargetMassFactorLessThanZeroInZDirection() {
      SetTargetMass(100, 100, -1);
      Assert.Single(_component.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void ComponentShouldReportErrorIfTargetMassFactorGreaterThan100InZDirection() {
      SetTargetMass(100, 100, 101);
      Assert.Single(_component.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void ComponentShouldReturnCorrectXDirectionParticipation() {
      SetTargetMass(90, 100, 100);
      Assert.Equal(90, _modeCalculationStrategy.TargetMassInXDirection);
    }

    [Fact]
    public void ComponentShouldReturnCorrectYDirectionParticipation() {
      SetTargetMass(90, 95, 100);
      Assert.Equal(95, _modeCalculationStrategy.TargetMassInYDirection);
    }

    [Fact]
    public void ComponentShouldReturnCorrectZDirectionParticipation() {
      SetTargetMass(90, 100, 99);
      Assert.Equal(99, _modeCalculationStrategy.TargetMassInZDirection);
    }

    [Fact]
    public void ComponentShouldReturnCorrectNumberOfLimitingMode() {
      SetLimitingMode(10);
      Assert.Equal(10, _modeCalculationStrategy.MaximumNumberOfModes);
    }

    [Fact]
    public void ComponentShouldReturnMASILOptionTrue() {
      SetMasil(true);
      Assert.True(_modeCalculationStrategy.SkipModesWithLowMassParticipation);
    }

    [Fact]
    public void ComponentShouldReturnMASILOptionFalse() {
      SetMasil(false);
      Assert.False(_modeCalculationStrategy.SkipModesWithLowMassParticipation);
    }

    [Fact]
    public void ComponentShouldReturnCorrectCaseDefinition() {
      SetLoadCase("L1");
      Assert.Equal("L1", _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.CaseDefinition);
    }

    [Fact]
    public void ComponentShouldReturnCorrectLoadScaleFactor() {
      SetLoadScaleFactor(1.1);
      Assert.Equal(1.1, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.ScaleFactor);
    }

    [Fact]
    public void ComponentShouldReturnCorrectMassScaleFactor() {
      SetMassScaleFactor(1.2);
      Assert.Equal(1.2, _modalDynamicAnalysis.MassOption.ScaleFactor);
    }

    [Fact]
    public void ComponentShouldReturnCorrectStiffnessProportion() {
      SetDampingStiffness(1);
      Assert.Equal(1, _modalDynamicAnalysis.ModalDamping.StiffnessProportion);
    }

    [Fact]
    public void ComponentShouldReturnCorrectXDirecion() {
      SetDirection(Direction.X);
      Assert.Equal(Direction.X, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectYDirecion() {
      SetDirection(Direction.Y);
      Assert.Equal(Direction.Y, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectZDirecion() {
      SetDirection(Direction.Z);
      Assert.Equal(Direction.Z, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectNodalMassOption() {
      SetMassOption(ModalMassOption.NodalMass);
      Assert.Equal(ModalMassOption.NodalMass, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ComponentShouldReturnCorrectLumpMassOption() {
      SetMassOption(ModalMassOption.LumpMassAtNode);
      Assert.Equal(ModalMassOption.LumpMassAtNode, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ComponentShouldReturnCorrectElementShapeFunctionOption() {
      SetMassOption(ModalMassOption.MassFromElementShapeFunction);
      Assert.Equal(ModalMassOption.MassFromElementShapeFunction, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ModalDynamicCreatedFromTaskReturnCorrectMethod() {
      CreateAnalysisTask analysisTaskComponent = CreateAnalysisTaskTests.CreateAnalysisTaskComponent(ModeCalculationMethod.TargetMassRatio);
      GsaAnalysisTask task = ((GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(analysisTaskComponent)).Value;
      var modaldynamic = new GsaModalDynamic(task.ApiTask);
      Assert.Equal(ModeCalculationMethod.TargetMassRatio, modaldynamic.ModeCalculationOption());
    }

  }
}
