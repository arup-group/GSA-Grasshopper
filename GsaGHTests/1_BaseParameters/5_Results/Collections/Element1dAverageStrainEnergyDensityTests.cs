using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Element1dAverageStrainEnergyDensityTests {

    private static readonly string ElementList = "2 to 38";

    [Fact]
    public void Element1dAverageStrainEnergyDensityElement1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity0dResultSubset<IEnergyDensity, Entity0dExtremaKey> resultSet
        = result.Element1dAverageStrainEnergyDensities.ResultSubset(elementIds);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element1dAverageStrainEnergyDensitysElement1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity0dResultSubset<IEnergyDensity, Entity0dExtremaKey> resultSet
        = result.Element1dAverageStrainEnergyDensities.ResultSubset(elementIds);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element1dAverageStrainEnergyDensitysMaxFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = Element1dAverageStrainEnergyDensity.A1EnergyInkJ().Max();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity0dResultSubset<IEnergyDensity, Entity0dExtremaKey> resultSet
        = result.Element1dAverageStrainEnergyDensities.ResultSubset(elementIds);

      // Assert Max in set
      double max = resultSet.GetExtrema(resultSet.Max).EnergyDensity.Kilojoules;
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Fact]
    public void Element1dAverageStrainEnergyDensitysMaxFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(
        Element1dAverageStrainEnergyDensity.C4p1EnergyInkJ().Max(),
        Element1dAverageStrainEnergyDensity.C4p2EnergyInkJ().Max());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity0dResultSubset<IEnergyDensity, Entity0dExtremaKey> resultSet
        = result.Element1dAverageStrainEnergyDensities.ResultSubset(elementIds);

      // Assert Max in set
      double max = resultSet.GetExtrema(resultSet.Max).EnergyDensity.Kilojoules;
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Fact]
    public void Element1dAverageStrainEnergyDensitysMinFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = Element1dAverageStrainEnergyDensity.A1EnergyInkJ().Min();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity0dResultSubset<IEnergyDensity, Entity0dExtremaKey> resultSet
        = result.Element1dAverageStrainEnergyDensities.ResultSubset(elementIds);

      // Assert Max in set
      double min = resultSet.GetExtrema(resultSet.Min).EnergyDensity.Kilojoules;
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    [Fact]
    public void Element1dAverageStrainEnergyDensitysMinFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(
        Element1dAverageStrainEnergyDensity.C4p1EnergyInkJ().Min(),
        Element1dAverageStrainEnergyDensity.C4p2EnergyInkJ().Min());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity0dResultSubset<IEnergyDensity, Entity0dExtremaKey> resultSet
        = result.Element1dAverageStrainEnergyDensities.ResultSubset(elementIds);

      // Assert Max in set
      double min = resultSet.GetExtrema(resultSet.Min).EnergyDensity.Kilojoules;
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    [Fact]
    public void Element1dAverageStrainEnergyDensitysValuesFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = Element1dAverageStrainEnergyDensity.A1EnergyInkJ();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity0dResultSubset<IEnergyDensity, Entity0dExtremaKey> resultSet
        = result.Element1dAverageStrainEnergyDensities.ResultSubset(elementIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEnergyDensity> strainEnergy = resultSet.Subset[id];

        // for analysis case results we expect 4 positions
        Assert.Single(strainEnergy);
        double value = strainEnergy[0].EnergyDensity.Kilojoules;
        Assert.Equal(expected[i++], ResultHelper.RoundToSignificantDigits(value, 4));
      }
    }

    [Fact]
    public void Element1dAverageStrainEnergyDensitysValuesFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = Element1dAverageStrainEnergyDensity.C4p1EnergyInkJ();
      List<double> expectedP2 = Element1dAverageStrainEnergyDensity.C4p2EnergyInkJ();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity0dResultSubset<IEnergyDensity, Entity0dExtremaKey> resultSet
        = result.Element1dAverageStrainEnergyDensities.ResultSubset(elementIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEnergyDensity> strainEnergy = resultSet.Subset[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, strainEnergy.Count);

        double perm1 = strainEnergy[0].EnergyDensity.Kilojoules;
        Assert.Equal(expectedP1[i], ResultHelper.RoundToSignificantDigits(perm1, 4));
        double perm2 = strainEnergy[1].EnergyDensity.Kilojoules;
        Assert.Equal(expectedP2[i++], ResultHelper.RoundToSignificantDigits(perm2, 4));
      }
    }
  }
}
