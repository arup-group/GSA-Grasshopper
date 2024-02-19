﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using OasysUnits;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class AssemblyDriftIndicesTests {
    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftIndicesMaxFromAnalysisCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      AssemblyDriftIndices resultSet = result.AssemblyDriftIndices.ResultSubset(new Collection<int>() { 2 });

      // Assert 
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max, 1E-6);
    }

    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftIndicesMaxFromCombinationCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedCombinationCaseValues(component).Max();

      // Act
      AssemblyDriftIndices resultSet = result.AssemblyDriftIndices.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max, 1E-5);
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
      AssemblyDriftIndices resultSet = result.AssemblyDriftIndices.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, 1E-6);
    }

    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftIndicesMinFromCombinationCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedCombinationCaseValues(component).Min();

      // Act
      AssemblyDriftIndices resultSet = result.AssemblyDriftIndices.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, 1E-6);
    }

    [Theory]
    [InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    [InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftIndexValuesFromAnalysisCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      AssemblyDriftIndices resultSet = result.AssemblyDriftIndices.ResultSubset(new Collection<int>() { 2 });

      // Assert
      IList<IAssemblyQuantity<IDrift<double>>> driftQuantity = resultSet.Subset[2];
      Assert.Single(driftQuantity);

      int position = 0;
      foreach (IDrift<double> drift in driftQuantity[0].Results.Values) {
        double x = TestsResultHelper.ResultsHelper(drift, component);
        Assert.Equal(expected[position++], x, 1E-6);
      }
    }

    [Theory]
    //[InlineData(DriftResultVector.X)]
    [InlineData(DriftResultVector.Y)]
    //[InlineData(DriftResultVector.Xy)]
    public void AssemblyDriftIndexValuesFromCombinationCaseTest(DriftResultVector component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblySimple, 1);
      List<double> expected = ExpectedCombinationCaseValues(component);

      // Act
      AssemblyDriftIndices resultSet = result.AssemblyDriftIndices.ResultSubset(new Collection<int>() { 2 });

      // Assert
      IList<IAssemblyQuantity<IDrift<double>>> driftQuantity = resultSet.Subset[2];
      Assert.Single(driftQuantity);

      int position = 0;
      foreach (IDrift<double> drift in driftQuantity[0].Results.Values) {
        double x = TestsResultHelper.ResultsHelper(drift, component);
        Assert.Equal(expected[position++], x, 1E-5);
      }
    }

    private List<double> ExpectedAnalysisCaseValues(DriftResultVector component) {
      switch (component) {
        case DriftResultVector.X: return AssemblyDriftIndicesA1.XInMillimeter();
        case DriftResultVector.Y: return AssemblyDriftIndicesA1.YInMillimeter();
        case DriftResultVector.Xy: return AssemblyDriftIndicesA1.XyInMillimeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseValues(DriftResultVector component) {
      switch (component) {
        case DriftResultVector.X: return AssemblyDriftIndicesC1.XInMillimeter();
        case DriftResultVector.Y: return AssemblyDriftIndicesC1.YInMillimeter();
        case DriftResultVector.Xy: return AssemblyDriftIndicesC1.XyInMillimeter();
      }

      throw new NotImplementedException();
    }
  }
}