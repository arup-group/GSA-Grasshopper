using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Helpers;
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
      IEntity0dResultSubset<ISteelUtilisation, SteelUtilisationExtremaKeys> resultSet
        = result.SteelUtilisations.ResultSubset(elementIds);

      // Assert
      var expectedIds = result.Model.ApiModel.Elements(MemberList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void SteelUtilsationMember1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      IEntity0dResultSubset<ISteelUtilisation, SteelUtilisationExtremaKeys> resultSet
        = result.SteelUtilisations.ResultSubset(elementIds);

      // Assert
      var expectedIds = result.Model.ApiModel.Elements(MemberList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void SteelUtilsationMaxFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      IEntity0dResultSubset<ISteelUtilisation, SteelUtilisationExtremaKeys> resultSet
        = result.SteelUtilisations.ResultSubset(elementIds);

      // Assert
      double overall = ((Ratio)resultSet.GetExtrema(resultSet.Max.Overall).Overall).DecimalFractions;
      double localCombined = ((Ratio)resultSet.GetExtrema(resultSet.Max.LocalCombined).LocalCombined).DecimalFractions;
      double bucklingCombined = ((Ratio)resultSet.GetExtrema(resultSet.Max.BucklingCombined).BucklingCombined).DecimalFractions;
      double localAxial = ((Ratio)resultSet.GetExtrema(resultSet.Max.LocalAxial).LocalAxial).DecimalFractions;
      double localShearU = ((Ratio)resultSet.GetExtrema(resultSet.Max.LocalShearU).LocalShearU).DecimalFractions;
      double localShearV = ((Ratio)resultSet.GetExtrema(resultSet.Max.LocalShearV).LocalShearV).DecimalFractions;
      double localTorsion = ((Ratio)resultSet.GetExtrema(resultSet.Max.LocalTorsion).LocalTorsion).DecimalFractions;
      double localMajorMoment = ((Ratio)resultSet.GetExtrema(resultSet.Max.LocalMajorMoment).LocalMajorMoment).DecimalFractions;
      double localMinorMoment = ((Ratio)resultSet.GetExtrema(resultSet.Max.LocalMinorMoment).LocalMinorMoment).DecimalFractions;
      double majorBuckling = ((Ratio)resultSet.GetExtrema(resultSet.Max.MajorBuckling).MajorBuckling).DecimalFractions;
      double minorBuckling = ((Ratio)resultSet.GetExtrema(resultSet.Max.MinorBuckling).MinorBuckling).DecimalFractions;
      double lateralTorsionalBuckling = ((Ratio)resultSet.GetExtrema(resultSet.Max.LateralTorsionalBuckling).LateralTorsionalBuckling).DecimalFractions;
      Assert.Equal(0.1499, overall, DoubleComparer.Default);
      Assert.Equal(0.1499, localCombined, DoubleComparer.Default);
      Assert.Equal(0.1421, bucklingCombined, DoubleComparer.Default);
      Assert.Equal(0.0, localAxial, DoubleComparer.Default);
      Assert.Equal(0.0, localShearU, DoubleComparer.Default);
      Assert.Equal(0.04847, localShearV, DoubleComparer.Default);
      Assert.Equal(0.0, localTorsion, DoubleComparer.Default);
      Assert.Equal(0.1499, localMajorMoment, DoubleComparer.Default);
      Assert.Equal(0.0, localMinorMoment, DoubleComparer.Default);
      Assert.Equal(0.0, majorBuckling, DoubleComparer.Default);
      Assert.Equal(0.0, minorBuckling, DoubleComparer.Default);
      Assert.Equal(0.0, lateralTorsionalBuckling, DoubleComparer.Default);
      Assert.Null(resultSet.GetExtrema(resultSet.Max.TorsionalBuckling).TorsionalBuckling);
      Assert.Null(resultSet.GetExtrema(resultSet.Max.FlexuralBuckling).FlexuralBuckling);
    }

    [Fact]
    public void SteelUtilsationMaxFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      IEntity0dResultSubset<ISteelUtilisation, SteelUtilisationExtremaKeys> resultSet
        = result.SteelUtilisations.ResultSubset(elementIds);

      // Assert
      double overall = ((Ratio)resultSet.GetExtrema(resultSet.Max.Overall).Overall).DecimalFractions;
      double localCombined = ((Ratio)resultSet.GetExtrema(resultSet.Max.LocalCombined).LocalCombined).DecimalFractions;
      double bucklingCombined = ((Ratio)resultSet.GetExtrema(resultSet.Max.BucklingCombined).BucklingCombined).DecimalFractions;
      double localAxial = ((Ratio)resultSet.GetExtrema(resultSet.Max.LocalAxial).LocalAxial).DecimalFractions;
      double localShearU = ((Ratio)resultSet.GetExtrema(resultSet.Max.LocalShearU).LocalShearU).DecimalFractions;
      double localShearV = ((Ratio)resultSet.GetExtrema(resultSet.Max.LocalShearV).LocalShearV).DecimalFractions;
      double localTorsion = ((Ratio)resultSet.GetExtrema(resultSet.Max.LocalTorsion).LocalTorsion).DecimalFractions;
      double localMajorMoment = ((Ratio)resultSet.GetExtrema(resultSet.Max.LocalMajorMoment).LocalMajorMoment).DecimalFractions;
      double localMinorMoment = ((Ratio)resultSet.GetExtrema(resultSet.Max.LocalMinorMoment).LocalMinorMoment).DecimalFractions;
      double majorBuckling = ((Ratio)resultSet.GetExtrema(resultSet.Max.MajorBuckling).MajorBuckling).DecimalFractions;
      double minorBuckling = ((Ratio)resultSet.GetExtrema(resultSet.Max.MinorBuckling).MinorBuckling).DecimalFractions;
      double lateralTorsionalBuckling = ((Ratio)resultSet.GetExtrema(resultSet.Max.LateralTorsionalBuckling).LateralTorsionalBuckling).DecimalFractions;
      Assert.Equal(0.4017, overall, DoubleComparer.Default);
      Assert.Equal(0.4017, localCombined, DoubleComparer.Default);
      Assert.Equal(0.3716, bucklingCombined, DoubleComparer.Default);
      Assert.Equal(0.0, localAxial, DoubleComparer.Default);
      Assert.Equal(0.0, localShearU, DoubleComparer.Default);
      Assert.Equal(0.09888, localShearV, DoubleComparer.Default);
      Assert.Equal(0.0, localTorsion, DoubleComparer.Default);
      Assert.Equal(0.4017, localMajorMoment, DoubleComparer.Default);
      Assert.Equal(0.0, localMinorMoment, DoubleComparer.Default);
      Assert.Equal(0.0, majorBuckling, DoubleComparer.Default);
      Assert.Equal(0.0, minorBuckling, DoubleComparer.Default);
      Assert.Equal(0.0, lateralTorsionalBuckling, DoubleComparer.Default);
      Assert.Null(resultSet.GetExtrema(resultSet.Max.TorsionalBuckling).TorsionalBuckling);
      Assert.Null(resultSet.GetExtrema(resultSet.Max.FlexuralBuckling).FlexuralBuckling);
    }

    [Fact]
    public void SteelUtilsationMinFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 2);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      IEntity0dResultSubset<ISteelUtilisation, SteelUtilisationExtremaKeys> resultSet
        = result.SteelUtilisations.ResultSubset(elementIds);

      // Assert
      double overall = ((Ratio)resultSet.GetExtrema(resultSet.Min.Overall).Overall).DecimalFractions;
      double localCombined = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalCombined).LocalCombined).DecimalFractions;
      double bucklingCombined = ((Ratio)resultSet.GetExtrema(resultSet.Min.BucklingCombined).BucklingCombined).DecimalFractions;
      double localAxial = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalAxial).LocalAxial).DecimalFractions;
      double localShearU = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalShearU).LocalShearU).DecimalFractions;
      double localShearV = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalShearV).LocalShearV).DecimalFractions;
      double localTorsion = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalTorsion).LocalTorsion).DecimalFractions;
      double localMajorMoment = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalMajorMoment).LocalMajorMoment).DecimalFractions;
      double localMinorMoment = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalMinorMoment).LocalMinorMoment).DecimalFractions;
      double majorBuckling = ((Ratio)resultSet.GetExtrema(resultSet.Min.MajorBuckling).MajorBuckling).DecimalFractions;
      double minorBuckling = ((Ratio)resultSet.GetExtrema(resultSet.Min.MinorBuckling).MinorBuckling).DecimalFractions;
      double lateralTorsionalBuckling = ((Ratio)resultSet.GetExtrema(resultSet.Min.LateralTorsionalBuckling).LateralTorsionalBuckling).DecimalFractions;
      Assert.Equal(0.1199, overall, DoubleComparer.Default);
      Assert.Equal(0.1199, localCombined, DoubleComparer.Default);
      Assert.Equal(0.1079, bucklingCombined, DoubleComparer.Default);
      Assert.Equal(0.0, localAxial, DoubleComparer.Default);
      Assert.Equal(0.0, localShearU, DoubleComparer.Default);
      Assert.Equal(0.01939, localShearV, DoubleComparer.Default);
      Assert.Equal(0.0, localTorsion, DoubleComparer.Default);
      Assert.Equal(0.1199, localMajorMoment, DoubleComparer.Default);
      Assert.Equal(0.0, localMinorMoment, DoubleComparer.Default);
      Assert.Equal(0.0, majorBuckling, DoubleComparer.Default);
      Assert.Equal(0.0, minorBuckling, DoubleComparer.Default);
      Assert.Equal(0.0, lateralTorsionalBuckling, DoubleComparer.Default);
      Assert.Null(resultSet.GetExtrema(resultSet.Min.TorsionalBuckling).TorsionalBuckling);
      Assert.Null(resultSet.GetExtrema(resultSet.Min.FlexuralBuckling).FlexuralBuckling);
    }

    [Fact]
    public void SteelUtilsationMinFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      IEntity0dResultSubset<ISteelUtilisation, SteelUtilisationExtremaKeys> resultSet
        = result.SteelUtilisations.ResultSubset(elementIds);

      // Assert
      double overall = ((Ratio)resultSet.GetExtrema(resultSet.Min.Overall).Overall).DecimalFractions;
      double localCombined = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalCombined).LocalCombined).DecimalFractions;
      double bucklingCombined = ((Ratio)resultSet.GetExtrema(resultSet.Min.BucklingCombined).BucklingCombined).DecimalFractions;
      double localAxial = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalAxial).LocalAxial).DecimalFractions;
      double localShearU = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalShearU).LocalShearU).DecimalFractions;
      double localShearV = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalShearV).LocalShearV).DecimalFractions;
      double localTorsion = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalTorsion).LocalTorsion).DecimalFractions;
      double localMajorMoment = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalMajorMoment).LocalMajorMoment).DecimalFractions;
      double localMinorMoment = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalMinorMoment).LocalMinorMoment).DecimalFractions;
      double majorBuckling = ((Ratio)resultSet.GetExtrema(resultSet.Min.MajorBuckling).MajorBuckling).DecimalFractions;
      double minorBuckling = ((Ratio)resultSet.GetExtrema(resultSet.Min.MinorBuckling).MinorBuckling).DecimalFractions;
      double lateralTorsionalBuckling = ((Ratio)resultSet.GetExtrema(resultSet.Min.LateralTorsionalBuckling).LateralTorsionalBuckling).DecimalFractions;
      Assert.Equal(0.4017, overall, DoubleComparer.Default);
      Assert.Equal(0.4017, localCombined, DoubleComparer.Default);
      Assert.Equal(0.3716, bucklingCombined, DoubleComparer.Default);
      Assert.Equal(0.0, localAxial, DoubleComparer.Default);
      Assert.Equal(0.0, localShearU, DoubleComparer.Default);
      Assert.Equal(0.09888, localShearV, DoubleComparer.Default);
      Assert.Equal(0.0, localTorsion, DoubleComparer.Default);
      Assert.Equal(0.4017, localMajorMoment, DoubleComparer.Default);
      Assert.Equal(0.0, localMinorMoment, DoubleComparer.Default);
      Assert.Equal(0.0, majorBuckling, DoubleComparer.Default);
      Assert.Equal(0.0, minorBuckling, DoubleComparer.Default);
      Assert.Equal(0.0, lateralTorsionalBuckling, DoubleComparer.Default);
      Assert.Null(resultSet.GetExtrema(resultSet.Max.TorsionalBuckling).TorsionalBuckling);
      Assert.Null(resultSet.GetExtrema(resultSet.Max.FlexuralBuckling).FlexuralBuckling);
    }

    [Fact]
    public void SteelUtilsationValuesFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      IEntity0dResultSubset<ISteelUtilisation, SteelUtilisationExtremaKeys> resultSet
        = result.SteelUtilisations.ResultSubset(elementIds);

      // Assert
      IList<ISteelUtilisation> utilisations = resultSet.Subset[1];
      double overall = ((Ratio)resultSet.GetExtrema(resultSet.Min.Overall).Overall).DecimalFractions;
      double localCombined = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalCombined).LocalCombined).DecimalFractions;
      double bucklingCombined = ((Ratio)resultSet.GetExtrema(resultSet.Min.BucklingCombined).BucklingCombined).DecimalFractions;
      double localAxial = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalAxial).LocalAxial).DecimalFractions;
      double localShearU = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalShearU).LocalShearU).DecimalFractions;
      double localShearV = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalShearV).LocalShearV).DecimalFractions;
      double localTorsion = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalTorsion).LocalTorsion).DecimalFractions;
      double localMajorMoment = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalMajorMoment).LocalMajorMoment).DecimalFractions;
      double localMinorMoment = ((Ratio)resultSet.GetExtrema(resultSet.Min.LocalMinorMoment).LocalMinorMoment).DecimalFractions;
      double majorBuckling = ((Ratio)resultSet.GetExtrema(resultSet.Min.MajorBuckling).MajorBuckling).DecimalFractions;
      double minorBuckling = ((Ratio)resultSet.GetExtrema(resultSet.Min.MinorBuckling).MinorBuckling).DecimalFractions;
      double lateralTorsionalBuckling = ((Ratio)resultSet.GetExtrema(resultSet.Min.LateralTorsionalBuckling).LateralTorsionalBuckling).DecimalFractions;
      Assert.Equal(0.4017, overall, DoubleComparer.Default);
      Assert.Equal(0.4017, localCombined, DoubleComparer.Default);
      Assert.Equal(0.3716, bucklingCombined, DoubleComparer.Default);
      Assert.Equal(0.0, localAxial, DoubleComparer.Default);
      Assert.Equal(0.0, localShearU, DoubleComparer.Default);
      Assert.Equal(0.09888, localShearV, DoubleComparer.Default);
      Assert.Equal(0.0, localTorsion, DoubleComparer.Default);
      Assert.Equal(0.4017, localMajorMoment, DoubleComparer.Default);
      Assert.Equal(0.0, localMinorMoment, DoubleComparer.Default);
      Assert.Equal(0.0, majorBuckling, DoubleComparer.Default);
      Assert.Equal(0.0, minorBuckling, DoubleComparer.Default);
      Assert.Equal(0.0, lateralTorsionalBuckling, DoubleComparer.Default);
      Assert.Null(resultSet.GetExtrema(resultSet.Max.TorsionalBuckling).TorsionalBuckling);
      Assert.Null(resultSet.GetExtrema(resultSet.Max.FlexuralBuckling).FlexuralBuckling);
    }

    [Fact]
    public void UpdateExtremaTest() {
      // Assemble
      var maxValue = new SteelUtilisation(0.0);
      var minValue = new SteelUtilisation(0.0);
      var maxKeys = new SteelUtilisationExtremaKeys();
      var minKeys = new SteelUtilisationExtremaKeys();

      // Act
      ExtremaKeyUtility.UpdateExtrema(new SteelUtilisation(0.5), 1, 0, ref maxValue, ref minValue, ref maxKeys, ref minKeys);
      ExtremaKeyUtility.UpdateExtrema(new SteelUtilisation(-0.5), 1, 0, ref maxValue, ref minValue, ref maxKeys, ref minKeys);
      ExtremaKeyUtility.UpdateExtrema(new SteelUtilisation(1.0), 1, 0, ref maxValue, ref minValue, ref maxKeys, ref minKeys);
      ExtremaKeyUtility.UpdateExtrema(new SteelUtilisation(-1.0), 2, 0, ref maxValue, ref minValue, ref maxKeys, ref minKeys);

      // Assert
      Assert.Equal(1.0, maxValue.Overall.Value.DecimalFractions);
      Assert.Equal(1.0, maxValue.LocalCombined.Value.DecimalFractions);
      Assert.Equal(1.0, maxValue.BucklingCombined.Value.DecimalFractions);
      Assert.Equal(1.0, maxValue.LocalAxial.Value.DecimalFractions);
      Assert.Equal(1.0, maxValue.LocalShearU.Value.DecimalFractions);
      Assert.Equal(1.0, maxValue.LocalShearV.Value.DecimalFractions);
      Assert.Equal(1.0, maxValue.LocalTorsion.Value.DecimalFractions);
      Assert.Equal(1.0, maxValue.LocalMajorMoment.Value.DecimalFractions);
      Assert.Equal(1.0, maxValue.LocalMinorMoment.Value.DecimalFractions);
      Assert.Equal(1.0, maxValue.MajorBuckling.Value.DecimalFractions);
      Assert.Equal(1.0, maxValue.MinorBuckling.Value.DecimalFractions);
      Assert.Equal(1.0, maxValue.LateralTorsionalBuckling.Value.DecimalFractions);
      Assert.Equal(1.0, maxValue.TorsionalBuckling.Value.DecimalFractions);
      Assert.Equal(1.0, maxValue.FlexuralBuckling.Value.DecimalFractions);
      Assert.Equal(1, maxKeys.Overall.Id);
      Assert.Equal(0, maxKeys.Overall.Permutation);
      Assert.Equal(-1.0, minValue.Overall.Value.DecimalFractions);
      Assert.Equal(-1.0, minValue.LocalCombined.Value.DecimalFractions);
      Assert.Equal(-1.0, minValue.BucklingCombined.Value.DecimalFractions);
      Assert.Equal(-1.0, minValue.LocalAxial.Value.DecimalFractions);
      Assert.Equal(-1.0, minValue.LocalShearU.Value.DecimalFractions);
      Assert.Equal(-1.0, minValue.LocalShearV.Value.DecimalFractions);
      Assert.Equal(-1.0, minValue.LocalTorsion.Value.DecimalFractions);
      Assert.Equal(-1.0, minValue.LocalMajorMoment.Value.DecimalFractions);
      Assert.Equal(-1.0, minValue.LocalMinorMoment.Value.DecimalFractions);
      Assert.Equal(-1.0, minValue.MajorBuckling.Value.DecimalFractions);
      Assert.Equal(-1.0, minValue.MinorBuckling.Value.DecimalFractions);
      Assert.Equal(-1.0, minValue.LateralTorsionalBuckling.Value.DecimalFractions);
      Assert.Equal(-1.0, minValue.TorsionalBuckling.Value.DecimalFractions);
      Assert.Equal(-1.0, minValue.FlexuralBuckling.Value.DecimalFractions);
      Assert.Equal(2, minKeys.Overall.Id);
      Assert.Equal(0, minKeys.Overall.Permutation);
    }

    [Fact]
    public void ShouldFindTheLargestAndSmallest() {
      var dictionary = new Dictionary<int, IList<SteelUtilisation>>() {
        {
          3, new List<SteelUtilisation>() {
            new SteelUtilisation(0),
          }
        }, {
          2, new List<SteelUtilisation>() {
            new SteelUtilisation(1),
          }
        },
      };
      (SteelUtilisationExtremaKeys max, SteelUtilisationExtremaKeys min) = dictionary.GetSteelUtilisationExtremaKeys();
      Assert.Equal(2, max.Overall.Id);
      Assert.Equal(3, min.Overall.Id);
    }

    [Fact]
    public void ShouldLookAtAllItemsOnTheList() {
      var dictionary = new Dictionary<int, IList<SteelUtilisation>>() {
        {
          3, new List<SteelUtilisation>() {
            new SteelUtilisation(1),
          }
        }, {
          2, new List<SteelUtilisation>() {
            new SteelUtilisation(0),
            new SteelUtilisation(2),
          }
        },
      };
      (SteelUtilisationExtremaKeys max, SteelUtilisationExtremaKeys min) = dictionary.GetSteelUtilisationExtremaKeys();
      Assert.Equal(2, max.Overall.Id);
      Assert.Equal(2, min.Overall.Id);
    }
  }
}
