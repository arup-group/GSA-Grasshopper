using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using OasysUnits;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class SteelUtilisationsTests {

    private static readonly string MemberList = "1";

    [Fact]
    public void SteelUtilsationMember1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      IEntity0dResultSubset<ISteelUtilisation, Entity0dExtremaKey> resultSet
        = result.SteelUtilisations.ResultSubset(elementIds);

      // Assert 
      var expectedIds = result.Model.Model.Elements(MemberList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

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
    public void SteelUtilsationMaxFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = 1;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      IEntity0dResultSubset<ISteelUtilisation, Entity0dExtremaKey> resultSet
        = result.SteelUtilisations.ResultSubset(elementIds);

      // Assert
      double max = ((Ratio)resultSet.GetExtrema(resultSet.Max).Overall).DecimalFractions;
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Fact]
    public void SteelUtilsationMinFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = 1;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      IEntity0dResultSubset<ISteelUtilisation, Entity0dExtremaKey> resultSet
        = result.SteelUtilisations.ResultSubset(elementIds);

      // Assert
      double min = ((Ratio)resultSet.GetExtrema(resultSet.Min).Overall).DecimalFractions;
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    [Fact]
    public void SteelUtilsationValuesFromCombinationCaseTest() {
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

        double perm1 = ((Ratio)utilisations[0].Overall).DecimalFractions;
        Assert.Equal(expectedP1[i], ResultHelper.RoundToSignificantDigits(perm1, 4));
        double perm2 = ((Ratio)utilisations[1].Overall).DecimalFractions;
        Assert.Equal(expectedP2[i++], ResultHelper.RoundToSignificantDigits(perm2, 4));
      }
    }
  }
}
