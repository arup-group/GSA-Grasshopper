using System.Linq;

using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Analysis {
  [Collection("GrasshopperFixture collection")]
  public class CreateModalDynamicParameterByNumberOfModesTests {
    private GH_OasysComponent _component;
    private ModeCalculationStrategyByNumberOfModes _modeCalculationStrategy;
    private GsaModalDynamic _modalDynamicAnalysis;
    public CreateModalDynamicParameterByNumberOfModesTests(ModalMassOption massOption = ModalMassOption.NodalMass, Direction direction = Direction.X) {
      PrepareComponent(massOption, direction);
    }


    private void PrepareComponent(ModalMassOption massOption, Direction direction) {
      _component = ComponentMother(massOption, direction);
      var _modalDynamicAnalysisGoo = (GsaModalDynamicGoo)ComponentTestHelper.GetOutput(_component);
      _modalDynamicAnalysis = _modalDynamicAnalysisGoo.Value;
      _modeCalculationStrategy = _modalDynamicAnalysis.ModeCalculationStrategy as ModeCalculationStrategyByNumberOfModes;
    }

    public static GH_OasysComponent ComponentMother(ModalMassOption massOption, Direction direction) {
      var comp = new CreateModalDynamicParameter();
      comp.CreateAttributes();
      comp.SetSelected(0, 0);
      int index = 0;
      ComponentTestHelper.SetInput(comp, 5, index++);
      ComponentTestHelper.SetInput(comp, "L1", index++);
      ComponentTestHelper.SetInput(comp, 1.1, index++);
      ComponentTestHelper.SetInput(comp, 1.2, index++);
      ComponentTestHelper.SetInput(comp, 1.3, index);

      switch (massOption) {
        case ModalMassOption.LumpMassAtNode:
          comp.SetSelected(1, 0);
          break;
        case ModalMassOption.MassFromElementShapeFunction:
          comp.SetSelected(1, 1);
          break;
        case ModalMassOption.NodalMass:
          comp.SetSelected(1, 2);
          break;
        default:
          break;
      }

      switch (direction) {
        case Direction.X:
          comp.SetSelected(2, 0);
          break;
        case Direction.Y:
          comp.SetSelected(2, 1);
          break;
        case Direction.Z:
          comp.SetSelected(2, 2);
          break;
        default:
          break;
      }
      return comp;
    }

    [Fact]
    public void ComponentShouldReturnCorrectNumberOfMode() {
      Assert.Equal(5, _modeCalculationStrategy.NumberOfModes);
    }

    [Fact]
    public void ComponentShouldReturnCorrectCaseDefinition() {
      Assert.Equal("L1", _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.CaseDefinition);
    }

    [Fact]
    public void ComponentShouldReturnCorrectLoadScaleFactor() {
      Assert.Equal(1.1, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.ScaleFactor);
    }

    [Fact]
    public void ComponentShouldReturnCorrectMassScaleFactor() {
      Assert.Equal(1.2, _modalDynamicAnalysis.MassOption.ScaleFactor);
    }

    [Fact]
    public void ComponentShouldReturnCorrectStiffnessProportion() {
      Assert.Equal(1.3, _modalDynamicAnalysis.ModalDamping.StiffnessProportion);
    }

    [Fact]
    public void ComponentShouldReturnCorrectNodalMassOption() {
      Assert.Equal(ModalMassOption.NodalMass, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ComponentShouldReturnCorrectXDirecion() {
      Assert.Equal(Direction.X, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectYDirecion() {
      PrepareComponent(ModalMassOption.NodalMass, Direction.Y);
      Assert.Equal(Direction.Y, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectZDirecion() {
      PrepareComponent(ModalMassOption.NodalMass, Direction.Z);
      Assert.Equal(Direction.Z, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectLumpMassOption() {
      PrepareComponent(ModalMassOption.LumpMassAtNode, Direction.Y);
      Assert.Equal(ModalMassOption.LumpMassAtNode, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ComponentShouldReturnCorrectElementShapeFunctionOption() {
      PrepareComponent(ModalMassOption.MassFromElementShapeFunction, Direction.Z);
      Assert.Equal(ModalMassOption.MassFromElementShapeFunction, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ModalDynamicCreatedFromTaskReturnCorrectMethod() {
      CreateAnalysisTask analysisTaskComponent = CreateAnalysisTaskTests.CreateAnalysisTaskComponent(ModeCalculationMethod.NumberOfMode);
      GsaAnalysisTask task = ((GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(analysisTaskComponent)).Value;
      var modaldynamic = new GsaModalDynamic(task.ApiTask);
      Assert.Equal(ModeCalculationMethod.NumberOfMode, modaldynamic.Method());
    }

  }

  [Collection("GrasshopperFixture collection")]
  public class CreateModalDynamicParameterByFrquencyRangeTest {
    private GH_OasysComponent _component;
    private ModeCalculationStrategyByFrequency _modeCalculationStrategy;
    private GsaModalDynamic _modalDynamicAnalysis;
    public CreateModalDynamicParameterByFrquencyRangeTest(ModalMassOption massOption = ModalMassOption.NodalMass, Direction direction = Direction.X) {
      PrepareComponent(massOption, direction);
    }


    private void PrepareComponent(ModalMassOption massOption, Direction direction) {
      _component = ComponentMother(massOption, direction);
      var _modalDynamicAnalysisGoo = (GsaModalDynamicGoo)ComponentTestHelper.GetOutput(_component);
      _modalDynamicAnalysis = _modalDynamicAnalysisGoo.Value;
      _modeCalculationStrategy = _modalDynamicAnalysis.ModeCalculationStrategy as ModeCalculationStrategyByFrequency;
    }

    public static GH_OasysComponent ComponentMother(ModalMassOption massOption= ModalMassOption.LumpMassAtNode, Direction direction= Direction.Y) {
      var comp = new CreateModalDynamicParameter();
      comp.CreateAttributes();
      comp.SetSelected(0, 1);
      int index = 0;
      ComponentTestHelper.SetInput(comp, 5, index++);
      ComponentTestHelper.SetInput(comp, 10, index++);
      ComponentTestHelper.SetInput(comp, 30, index++);
      ComponentTestHelper.SetInput(comp, "L1", index++);
      ComponentTestHelper.SetInput(comp, 1.1, index++);
      ComponentTestHelper.SetInput(comp, 1.2, index++);
      ComponentTestHelper.SetInput(comp, 1.3, index);

      switch (massOption) {
        case ModalMassOption.LumpMassAtNode:
          comp.SetSelected(1, 0);
          break;
        case ModalMassOption.MassFromElementShapeFunction:
          comp.SetSelected(1, 1);
          break;
        case ModalMassOption.NodalMass:
          comp.SetSelected(1, 2);
          break;
        default:
          break;
      }

      switch (direction) {
        case Direction.X:
          comp.SetSelected(2, 0);
          break;
        case Direction.Y:
          comp.SetSelected(2, 1);
          break;
        case Direction.Z:
          comp.SetSelected(2, 2);
          break;
        default:
          break;
      }
      return comp;
    }

    [Fact]
    public void ComponentShouldReturnCorrectNumberOfLimitingMode() {
      Assert.Equal(30, _modeCalculationStrategy.MaximumNumberOfModes);
    }

    [Fact]
    public void ComponentShouldReturnCorrectUpperFrequency() {
      Assert.Equal(10, _modeCalculationStrategy.HigherFrequency);
    }

    [Fact]
    public void ComponentShouldReturnCorrectLowerFrequency() {
      Assert.Equal(5, _modeCalculationStrategy.LowerFrequency);
    }

    [Fact]
    public void ComponentShouldReturnCorrectCaseDefinition() {
      Assert.Equal("L1", _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.CaseDefinition);
    }

    [Fact]
    public void ComponentShouldReturnCorrectLoadScaleFactor() {
      Assert.Equal(1.1, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.ScaleFactor);
    }

    [Fact]
    public void ComponentShouldReturnCorrectMassScaleFactor() {
      Assert.Equal(1.2, _modalDynamicAnalysis.MassOption.ScaleFactor);
    }

    [Fact]
    public void ComponentShouldReturnCorrectStiffnessProportion() {
      Assert.Equal(1.3, _modalDynamicAnalysis.ModalDamping.StiffnessProportion);
    }

    [Fact]
    public void ComponentShouldReturnCorrectNodalMassOption() {
      Assert.Equal(ModalMassOption.NodalMass, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ComponentShouldReturnCorrectXDirecion() {
      Assert.Equal(Direction.X, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectYDirecion() {
      PrepareComponent(ModalMassOption.NodalMass, Direction.Y);
      Assert.Equal(Direction.Y, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectZDirecion() {
      PrepareComponent(ModalMassOption.NodalMass, Direction.Z);
      Assert.Equal(Direction.Z, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectLumpMassOption() {
      PrepareComponent(ModalMassOption.LumpMassAtNode, Direction.Y);
      Assert.Equal(ModalMassOption.LumpMassAtNode, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ComponentShouldReturnCorrectElementShapeFunctionOption() {
      PrepareComponent(ModalMassOption.MassFromElementShapeFunction, Direction.Z);
      Assert.Equal(ModalMassOption.MassFromElementShapeFunction, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ModalDynamicCreatedFromTaskReturnCorrectMethod() {
      CreateAnalysisTask analysisTaskComponent = CreateAnalysisTaskTests.CreateAnalysisTaskComponent(ModeCalculationMethod.FrquencyRange);
      GsaAnalysisTask task = ((GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(analysisTaskComponent)).Value;
      var modaldynamic = new GsaModalDynamic(task.ApiTask);
      Assert.Equal(ModeCalculationMethod.FrquencyRange, modaldynamic.Method());
    }

  }

  [Collection("GrasshopperFixture collection")]
  public class CreateModalDynamicParameterByTargetMassParticipationTest {
    private GH_OasysComponent _component;
    private ModeCalculationStrategyByMassParticipation _modeCalculationStrategy;
    private GsaModalDynamic _modalDynamicAnalysis;
    public CreateModalDynamicParameterByTargetMassParticipationTest(ModalMassOption massOption = ModalMassOption.NodalMass, Direction direction = Direction.X) {
      PrepareComponent(massOption, direction);
    }


    private void PrepareComponent(ModalMassOption massOption, Direction direction) {
      _component = ComponentMother(massOption, direction);
      var _modalDynamicAnalysisGoo = (GsaModalDynamicGoo)ComponentTestHelper.GetOutput(_component);
      _modalDynamicAnalysis = _modalDynamicAnalysisGoo.Value;
      _modeCalculationStrategy = _modalDynamicAnalysis.ModeCalculationStrategy as ModeCalculationStrategyByMassParticipation;
    }

    public static GH_OasysComponent ComponentMother(ModalMassOption massOption, Direction direction) {
      var comp = new CreateModalDynamicParameter();
      comp.CreateAttributes();
      comp.SetSelected(0, 2);
      int index = 0;
      ComponentTestHelper.SetInput(comp, 90, index++);
      ComponentTestHelper.SetInput(comp, 95, index++);
      ComponentTestHelper.SetInput(comp, 100, index++);
      ComponentTestHelper.SetInput(comp, 10, index++);
      ComponentTestHelper.SetInput(comp, true, index++);
      ComponentTestHelper.SetInput(comp, "L1", index++);
      ComponentTestHelper.SetInput(comp, 1.1, index++);
      ComponentTestHelper.SetInput(comp, 1.2, index++);
      ComponentTestHelper.SetInput(comp, 1.3, index);

      switch (massOption) {
        case ModalMassOption.LumpMassAtNode:
          comp.SetSelected(1, 0);
          break;
        case ModalMassOption.MassFromElementShapeFunction:
          comp.SetSelected(1, 1);
          break;
        case ModalMassOption.NodalMass:
          comp.SetSelected(1, 2);
          break;
        default:
          break;
      }

      switch (direction) {
        case Direction.X:
          comp.SetSelected(2, 0);
          break;
        case Direction.Y:
          comp.SetSelected(2, 1);
          break;
        case Direction.Z:
          comp.SetSelected(2, 2);
          break;
        default:
          break;
      }
      return comp;
    }

    [Fact]
    public void ComponentShouldReturnCorrectNumberOfLimitingMode() {
      Assert.Equal(10, _modeCalculationStrategy.MaximumNumberOfModes);
    }

    [Fact]
    public void ComponentShouldReturnCorrectXDirectionParticipation() {
      Assert.Equal(90, _modeCalculationStrategy.TargetMassInXDirection);
    }

    [Fact]
    public void ComponentShouldReturnCorrectYDirectionParticipation() {
      Assert.Equal(95, _modeCalculationStrategy.TargetMassInYDirection);
    }

    [Fact]
    public void ComponentShouldReturnCorrectZDirectionParticipation() {
      Assert.Equal(100, _modeCalculationStrategy.TargetMassInZDirection);
    }

    [Fact]
    public void ComponentShouldReturnCorrectMASILOption() {
      Assert.True(_modeCalculationStrategy.SkipModesWithLowMassParticipation);
    }

    [Fact]
    public void ComponentShouldReturnCorrectCaseDefinition() {
      Assert.Equal("L1", _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.CaseDefinition);
    }

    [Fact]
    public void ComponentShouldReturnCorrectLoadScaleFactor() {
      Assert.Equal(1.1, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.ScaleFactor);
    }

    [Fact]
    public void ComponentShouldReturnCorrectMassScaleFactor() {
      Assert.Equal(1.2, _modalDynamicAnalysis.MassOption.ScaleFactor);
    }

    [Fact]
    public void ComponentShouldReturnCorrectStiffnessProportion() {
      Assert.Equal(1.3, _modalDynamicAnalysis.ModalDamping.StiffnessProportion);
    }

    [Fact]
    public void ComponentShouldReturnCorrectNodalMassOption() {
      Assert.Equal(ModalMassOption.NodalMass, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ComponentShouldReturnCorrectXDirecion() {
      Assert.Equal(Direction.X, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectYDirecion() {
      PrepareComponent(ModalMassOption.NodalMass, Direction.Y);
      Assert.Equal(Direction.Y, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectZDirecion() {
      PrepareComponent(ModalMassOption.NodalMass, Direction.Z);
      Assert.Equal(Direction.Z, _modalDynamicAnalysis.AdditionalMassDerivedFromLoads.Direction);
    }

    [Fact]
    public void ComponentShouldReturnCorrectLumpMassOption() {
      PrepareComponent(ModalMassOption.LumpMassAtNode, Direction.Y);
      Assert.Equal(ModalMassOption.LumpMassAtNode, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ComponentShouldReturnCorrectElementShapeFunctionOption() {
      PrepareComponent(ModalMassOption.MassFromElementShapeFunction, Direction.Z);
      Assert.Equal(ModalMassOption.MassFromElementShapeFunction, _modalDynamicAnalysis.MassOption.ModalMassOption);
    }

    [Fact]
    public void ModalDynamicCreatedFromTaskReturnCorrectMethod() {
      CreateAnalysisTask analysisTaskComponent = CreateAnalysisTaskTests.CreateAnalysisTaskComponent(ModeCalculationMethod.TargetMassRatio);
      GsaAnalysisTask task = ((GsaAnalysisTaskGoo)ComponentTestHelper.GetOutput(analysisTaskComponent)).Value;
      var modaldynamic = new GsaModalDynamic(task.ApiTask);
      Assert.Equal(ModeCalculationMethod.TargetMassRatio, modaldynamic.Method());
    }

  }
}
