﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Element1dDisplacementsTests {

    private static readonly string ElementList = "24 to 30";

    [Fact]
    public void Element1dDisplacementsElement1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dDisplacements.ResultSubset(elementIds, 1);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element1dDisplacementsElement1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dDisplacements.ResultSubset(elementIds, 1);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
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
    public void Element1dDisplacementsMaxFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dDisplacements.ResultSubset(elementIds, 4);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
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
    public void Element1dDisplacementsMaxFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(ExpectedCombinationCaseC4p1Values(component).Max(),
        ExpectedCombinationCaseC4p2Values(component).Max());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dDisplacements.ResultSubset(elementIds, 4);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
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
    public void Element1dDisplacementsMinFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dDisplacements.ResultSubset(elementIds, 4);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
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
    public void Element1dDisplacementsMinFromcombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(ExpectedCombinationCaseC4p1Values(component).Min(),
        ExpectedCombinationCaseC4p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dDisplacements.ResultSubset(elementIds, 4);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
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
    public void Element1dDisplacementsValuesFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);
      int positionsCount = 4;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dDisplacements.ResultSubset(elementIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dQuantity<IDisplacement>> displacementQuantity = resultSet.Subset[id];

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
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void Element1dDisplacementsValuesFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = ExpectedCombinationCaseC4p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC4p2Values(component);
      int positionsCount = 4;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dDisplacements.ResultSubset(elementIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dQuantity<IDisplacement>> displacementQuantity = resultSet.Subset[id];

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

    [Fact]
    public void Element1dDisplacementsCacheChangePositionsTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Element1dDisplacements.ResultSubset(elementIds, positionsCount);

      // Assert
      Assert.Equal(positionsCount,
        result.Element1dDisplacements.Cache.FirstOrDefault().Value.FirstOrDefault().Results.Count);
      Assert.Equal(positionsCount,
        resultSet.Subset.FirstOrDefault().Value.FirstOrDefault().Results.Count);

      // Act again
      int newPositionsCount = 4;
      resultSet = result.Element1dDisplacements.ResultSubset(elementIds, newPositionsCount);

      // Assert again
      Assert.NotEqual(newPositionsCount,
        result.Element1dDisplacements.Cache.FirstOrDefault().Value.FirstOrDefault().Results.Count);
      Assert.Equal(newPositionsCount,
        resultSet.Subset.FirstOrDefault().Value.FirstOrDefault().Results.Count);
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return Element1dDisplacementsA1.XInMillimeter();
        case ResultVector6.Y: return Element1dDisplacementsA1.YInMillimeter();
        case ResultVector6.Z: return Element1dDisplacementsA1.ZInMillimeter();
        case ResultVector6.Xyz: return Element1dDisplacementsA1.XyzInMillimeter();
        case ResultVector6.Xx: return Element1dDisplacementsA1.XxInRadian();
        case ResultVector6.Yy: return Element1dDisplacementsA1.YyInRadian();
        case ResultVector6.Zz: return Element1dDisplacementsA1.ZzInRadian();
        case ResultVector6.Xxyyzz: return Element1dDisplacementsA1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p1Values(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return Element1dDisplacementsC4p1.XInMillimeter();
        case ResultVector6.Y: return Element1dDisplacementsC4p1.YInMillimeter();
        case ResultVector6.Z: return Element1dDisplacementsC4p1.ZInMillimeter();
        case ResultVector6.Xyz: return Element1dDisplacementsC4p1.XyzInMillimeter();
        case ResultVector6.Xx: return Element1dDisplacementsC4p1.XxInRadian();
        case ResultVector6.Yy: return Element1dDisplacementsC4p1.YyInRadian();
        case ResultVector6.Zz: return Element1dDisplacementsC4p1.ZzInRadian();
        case ResultVector6.Xxyyzz: return Element1dDisplacementsC4p1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p2Values(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return Element1dDisplacementsC4p2.XInMillimeter();
        case ResultVector6.Y: return Element1dDisplacementsC4p2.YInMillimeter();
        case ResultVector6.Z: return Element1dDisplacementsC4p2.ZInMillimeter();
        case ResultVector6.Xyz: return Element1dDisplacementsC4p2.XyzInMillimeter();
        case ResultVector6.Xx: return Element1dDisplacementsC4p2.XxInRadian();
        case ResultVector6.Yy: return Element1dDisplacementsC4p2.YyInRadian();
        case ResultVector6.Zz: return Element1dDisplacementsC4p2.ZzInRadian();
        case ResultVector6.Xxyyzz: return Element1dDisplacementsC4p2.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }
  }
}
