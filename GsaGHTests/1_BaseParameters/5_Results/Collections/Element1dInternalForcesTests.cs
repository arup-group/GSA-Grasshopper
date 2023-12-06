using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Helpers.Import;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Element1dInternalForcesTests {

    private static readonly string ElementList = "2 to 6";

    [Fact]
    public void Element1dInternalForcesElement1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 1);

      // Assert element IDs
      var expectedIds = result.Model.Model.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element1dInternalForcesElement1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 1);

      // Assert element IDs
      var expectedIds = result.Model.Model.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void Element1dInternalForcesMaxFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void Element1dInternalForcesMaxFromCombinationCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(ExpectedCombinationCaseC4p1Values(component).Max(),
        ExpectedCombinationCaseC4p2Values(component).Max());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void Element1dInternalForcesMinFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void Element1dInternalForcesMinFromcombinationCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(ExpectedCombinationCaseC4p1Values(component).Min(),
        ExpectedCombinationCaseC4p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, 5);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void Element1dInternalForcesValuesFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dInternalForce> displacementQuantity = resultSet.Subset[id];

        // for analysis case results we expect 4 positions
        Assert.Single(displacementQuantity);
        var positions = Enumerable.Range(0, positionsCount).Select(
        k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double x = TestsResultHelper.ResultsHelper(displacementQuantity[0].Results[position], component);
          Assert.Equal(expected[i++], x);
        }
      }
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void Element1dInternalForcesValuesFromCombinationCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = ExpectedCombinationCaseC4p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC4p2Values(component);
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dInternalForces.ResultSubset(elementIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dInternalForce> displacementQuantity = resultSet.Subset[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, displacementQuantity.Count);

        var positions = Enumerable.Range(0, positionsCount).Select(
        k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double perm1 = TestsResultHelper.ResultsHelper(displacementQuantity[0].Results[position], component);
          Assert.Equal(expectedP1[i], perm1);
          double perm2 = TestsResultHelper.ResultsHelper(displacementQuantity[1].Results[position], component);
          Assert.Equal(expectedP2[i++], perm2);
        }
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Element1dForcesAndMomentsA1.XInKiloNewton();

        case ResultVector6HelperEnum.Y: return Element1dForcesAndMomentsA1.YInKiloNewton();

        case ResultVector6HelperEnum.Z: return Element1dForcesAndMomentsA1.ZInKiloNewton();

        case ResultVector6HelperEnum.Xyz: return Element1dForcesAndMomentsA1.XyzInKiloNewton();

        case ResultVector6HelperEnum.Xx: return Element1dForcesAndMomentsA1.XxInKiloNewtonMeter();

        case ResultVector6HelperEnum.Yy: return Element1dForcesAndMomentsA1.YyInKiloNewtonMeter();

        case ResultVector6HelperEnum.Zz: return Element1dForcesAndMomentsA1.ZzInKiloNewtonMeter();

        case ResultVector6HelperEnum.Xxyyzz: return Element1dForcesAndMomentsA1.XxyyzzInKiloNewtonMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p1Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Element1dForcesAndMomentsC4p1.XInKiloNewton();

        case ResultVector6HelperEnum.Y: return Element1dForcesAndMomentsC4p1.YInKiloNewton();

        case ResultVector6HelperEnum.Z: return Element1dForcesAndMomentsC4p1.ZInKiloNewton();

        case ResultVector6HelperEnum.Xyz: return Element1dForcesAndMomentsC4p1.XyzInKiloNewton();

        case ResultVector6HelperEnum.Xx: return Element1dForcesAndMomentsC4p1.XxInKiloNewtonMeter();

        case ResultVector6HelperEnum.Yy: return Element1dForcesAndMomentsC4p1.YyInKiloNewtonMeter();

        case ResultVector6HelperEnum.Zz: return Element1dForcesAndMomentsC4p1.ZzInKiloNewtonMeter();

        case ResultVector6HelperEnum.Xxyyzz: return Element1dForcesAndMomentsC4p1.XxyyzzInKiloNewtonMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p2Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Element1dForcesAndMomentsC4p2.XInKiloNewton();

        case ResultVector6HelperEnum.Y: return Element1dForcesAndMomentsC4p2.YInKiloNewton();

        case ResultVector6HelperEnum.Z: return Element1dForcesAndMomentsC4p2.ZInKiloNewton();

        case ResultVector6HelperEnum.Xyz: return Element1dForcesAndMomentsC4p2.XyzInKiloNewton();

        case ResultVector6HelperEnum.Xx: return Element1dForcesAndMomentsC4p2.XxInKiloNewtonMeter();

        case ResultVector6HelperEnum.Yy: return Element1dForcesAndMomentsC4p2.YyInKiloNewtonMeter();

        case ResultVector6HelperEnum.Zz: return Element1dForcesAndMomentsC4p2.ZzInKiloNewtonMeter();

        case ResultVector6HelperEnum.Xxyyzz: return Element1dForcesAndMomentsC4p2.XxyyzzInKiloNewtonMeter();
      }

      throw new NotImplementedException();
    }
  }
}
