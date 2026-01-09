using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Helpers;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Element1dStrainEnergyDensityTests {

    private static readonly string ElementList = "2 to 3";

    [Fact]
    public void Element1dStrainEnergyDensityElement1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEnergyDensity, Entity1dExtremaKey> resultSet
        = result.Element1dStrainEnergyDensities.ResultSubset(elementIds, 1);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element1dStrainEnergyDensitysElement1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEnergyDensity, Entity1dExtremaKey> resultSet
        = result.Element1dStrainEnergyDensities.ResultSubset(elementIds, 1);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element1dStrainEnergyDensitysMaxFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = Element1dStrainEnergyDensity.A1EnergyInkJ().Max();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEnergyDensity, Entity1dExtremaKey> resultSet
        = result.Element1dStrainEnergyDensities.ResultSubset(elementIds, 5);

      // Assert Max in set
      double max = resultSet.GetExtrema(resultSet.Max).EnergyDensity.Kilojoules;
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Fact]
    public void Element1dStrainEnergyDensitysMaxFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(Element1dStrainEnergyDensity.C4p1EnergyInkJ().Max(),
        Element1dStrainEnergyDensity.C4p2EnergyInkJ().Max());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEnergyDensity, Entity1dExtremaKey> resultSet
        = result.Element1dStrainEnergyDensities.ResultSubset(elementIds, 5);

      // Assert Max in set
      double max = resultSet.GetExtrema(resultSet.Max).EnergyDensity.Kilojoules;
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Fact]
    public void Element1dStrainEnergyDensitysMinFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = Element1dStrainEnergyDensity.A1EnergyInkJ().Min();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEnergyDensity, Entity1dExtremaKey> resultSet
        = result.Element1dStrainEnergyDensities.ResultSubset(elementIds, 5);

      // Assert Max in set
      double min = resultSet.GetExtrema(resultSet.Min).EnergyDensity.Kilojoules;
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Fact]
    public void Element1dStrainEnergyDensitysMinFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(Element1dStrainEnergyDensity.C4p1EnergyInkJ().Min(),
        Element1dStrainEnergyDensity.C4p2EnergyInkJ().Min());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEnergyDensity, Entity1dExtremaKey> resultSet
        = result.Element1dStrainEnergyDensities.ResultSubset(elementIds, 5);

      // Assert Max in set
      double min = resultSet.GetExtrema(resultSet.Min).EnergyDensity.Kilojoules;
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Fact]
    public void Element1dStrainEnergyDensitysValuesFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = Element1dStrainEnergyDensity.A1EnergyInkJ();
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEnergyDensity, Entity1dExtremaKey> resultSet
        = result.Element1dStrainEnergyDensities.ResultSubset(elementIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dQuantity<IEnergyDensity>> strainEnergy = resultSet.Subset[id];

        // for analysis case results we expect 4 positions
        Assert.Single(strainEnergy);
        var positions = Enumerable.Range(0, positionsCount).Select(
        k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double value = strainEnergy[0].Results[position].EnergyDensity.Kilojoules;
          Assert.Equal(expected[i++], value, DoubleComparer.Default);
        }
      }
    }

    [Fact]
    public void Element1dStrainEnergyDensitysValuesFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = Element1dStrainEnergyDensity.C4p1EnergyInkJ();
      List<double> expectedP2 = Element1dStrainEnergyDensity.C4p2EnergyInkJ();
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEnergyDensity, Entity1dExtremaKey> resultSet
        = result.Element1dStrainEnergyDensities.ResultSubset(elementIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dQuantity<IEnergyDensity>> strainEnergy = resultSet.Subset[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, strainEnergy.Count);

        var positions = Enumerable.Range(0, positionsCount).Select(
        k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double perm1 = strainEnergy[0].Results[position].EnergyDensity.Kilojoules;
          Assert.Equal(expectedP1[i], perm1, DoubleComparer.Default);
          double perm2 = strainEnergy[1].Results[position].EnergyDensity.Kilojoules;
          Assert.Equal(expectedP2[i++], perm2, DoubleComparer.Default);
        }
      }
    }
  }
}
