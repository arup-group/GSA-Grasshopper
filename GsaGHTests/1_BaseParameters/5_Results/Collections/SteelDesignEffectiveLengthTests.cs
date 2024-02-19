﻿using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class SteelDesignEffectiveLengthTests {

    private static readonly string MemberList = "1";

    [Fact]
    public void SteelDesignEffectiveLengthMember1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      SteelDesignEffectiveLengths resultSet = result.SteelDesignEffectiveLengths.ResultSubset(elementIds);

      // Assert 
      var expectedIds = result.Model.Model.Elements(MemberList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void SteelDesignEffectiveLengthMember1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      SteelDesignEffectiveLengths resultSet = result.SteelDesignEffectiveLengths.ResultSubset(elementIds);

      // Assert 
      var expectedIds = result.Model.Model.Elements(MemberList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void SteelDesignEffectiveLengthValuesFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(MemberList, 1);
      SteelDesignEffectiveLengths resultSet = result.SteelDesignEffectiveLengths.ResultSubset(elementIds);

      // Assert 
      ISteelDesignEffectiveLength effectiveLengths = resultSet.Subset[1][0];

      Assert.Equal(7.5, ResultHelper.RoundToSignificantDigits(effectiveLengths.MemberLength.Value, 4));

      Assert.Equal(0.0, ResultHelper.RoundToSignificantDigits(effectiveLengths.MajorAxisSubSpans[0].StartPosition.Value, 4));
      Assert.Equal(7.5, ResultHelper.RoundToSignificantDigits(effectiveLengths.MajorAxisSubSpans[0].EndPosition.Value, 4));
      Assert.Equal(7.5, ResultHelper.RoundToSignificantDigits(effectiveLengths.MajorAxisSubSpans[0].EffectiveLength.Value, 4));
      Assert.Equal(39.5, ResultHelper.RoundToSignificantDigits(effectiveLengths.MajorAxisSubSpans[0].SlendernessRatio.Value, 4));
      Assert.Single(effectiveLengths.MajorAxisSubSpans[0].ElementIds);
      Assert.Equal(1, effectiveLengths.MajorAxisSubSpans[0].ElementIds[0]);

      Assert.Equal(0.0, ResultHelper.RoundToSignificantDigits(effectiveLengths.MinorAxisSubSpans[0].StartPosition.Value, 4));
      Assert.Equal(7.5, ResultHelper.RoundToSignificantDigits(effectiveLengths.MinorAxisSubSpans[0].EndPosition.Value, 4));
      Assert.Equal(7.5, ResultHelper.RoundToSignificantDigits(effectiveLengths.MinorAxisSubSpans[0].EffectiveLength.Value, 4));
      Assert.Equal(175.0, ResultHelper.RoundToSignificantDigits(effectiveLengths.MinorAxisSubSpans[0].SlendernessRatio.Value, 4));
      Assert.Single(effectiveLengths.MinorAxisSubSpans[0].ElementIds);
      Assert.Equal(1, effectiveLengths.MinorAxisSubSpans[0].ElementIds[0]);

      Assert.Equal(0.0, ResultHelper.RoundToSignificantDigits(effectiveLengths.LateralTorsionalSubSpans[0].StartPosition.Value, 4));
      Assert.Equal(7.5, ResultHelper.RoundToSignificantDigits(effectiveLengths.LateralTorsionalSubSpans[0].EndPosition.Value, 4));
      Assert.Equal(0.0, ResultHelper.RoundToSignificantDigits(effectiveLengths.LateralTorsionalSubSpans[0].EffectiveLength.Value, 4));
      Assert.Equal(0.0, ResultHelper.RoundToSignificantDigits(effectiveLengths.LateralTorsionalSubSpans[0].SlendernessRatio.Value, 4));
      Assert.Single(effectiveLengths.LateralTorsionalSubSpans[0].ElementIds);
      Assert.Equal(1, effectiveLengths.LateralTorsionalSubSpans[0].ElementIds[0]);
    }
  }
}
