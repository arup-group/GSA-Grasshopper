using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class SteelUtilisationsTests {

    private static readonly string MemberList = "1";

    [Fact]
    public void SteelUtilsationMember1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      IEntity0dResultSubset<ISteelUtilisation, Entity0dExtremaKey> resultSet
        = result.SteelUtilisations.ResultSubset(elementIds);

      // Assert 
      var expectedIds = result.Model.Model.Elements(MemberList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element1dAverageStrainEnergyDensitysMaxFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = 1;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      IEntity0dResultSubset<ISteelUtilisation, Entity0dExtremaKey> resultSet
        = result.SteelUtilisations.ResultSubset(elementIds);

      // Assert
      double max = resultSet.GetExtrema(resultSet.Max).Overall.DecimalFractions;
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Fact]
    public void Element1dAverageStrainEnergyDensitysMinFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = 1;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      IEntity0dResultSubset<ISteelUtilisation, Entity0dExtremaKey> resultSet
        = result.SteelUtilisations.ResultSubset(elementIds);

      // Assert
      double min = resultSet.GetExtrema(resultSet.Min).Overall.DecimalFractions;
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    [Fact]
    public void Element1dAverageStrainEnergyDensitysValuesFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);
      List<double> expectedP1 = Element1dAverageStrainEnergyDensity.C4p1EnergyInkJ();
      List<double> expectedP2 = Element1dAverageStrainEnergyDensity.C4p2EnergyInkJ();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      IEntity0dResultSubset<ISteelUtilisation, Entity0dExtremaKey> resultSet
        = result.SteelUtilisations.ResultSubset(elementIds);

      // Assert 
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<ISteelUtilisation> utilisations = resultSet.Subset[id];

        Assert.Equal(2, utilisations.Count);

        double perm1 = utilisations[0].Overall.DecimalFractions;
        Assert.Equal(expectedP1[i], ResultHelper.RoundToSignificantDigits(perm1, 4));
        double perm2 = utilisations[1].Overall.DecimalFractions;
        Assert.Equal(expectedP2[i++], ResultHelper.RoundToSignificantDigits(perm2, 4));
      }
    }
  }
}
