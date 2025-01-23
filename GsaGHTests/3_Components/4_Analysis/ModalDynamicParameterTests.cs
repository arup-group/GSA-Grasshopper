using System.Linq;

using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Components.Analysis {
  [Collection("GrasshopperFixture collection")]
  public class ModalDynamicParameterByNumberOfModesTests {
    private GH_OasysComponent _component;
    private ModeCalculationStrategyByNumberOfModes _modeCalculationStrategy;
    private GsaModalDynamicAnalysis _modalDynamicAnalysis;
    public ModalDynamicParameterByNumberOfModesTests(ModalMassOption massOption = ModalMassOption.NodalMass, Direction direction = Direction.X) {
      PrepareComponent(massOption, direction);
    }


    private void PrepareComponent(ModalMassOption massOption, Direction direction) {
      _component = ComponentMother(massOption, direction);
      var _modalDynamicAnalysisGoo = (GsaModalDynamicAnalysisGoo)ComponentTestHelper.GetOutput(_component);
      _modalDynamicAnalysis = _modalDynamicAnalysisGoo.Value;
      _modeCalculationStrategy = _modalDynamicAnalysis.ModeCalculationStrategy as ModeCalculationStrategyByNumberOfModes;
    }

    public static GH_OasysComponent ComponentMother(ModalMassOption massOption, Direction direction) {
      var comp = new CreateModalDynamicAnalysisParameter();
      comp.CreateAttributes();
      comp.SetSelected(0, 0);
      ComponentTestHelper.SetInput(comp, 5, 1);
      ComponentTestHelper.SetInput(comp, "L1", 2);
      ComponentTestHelper.SetInput(comp, 1.1, 3);
      ComponentTestHelper.SetInput(comp, 1.2, 4);
      ComponentTestHelper.SetInput(comp, 1.3, 5);

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

  }

  [Collection("GrasshopperFixture collection")]
  public class ModalDynamicParameterByFrquencyRangeTest {
    private GH_OasysComponent _component;
    private ModeCalculationStrategyByFrequency _modeCalculationStrategy;
    private GsaModalDynamicAnalysis _modalDynamicAnalysis;
    public ModalDynamicParameterByFrquencyRangeTest(ModalMassOption massOption = ModalMassOption.NodalMass, Direction direction = Direction.X) {
      PrepareComponent(massOption, direction);
    }


    private void PrepareComponent(ModalMassOption massOption, Direction direction) {
      _component = ComponentMother(massOption, direction);
      var _modalDynamicAnalysisGoo = (GsaModalDynamicAnalysisGoo)ComponentTestHelper.GetOutput(_component);
      _modalDynamicAnalysis = _modalDynamicAnalysisGoo.Value;
      _modeCalculationStrategy = _modalDynamicAnalysis.ModeCalculationStrategy as ModeCalculationStrategyByFrequency;
    }

    public static GH_OasysComponent ComponentMother(ModalMassOption massOption, Direction direction) {
      var comp = new CreateModalDynamicAnalysisParameter();
      comp.CreateAttributes();
      comp.SetSelected(0, 1);
      ComponentTestHelper.SetInput(comp, 5, 1);
      ComponentTestHelper.SetInput(comp, 10, 2);
      ComponentTestHelper.SetInput(comp, 30, 3);
      ComponentTestHelper.SetInput(comp, "L1", 4);
      ComponentTestHelper.SetInput(comp, 1.1, 5);
      ComponentTestHelper.SetInput(comp, 1.2, 6);
      ComponentTestHelper.SetInput(comp, 1.3, 7);

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

  }

  [Collection("GrasshopperFixture collection")]
  public class ModalDynamicParameterByTargetMassParticipationTest {
    private GH_OasysComponent _component;
    private ModeCalculationStrategyByMassParticipation _modeCalculationStrategy;
    private GsaModalDynamicAnalysis _modalDynamicAnalysis;
    public ModalDynamicParameterByTargetMassParticipationTest(ModalMassOption massOption = ModalMassOption.NodalMass, Direction direction = Direction.X) {
      PrepareComponent(massOption, direction);
    }


    private void PrepareComponent(ModalMassOption massOption, Direction direction) {
      _component = ComponentMother(massOption, direction);
      var _modalDynamicAnalysisGoo = (GsaModalDynamicAnalysisGoo)ComponentTestHelper.GetOutput(_component);
      _modalDynamicAnalysis = _modalDynamicAnalysisGoo.Value;
      _modeCalculationStrategy = _modalDynamicAnalysis.ModeCalculationStrategy as ModeCalculationStrategyByMassParticipation;
    }

    public static GH_OasysComponent ComponentMother(ModalMassOption massOption, Direction direction) {
      var comp = new CreateModalDynamicAnalysisParameter();
      comp.CreateAttributes();
      comp.SetSelected(0, 2);
      ComponentTestHelper.SetInput(comp, 90, 1);
      ComponentTestHelper.SetInput(comp, 95, 2);
      ComponentTestHelper.SetInput(comp, 100, 3);
      ComponentTestHelper.SetInput(comp, 10, 4);
      ComponentTestHelper.SetInput(comp, true, 5);
      ComponentTestHelper.SetInput(comp, "L1", 6);
      ComponentTestHelper.SetInput(comp, 1.1, 7);
      ComponentTestHelper.SetInput(comp, 1.2, 8);
      ComponentTestHelper.SetInput(comp, 1.3, 9);

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

  }
}
