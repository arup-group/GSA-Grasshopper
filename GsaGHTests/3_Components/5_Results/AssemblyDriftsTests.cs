using System;
using System.Collections.Generic;
using System.Linq;

using GsaGH.Helpers;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters.Results;


using OasysUnits;
using OasysUnits.Units;

using Xunit;

using AssemblyDrifts = GsaGH.Components.AssemblyDrifts;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class AssemblyDriftsTests {
    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new AssemblyDrifts();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftsMaxFromAnalysisCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      var comp = new AssemblyDrifts();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, "2", 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert
      double max = output.Max().As(Unit()); ;
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftsMaxFromCombinationCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedCombinationCaseC1Values(component).Max();

      // Act
      var comp = new AssemblyDrifts();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, "2", 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert
      double max = output.Max().As(Unit());
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftsMinFromAnalysisCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      var comp = new AssemblyDrifts();
      comp.SetSelected(0, 4 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, "2", 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert
      double min = output.Min().As(Unit());
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftsMinFromcombinationCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedCombinationCaseC1Values(component).Min();

      // Act
      var comp = new AssemblyDrifts();
      comp.SetSelected(0, 4 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, "2", 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert
      double min = output.Min().As(Unit());
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    private List<double> ExpectedAnalysisCaseValues(DriftResultVector component) {
      switch (component) {
        case DriftResultVector.X: return AssemblyDriftsA1.XInMillimeter();
        case DriftResultVector.Y: return AssemblyDriftsA1.YInMillimeter();
        case DriftResultVector.Xy: return AssemblyDriftsA1.XyInMillimeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC1Values(DriftResultVector component) {
      switch (component) {
        case DriftResultVector.X: return AssemblyDriftsC1.XInMillimeter();
        case DriftResultVector.Y: return AssemblyDriftsC1.YInMillimeter();
        case DriftResultVector.Xy: return AssemblyDriftsC1.XyInMillimeter();
      }

      throw new NotImplementedException();
    }

    private Enum Unit() {
      Enum unit = LengthUnit.Millimeter;
      return unit;
    }
  }
}
