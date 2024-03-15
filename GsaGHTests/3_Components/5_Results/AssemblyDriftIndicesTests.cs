﻿using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters.Results;
using OasysUnits;
using OasysUnits.Units;
using Xunit;
using AssemblyDriftIndices = GsaGH.Components.AssemblyDriftIndices;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class AssemblyDriftIndicesTests {
    private static double FindMax(IList<IGH_Goo> output) {
      double max = double.MinValue;
      foreach (IGH_Goo goo in output) {
        var number = (GH_Number)goo;
        double value = (double)number.Value;
        if (value > max) {
          max = value;
        }
      }
      return max;
    }

    private static double FindMin(IList<IGH_Goo> output) {
      double min = double.MaxValue;
      foreach (IGH_Goo goo in output) {
        var number = (GH_Number)goo;
        double value = (double)number.Value;
        if (value < min) {
          min = value;
        }
      }
      return min;
    }

    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new AssemblyDriftIndices();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftIndicesMaxFromAnalysisCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      var comp = new AssemblyDriftIndices();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, "2", 1);

      var output = (IList<IGH_Goo>)ComponentTestHelper.GetListOutput(comp, (int)component);

      // Assert
      double max = FindMax(output);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftIndicesMaxFromCombinationCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedCombinationCaseC1Values(component).Max();

      // Act
      var comp = new AssemblyDriftIndices();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, "2", 1);

      var output = (IList<IGH_Goo>)ComponentTestHelper.GetListOutput(comp, (int)component);

      // Assert
      double max = FindMax(output);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftIndicesMinFromAnalysisCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      var comp = new AssemblyDriftIndices();
      comp.SetSelected(0, 4 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, "2", 1);

      var output = (IList<IGH_Goo>)ComponentTestHelper.GetListOutput(comp, (int)component);

      // Assert 
      double min = FindMin(output);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftIndicesMinFromcombinationCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedCombinationCaseC1Values(component).Min();

      // Act
      var comp = new AssemblyDriftIndices();
      comp.SetSelected(0, 4 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, "2", 1);

      var output = (IList<IGH_Goo>)ComponentTestHelper.GetListOutput(comp, (int)component);

      // Assert
      double min = FindMin(output);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    private List<double> ExpectedAnalysisCaseValues(DriftResultVector component) {
      switch (component) {
        case DriftResultVector.X: return AssemblyDriftIndicesA1.X();
        case DriftResultVector.Y: return AssemblyDriftIndicesA1.Y();
        case DriftResultVector.Xy: return AssemblyDriftIndicesA1.Xy();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC1Values(DriftResultVector component) {
      switch (component) {
        case DriftResultVector.X: return AssemblyDriftIndicesC1.X();
        case DriftResultVector.Y: return AssemblyDriftIndicesC1.Y();
        case DriftResultVector.Xy: return AssemblyDriftIndicesC1.Xy();
      }

      throw new NotImplementedException();
    }
  }
}
