using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Parameters.Results;

using GsaGHTests.Helper;
using GsaGHTests.TestHelpers;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class AssemblyByStoreyDisplacementsTests {
    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void AssemblyByStoreyDisplacementsMaxFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblyByStorey, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      AssemblyDisplacements resultSet = result.AssemblyDisplacements.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      DoubleAssertHelper.Equals(expected, max);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void AssemblyByStoreyDisplacementsMaxFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblyByStorey, 1);
      double expected = ExpectedCombinationCaseValues(component).Max();

      // Act
      AssemblyDisplacements resultSet = result.AssemblyDisplacements.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      DoubleAssertHelper.Equals(expected, max);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void AssemblyByStoreyDisplacementsMinFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblyByStorey, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      AssemblyDisplacements resultSet = result.AssemblyDisplacements.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      DoubleAssertHelper.Equals(expected, min);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void AssemblyByStoreyDisplacementsMinFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblyByStorey, 1);
      double expected = ExpectedCombinationCaseValues(component).Min();

      // Act
      AssemblyDisplacements resultSet = result.AssemblyDisplacements.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      DoubleAssertHelper.Equals(expected, min);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void AssemblyByStoreyDisplacementValuesFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblyByStorey, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      AssemblyDisplacements resultSet = result.AssemblyDisplacements.ResultSubset(new Collection<int>() { 2 });

      // Assert
      IList<IEntity1dQuantity<IDisplacement>> displacementQuantity = resultSet.Subset[2];
      Assert.Single(displacementQuantity);

      int position = 0;
      foreach (IDisplacement displacement in displacementQuantity[0].Results.Values) {
        double x = TestsResultHelper.ResultsHelper(displacement, component);
        DoubleAssertHelper.Equals(expected[position++], x);
      }
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void AssemblyByStoreyDisplacementValuesFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblyByStorey, 1);
      List<double> expected = ExpectedCombinationCaseValues(component);

      // Act
      AssemblyDisplacements resultSet = result.AssemblyDisplacements.ResultSubset(new Collection<int>() { 2 });

      // Assert
      IList<IEntity1dQuantity<IDisplacement>> displacementQuantity = resultSet.Subset[2];
      Assert.Single(displacementQuantity);

      int position = 0;
      foreach (IDisplacement displacement in displacementQuantity[0].Results.Values) {
        double x = TestsResultHelper.ResultsHelper(displacement, component);
        DoubleAssertHelper.Equals(expected[position++], x);
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return AssemblyByStoreyDisplacementsA1.XInMillimeter();
        case ResultVector6.Y: return AssemblyByStoreyDisplacementsA1.YInMillimeter();
        case ResultVector6.Z: return AssemblyByStoreyDisplacementsA1.ZInMillimeter();
        case ResultVector6.Xyz: return AssemblyByStoreyDisplacementsA1.XyzInMillimeter();
        case ResultVector6.Xx: return AssemblyByStoreyDisplacementsA1.XxInRadian();
        case ResultVector6.Yy: return AssemblyByStoreyDisplacementsA1.YyInRadian();
        case ResultVector6.Zz: return AssemblyByStoreyDisplacementsA1.ZzInRadian();
        case ResultVector6.Xxyyzz: return AssemblyByStoreyDisplacementsA1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return AssemblyByStoreyDisplacementsC1.XInMillimeter();
        case ResultVector6.Y: return AssemblyByStoreyDisplacementsC1.YInMillimeter();
        case ResultVector6.Z: return AssemblyByStoreyDisplacementsC1.ZInMillimeter();
        case ResultVector6.Xyz: return AssemblyByStoreyDisplacementsC1.XyzInMillimeter();
        case ResultVector6.Xx: return AssemblyByStoreyDisplacementsC1.XxInRadian();
        case ResultVector6.Yy: return AssemblyByStoreyDisplacementsC1.YyInRadian();
        case ResultVector6.Zz: return AssemblyByStoreyDisplacementsC1.ZzInRadian();
        case ResultVector6.Xxyyzz: return AssemblyByStoreyDisplacementsC1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }
  }
}
