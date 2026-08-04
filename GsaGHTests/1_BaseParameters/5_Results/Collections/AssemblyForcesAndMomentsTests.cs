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
  public class AssemblyForcesAndMomentsTests {
    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void AssemblyForcesAndMomentsMaxFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      AssemblyForcesAndMoments resultSet = result.AssemblyForcesAndMoments.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max, DoubleComparer.Default);
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
    public void AssemblyForcesAndMomentsMaxFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedCombinationCaseValues(component).Max();

      // Act
      AssemblyForcesAndMoments resultSet = result.AssemblyForcesAndMoments.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max, DoubleComparer.Default);
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
    public void AssemblyForcesAndMomentsMinFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      AssemblyForcesAndMoments resultSet = result.AssemblyForcesAndMoments.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, DoubleComparer.Default);
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
    public void AssemblyForcesAndMomentsMinFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedCombinationCaseValues(component).Min();

      // Act
      AssemblyForcesAndMoments resultSet = result.AssemblyForcesAndMoments.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, DoubleComparer.Default);
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
    public void AssemblyForcesAndMomentValuesFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      AssemblyForcesAndMoments resultSet = result.AssemblyForcesAndMoments.ResultSubset(new Collection<int>() { 2 });

      // Assert
      IList<IEntity1dQuantity<IInternalForce>> forceQuantity = resultSet.Subset[2];
      Assert.Single(forceQuantity);

      int position = 0;
      foreach (IInternalForce force in forceQuantity[0].Results.Values) {
        double x = TestsResultHelper.ResultsHelper(force, component);
        Assert.Equal(expected[position++], x, DoubleComparer.Default);
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
    public void AssemblyForcesAndMomentValuesFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.AssemblySimple, 1);
      List<double> expected = ExpectedCombinationCaseValues(component);

      // Act
      AssemblyForcesAndMoments resultSet = result.AssemblyForcesAndMoments.ResultSubset(new Collection<int>() { 2 });

      // Assert
      IList<IEntity1dQuantity<IInternalForce>> forceQuantity = resultSet.Subset[2];
      Assert.Single(forceQuantity);

      int position = 0;
      foreach (IInternalForce force in forceQuantity[0].Results.Values) {
        double x = TestsResultHelper.ResultsHelper(force, component);
        Assert.Equal(expected[position++], x, DoubleComparer.Default);
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return AssemblyForcesAndMomentsA1.XInKiloNewton();
        case ResultVector6.Y: return AssemblyForcesAndMomentsA1.YInKiloNewton();
        case ResultVector6.Z: return AssemblyForcesAndMomentsA1.ZInKiloNewton();
        case ResultVector6.Xyz: return AssemblyForcesAndMomentsA1.YzInKiloNewton();
        case ResultVector6.Xx: return AssemblyForcesAndMomentsA1.XxInKiloNewtonMeter();
        case ResultVector6.Yy: return AssemblyForcesAndMomentsA1.YyInKiloNewtonMeter();
        case ResultVector6.Zz: return AssemblyForcesAndMomentsA1.ZzInKiloNewtonMeter();
        case ResultVector6.Xxyyzz: return AssemblyForcesAndMomentsA1.YyzzInKiloNewtonMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return AssemblyForcesAndMomentsC1.XInKiloNewton();
        case ResultVector6.Y: return AssemblyForcesAndMomentsC1.YInKiloNewton();
        case ResultVector6.Z: return AssemblyForcesAndMomentsC1.ZInKiloNewton();
        case ResultVector6.Xyz: return AssemblyForcesAndMomentsC1.YzInKiloNewton();
        case ResultVector6.Xx: return AssemblyForcesAndMomentsC1.XxInKiloNewtonMeter();
        case ResultVector6.Yy: return AssemblyForcesAndMomentsC1.YyInKiloNewtonMeter();
        case ResultVector6.Zz: return AssemblyForcesAndMomentsC1.ZzInKiloNewtonMeter();
        case ResultVector6.Xxyyzz: return AssemblyForcesAndMomentsC1.YyzzInKiloNewtonMeter();
      }

      throw new NotImplementedException();
    }
  }
}
