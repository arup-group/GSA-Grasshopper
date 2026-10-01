using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters.Results;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

using SteelDesignEffectiveLength = GsaGH.Components.SteelDesignEffectiveLength;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class SteelDesignEffectiveLengthTests {
    private static string _memberList = "46 to 48";
    private static string[] _types = {
      "Major",
      "Minor",
      "LT",
    };

    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new SteelDesignEffectiveLength();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void SteelDesignEffectiveLengthIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.BasicFrame, 1);

      // Act
      var comp = new SteelDesignEffectiveLength();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Equal(37, paths.Count);

        var cases = paths.Select(x => x.Indices[0]).ToList();
        foreach (int caseid in cases) {
          Assert.Equal(1, caseid);
        }

        var permutations = paths.Select(x => x.Indices[1]).ToList();
        foreach (int permutation in permutations) {
          Assert.Equal(0, permutation);
        }
      }
    }

    [Fact]
    public void SteelDesignEffectiveLengthIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.BasicFrame, 1);

      // Act
      var comp = new SteelDesignEffectiveLength();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Equal(37, paths.Count);

        var cases = paths.Select(x => x.Indices[0]).ToList();
        foreach (int caseid in cases) {
          Assert.Equal(1, caseid);
        }

        var permutations = paths.Select(x => x.Indices[1]).ToList();
        foreach (int permutation in permutations) {
          Assert.Equal(1, permutation);
        }
      }
    }

    [Theory]
    [InlineData(SteelDesignEffectiveLengthHelper.SpanElements, true)]
    [InlineData(SteelDesignEffectiveLengthHelper.SpanElements, false)]
    public void SteelDesignEffectiveLengthStringOutputAnalysisCaseTest(
      SteelDesignEffectiveLengthHelper outputType, bool isAnalysisCase) {
      // Assemble
      GsaResult result = isAnalysisCase
        ? (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.BasicFrame, 1)
        : (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.BasicFrame, 1);

      var comp = new SteelDesignEffectiveLength();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, _memberList, 1);

      for (int type = 0; type < _types.Length; type++) {
        comp.SetSelected(0, type);
        List<double> expected = GetExpectedValues(outputType, isAnalysisCase, _types[type]);

        var strResult = ComponentTestHelper.GetResultOutputAllData(comp, (int)outputType)
         .Select(x => ((GH_String)x).Value).ToList();
        for (int i = 0; i < expected.Count; i++) {
          Assert.Equal(expected[i],
            double.Parse(strResult[i]));
        }
      }
    }

    [Theory]
    [InlineData(SteelDesignEffectiveLengthHelper.Span, true)]
    [InlineData(SteelDesignEffectiveLengthHelper.Span, false)]
    public void SteelDesignEffectiveLengthIntegerOutputAnalysisCaseTest(
      SteelDesignEffectiveLengthHelper outputType, bool isAnalysisCase) {
      // Assemble
      GsaResult result = isAnalysisCase
        ? (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.BasicFrame, 1)
        : (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.BasicFrame, 1);

      var comp = new SteelDesignEffectiveLength();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, _memberList, 1);

      for (int type = 0; type < _types.Length; type++) {
        comp.SetSelected(0, type);
        List<double> expected = GetExpectedValues(outputType, isAnalysisCase, _types[type]);

        var res = ComponentTestHelper.GetResultOutputAllData(comp, (int)outputType)
         .Select(x => ((GH_Integer)x).Value).ToList();

        for (int i = 0; i < expected.Count; i++) {
          Assert.Equal(expected[i], res[i]);
        }
      }
    }

    [Theory]
    [InlineData(SteelDesignEffectiveLengthHelper.MemberLength, true)]
    [InlineData(SteelDesignEffectiveLengthHelper.MemberLength, false)]
    [InlineData(SteelDesignEffectiveLengthHelper.StartPosition, true)]
    [InlineData(SteelDesignEffectiveLengthHelper.StartPosition, false)]
    [InlineData(SteelDesignEffectiveLengthHelper.EndPosition, true)]
    [InlineData(SteelDesignEffectiveLengthHelper.EndPosition, false)]
    [InlineData(SteelDesignEffectiveLengthHelper.SpanLength, true)]
    [InlineData(SteelDesignEffectiveLengthHelper.SpanLength, false)]
    [InlineData(SteelDesignEffectiveLengthHelper.EffectiveLength, true)]
    [InlineData(SteelDesignEffectiveLengthHelper.EffectiveLength, false)]
    public void SteelDesignEffectiveLengthLengthOutputAnalysisCaseTest(
      SteelDesignEffectiveLengthHelper outputType, bool isAnalysisCase) {
      // Assemble
      GsaResult result = isAnalysisCase
        ? (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.BasicFrame, 1)
        : (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.BasicFrame, 1);

      var comp = new SteelDesignEffectiveLength();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, _memberList, 1);

      for (int type = 0; type < _types.Length; type++) {
        comp.SetSelected(0, type);
        List<double> expected = GetExpectedValues(outputType, isAnalysisCase, _types[type]);

        for (int i = 0; i < expected.Count; i++) {
          List<IQuantity> resultOutput = ComponentTestHelper.GetResultOutput(comp, (int)outputType);
          Assert.Equal(expected[i], resultOutput[i].As(LengthUnit.Millimeter), DoubleComparer.Default);
        }
      }
    }

    [Theory]
    [InlineData(SteelDesignEffectiveLengthHelper.SlendernessRatio, true)]
    [InlineData(SteelDesignEffectiveLengthHelper.SlendernessRatio, false)]
    public void SteelDesignEffectiveLengthRatioOutputAnalysisCaseTest(
      SteelDesignEffectiveLengthHelper outputType, bool isAnalysisCase) {
      // Assemble
      GsaResult result = isAnalysisCase
        ? (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.BasicFrame, 1)
        : (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.BasicFrame, 1);

      var comp = new SteelDesignEffectiveLength();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, _memberList, 1);

      for (int type = 0; type < _types.Length; type++) {
        comp.SetSelected(0, type);
        List<double> expected = GetExpectedValues(outputType, isAnalysisCase, _types[type]);

        for (int i = 0; i < expected.Count; i++) {
          List<IQuantity> resultOutput = ComponentTestHelper.GetResultOutput(comp, (int)outputType);

          Assert.Equal(expected[i],
           resultOutput[i].As(RatioUnit.DecimalFraction),
              DoubleComparer.Default);
        }
      }
    }

    [Theory]

    [InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio, true)]
    [InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio, false)]
    [InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2, true)]
    [InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2, false)]
    public void SteelDesignEffectiveLengthNumberOutputAnalysisCaseTest(
      SteelDesignEffectiveLengthHelper outputType, bool isAnalysisCase) {
      // Assemble
      GsaResult result = isAnalysisCase
        ? (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.BasicFrame, 1)
        : (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.BasicFrame, 1);

      var comp = new SteelDesignEffectiveLength();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, _memberList, 1);

      for (int type = 0; type < _types.Length; type++) {
        comp.SetSelected(0, type);
        List<double> expected = GetExpectedValues(outputType, isAnalysisCase, _types[type]);

        for (int i = 0; i < expected.Count; i++) {
          var ratioResult = ComponentTestHelper.GetResultOutputAllData(comp, (int)outputType)
           .Select(x => ((GH_Number)x).Value).ToList();

          Assert.Equal(expected[i], ratioResult[i], DoubleComparer.Default);
        }
      }
    }

    private List<double> GetExpectedValues(
      SteelDesignEffectiveLengthHelper outputType, bool isAnalysisCase, string type) {
      switch (type) {
        case "Major":
          return isAnalysisCase ? ExpectedMajorAnalysisCaseValues(outputType) :
            ExpectedMajorCombinationCaseC1p1Values(outputType);
        case "Minor":
          return isAnalysisCase ? ExpectedMinorAnalysisCaseValues(outputType) :
            ExpectedMinorCombinationCaseC1p1Values(outputType);
        case "LT":
          return isAnalysisCase ? ExpectedLTAnalysisCaseValues(outputType) :
            ExpectedLTCombinationCaseC1p1Values(outputType);
        default: return null;
      }
    }

    private List<double> ExpectedMajorAnalysisCaseValues(SteelDesignEffectiveLengthHelper component) {
      switch (component) {
        case SteelDesignEffectiveLengthHelper.MemberLength: return SteelDesignEffectiveLengthA1.MajorMemberLength;
        case SteelDesignEffectiveLengthHelper.Span: return SteelDesignEffectiveLengthA1.MajorSpan;
        case SteelDesignEffectiveLengthHelper.SpanElements: return SteelDesignEffectiveLengthA1.MajorSpanElements;
        case SteelDesignEffectiveLengthHelper.StartPosition: return SteelDesignEffectiveLengthA1.MajorStartPosition;
        case SteelDesignEffectiveLengthHelper.EndPosition: return SteelDesignEffectiveLengthA1.MajorEndPosition;
        case SteelDesignEffectiveLengthHelper.SpanLength: return SteelDesignEffectiveLengthA1.MajorSpanLength;
        case SteelDesignEffectiveLengthHelper.EffectiveLength: return SteelDesignEffectiveLengthA1.MajorEffectiveLength;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio: return SteelDesignEffectiveLengthA1.MajorEffectiveLengthEffectiveSpanRatio;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2: return SteelDesignEffectiveLengthA1.MajorEffectiveLengthEffectiveSpanRatio2;
        case SteelDesignEffectiveLengthHelper.SlendernessRatio: return SteelDesignEffectiveLengthA1.MajorSlendernessRatio;
      }

      throw new NotImplementedException();
    }
    private List<double> ExpectedMinorAnalysisCaseValues(SteelDesignEffectiveLengthHelper component) {
      switch (component) {
        case SteelDesignEffectiveLengthHelper.MemberLength: return SteelDesignEffectiveLengthA1.MinorMemberLength;
        case SteelDesignEffectiveLengthHelper.Span: return SteelDesignEffectiveLengthA1.MinorSpan;
        case SteelDesignEffectiveLengthHelper.SpanElements: return SteelDesignEffectiveLengthA1.MinorSpanElements;
        case SteelDesignEffectiveLengthHelper.StartPosition: return SteelDesignEffectiveLengthA1.MinorStartPosition;
        case SteelDesignEffectiveLengthHelper.EndPosition: return SteelDesignEffectiveLengthA1.MinorEndPosition;
        case SteelDesignEffectiveLengthHelper.SpanLength: return SteelDesignEffectiveLengthA1.MinorSpanLength;
        case SteelDesignEffectiveLengthHelper.EffectiveLength: return SteelDesignEffectiveLengthA1.MinorEffectiveLength;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio: return SteelDesignEffectiveLengthA1.MinorEffectiveLengthEffectiveSpanRatio;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2: return SteelDesignEffectiveLengthA1.MinorEffectiveLengthEffectiveSpanRatio2;
        case SteelDesignEffectiveLengthHelper.SlendernessRatio: return SteelDesignEffectiveLengthA1.MinorSlendernessRatio;
      }

      throw new NotImplementedException();
    }
    private List<double> ExpectedLTAnalysisCaseValues(SteelDesignEffectiveLengthHelper component) {
      switch (component) {
        case SteelDesignEffectiveLengthHelper.MemberLength: return SteelDesignEffectiveLengthA1.LTMemberLength;
        case SteelDesignEffectiveLengthHelper.Span: return SteelDesignEffectiveLengthA1.LTSpan;
        case SteelDesignEffectiveLengthHelper.SpanElements: return SteelDesignEffectiveLengthA1.LTSpanElements;
        case SteelDesignEffectiveLengthHelper.StartPosition: return SteelDesignEffectiveLengthA1.LTStartPosition;
        case SteelDesignEffectiveLengthHelper.EndPosition: return SteelDesignEffectiveLengthA1.LTEndPosition;
        case SteelDesignEffectiveLengthHelper.SpanLength: return SteelDesignEffectiveLengthA1.LTSpanLength;
        case SteelDesignEffectiveLengthHelper.EffectiveLength: return SteelDesignEffectiveLengthA1.LTEffectiveLength;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio: return SteelDesignEffectiveLengthA1.LTEffectiveLengthEffectiveSpanRatio;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2: return SteelDesignEffectiveLengthA1.LTEffectiveLengthEffectiveSpanRatio2;
        case SteelDesignEffectiveLengthHelper.SlendernessRatio: return SteelDesignEffectiveLengthA1.LTSlendernessRatio;
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedMajorCombinationCaseC1p1Values(SteelDesignEffectiveLengthHelper component) {
      switch (component) {
        case SteelDesignEffectiveLengthHelper.MemberLength: return SteelDesignEffectiveLengthC1P1.MajorMemberLength;
        case SteelDesignEffectiveLengthHelper.Span: return SteelDesignEffectiveLengthC1P1.MajorSpan;
        case SteelDesignEffectiveLengthHelper.SpanElements: return SteelDesignEffectiveLengthC1P1.MajorSpanElements;
        case SteelDesignEffectiveLengthHelper.StartPosition: return SteelDesignEffectiveLengthC1P1.MajorStartPosition;
        case SteelDesignEffectiveLengthHelper.EndPosition: return SteelDesignEffectiveLengthC1P1.MajorEndPosition;
        case SteelDesignEffectiveLengthHelper.SpanLength: return SteelDesignEffectiveLengthC1P1.MajorSpanLength;
        case SteelDesignEffectiveLengthHelper.EffectiveLength: return SteelDesignEffectiveLengthC1P1.MajorEffectiveLength;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio: return SteelDesignEffectiveLengthC1P1.MajorEffectiveLengthEffectiveSpanRatio;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2: return SteelDesignEffectiveLengthC1P1.MajorEffectiveLengthEffectiveSpanRatio2;
        case SteelDesignEffectiveLengthHelper.SlendernessRatio: return SteelDesignEffectiveLengthC1P1.MajorSlendernessRatio;
      }

      throw new NotImplementedException();
    }
    private List<double> ExpectedMinorCombinationCaseC1p1Values(SteelDesignEffectiveLengthHelper component) {
      switch (component) {
        case SteelDesignEffectiveLengthHelper.MemberLength: return SteelDesignEffectiveLengthC1P1.MinorMemberLength;
        case SteelDesignEffectiveLengthHelper.Span: return SteelDesignEffectiveLengthC1P1.MinorSpan;
        case SteelDesignEffectiveLengthHelper.SpanElements: return SteelDesignEffectiveLengthC1P1.MinorSpanElements;
        case SteelDesignEffectiveLengthHelper.StartPosition: return SteelDesignEffectiveLengthC1P1.MinorStartPosition;
        case SteelDesignEffectiveLengthHelper.EndPosition: return SteelDesignEffectiveLengthC1P1.MinorEndPosition;
        case SteelDesignEffectiveLengthHelper.SpanLength: return SteelDesignEffectiveLengthC1P1.MinorSpanLength;
        case SteelDesignEffectiveLengthHelper.EffectiveLength: return SteelDesignEffectiveLengthC1P1.MinorEffectiveLength;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio: return SteelDesignEffectiveLengthC1P1.MinorEffectiveLengthEffectiveSpanRatio;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2: return SteelDesignEffectiveLengthC1P1.MinorEffectiveLengthEffectiveSpanRatio2;
        case SteelDesignEffectiveLengthHelper.SlendernessRatio: return SteelDesignEffectiveLengthC1P1.MinorSlendernessRatio;
      }

      throw new NotImplementedException();
    }
    private List<double> ExpectedLTCombinationCaseC1p1Values(SteelDesignEffectiveLengthHelper component) {
      switch (component) {
        case SteelDesignEffectiveLengthHelper.MemberLength: return SteelDesignEffectiveLengthC1P1.LTMemberLength;
        case SteelDesignEffectiveLengthHelper.Span: return SteelDesignEffectiveLengthC1P1.LTSpan;
        case SteelDesignEffectiveLengthHelper.SpanElements: return SteelDesignEffectiveLengthC1P1.LTSpanElements;
        case SteelDesignEffectiveLengthHelper.StartPosition: return SteelDesignEffectiveLengthC1P1.LTStartPosition;
        case SteelDesignEffectiveLengthHelper.EndPosition: return SteelDesignEffectiveLengthC1P1.LTEndPosition;
        case SteelDesignEffectiveLengthHelper.SpanLength: return SteelDesignEffectiveLengthC1P1.LTSpanLength;
        case SteelDesignEffectiveLengthHelper.EffectiveLength: return SteelDesignEffectiveLengthC1P1.LTEffectiveLength;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio: return SteelDesignEffectiveLengthC1P1.LTEffectiveLengthEffectiveSpanRatio;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2: return SteelDesignEffectiveLengthC1P1.LTEffectiveLengthEffectiveSpanRatio2;
        case SteelDesignEffectiveLengthHelper.SlendernessRatio: return SteelDesignEffectiveLengthC1P1.LTSlendernessRatio;
      }

      throw new NotImplementedException();
    }
    private Enum Unit(SteelDesignEffectiveLengthHelper component) {
      Enum unit = ForceUnit.Kilonewton;
      if ((int)component > 3) {
        unit = MomentUnit.KilonewtonMeter;
      }
      return unit;
    }
  }
}
