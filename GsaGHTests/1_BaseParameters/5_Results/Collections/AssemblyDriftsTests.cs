using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Helpers;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;


using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class AssemblyDriftsTests {
    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftsMaxFromAnalysisCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      AssemblyDrifts resultSet = result.AssemblyDrifts.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftsMaxFromCombinationCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedCombinationCaseValues(component).Max();

      // Act
      AssemblyDrifts resultSet = result.AssemblyDrifts.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
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
      AssemblyDrifts resultSet = result.AssemblyDrifts.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftsMinFromCombinationCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedCombinationCaseValues(component).Min();

      // Act
      AssemblyDrifts resultSet = result.AssemblyDrifts.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftValuesFromAnalysisCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      AssemblyDrifts resultSet = result.AssemblyDrifts.ResultSubset(new Collection<int>() { 2 });

      // Assert
      IList<IEntity1dQuantity<Drift>> driftQuantity = resultSet.Subset[2];
      Assert.Single(driftQuantity);

      int position = 0;
      foreach (Drift drift in driftQuantity[0].Results.Values) {
        double x = TestsResultHelper.ResultsHelper(drift, component);
        Assert.Equal(expected[position++], x, DoubleComparer.Default);
      }
    }

    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftValuesFromCombinationCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblySimple, 1);
      List<double> expected = ExpectedCombinationCaseValues(component);

      // Act
      AssemblyDrifts resultSet = result.AssemblyDrifts.ResultSubset(new Collection<int>() { 2 });

      // Assert
      IList<IEntity1dQuantity<Drift>> driftQuantity = resultSet.Subset[2];
      Assert.Single(driftQuantity);

      int position = 0;
      foreach (Drift drift in driftQuantity[0].Results.Values) {
        double x = TestsResultHelper.ResultsHelper(drift, component);
        Assert.Equal(expected[position++], x, DoubleComparer.Default);
      }
    }

    private List<double> ExpectedAnalysisCaseValues(DriftResultVector component) {
      switch (component) {
        case DriftResultVector.X: return AssemblyDriftsA1.XInMillimeter();
        case DriftResultVector.Y: return AssemblyDriftsA1.YInMillimeter();
        case DriftResultVector.Xy: return AssemblyDriftsA1.XyInMillimeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseValues(DriftResultVector component) {
      switch (component) {
        case DriftResultVector.X: return AssemblyDriftsC1.XInMillimeter();
        case DriftResultVector.Y: return AssemblyDriftsC1.YInMillimeter();
        case DriftResultVector.Xy: return AssemblyDriftsC1.XyInMillimeter();
      }

      throw new NotImplementedException();
    }
  }
}
