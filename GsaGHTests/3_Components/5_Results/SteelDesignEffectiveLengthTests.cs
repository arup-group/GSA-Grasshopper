using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGHTests._1_BaseParameters._5_Results.Collections.RegressionValues;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters.Results;
using OasysGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Xunit;
using SteelDesignEffectiveLength = GsaGH.Components.SteelDesignEffectiveLength;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class SteelDesignEffectiveLengthTests {
    private static readonly string MemberList = "46 to 48";

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

    //[Theory]
    ////[InlineData(SteelDesignEffectiveLengthHelper.MemberLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.Span)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SpanElements)]
    //[InlineData(SteelDesignEffectiveLengthHelper.StartPosition)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EndPosition)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SpanLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SlendernessRatio)]
    //public void SteelDesignEffectiveLengthFromAnalysisCaseTest(SteelDesignEffectiveLengthHelper component) {
    //  // Assemble
    //  var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.BasicFrame, 1);
    //  List<double> expected = ExpectedAnalysisCaseValues(component);

    //  // Act
    //  var comp = new SteelDesignEffectiveLength();
    //  ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
    //  List<IQuantity> resultOutput;

    //  for (int i = 0; i<= expected.Count; i++)
    //  {
    //    switch (component) {
    //      //case SteelDesignEffectiveLengthHelper.MemberLength:
    //      case SteelDesignEffectiveLengthHelper.Span: //integer
    //        var res = ((IList<IGH_Goo>)ComponentTestHelper.GetListOutput(comp, (int)component)).Select(x => ((GH_Integer)x).Value).ToList();

    //        Assert.Equal(expected[i], res[i]);
    //        break;
    //      case SteelDesignEffectiveLengthHelper.SpanElements:
    //        var a = (IList<GH_String>)ComponentTestHelper.GetListOutput(comp, (int)component);
    //        var strResult = ((IList<GH_String>)ComponentTestHelper.GetListOutput(comp, (int)component)).Select(x => ((GH_String)x).Value).ToList();

    //        Assert.Equal(expected[i], double.Parse(strResult[i]));
    //        break;

    //      case SteelDesignEffectiveLengthHelper.StartPosition:
    //      case SteelDesignEffectiveLengthHelper.EndPosition:
    //      case SteelDesignEffectiveLengthHelper.SpanLength:
    //      case SteelDesignEffectiveLengthHelper.EffectiveLength:
    //        resultOutput = ComponentTestHelper.GetResultOutput(comp, (int)component);
    //        Assert.Equal(expected[i], ResultHelper.RoundToSignificantDigits(resultOutput[i].As(LengthUnit.Meter), 4));
    //        break;
    //      case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio://ratio
    //      case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2:
    //      case SteelDesignEffectiveLengthHelper.SlendernessRatio:
    //        resultOutput = ComponentTestHelper.GetResultOutput(comp, (int)component);
    //        Assert.Equal(expected[i], ResultHelper.RoundToSignificantDigits(resultOutput[i].As(Ratio.BaseUnit), 4));
    //        break;
    //    }
    //  }
          

      // Assert Max in set
      //Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    //}

    //[Theory]
    //[InlineData(SteelDesignEffectiveLengthHelper.MemberLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.Span)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SpanElements)]
    //[InlineData(SteelDesignEffectiveLengthHelper.StartPosition)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EndPosition)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SpanLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SlendernessRatio)]
    //public void NodeReactionForceMaxFromCombinationCaseTest(SteelDesignEffectiveLengthHelper component) {
    //  // Assemble
    //  var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.BasicFrame, 2);
    //  var values = new List<double?>();
    //  values.AddRange(ExpectedCombinationCaseC2p1Values(component));
    //  values.AddRange(ExpectedCombinationCaseC2p2Values(component));
    //  double? expected = NodeReactionForcesTests.Max(values);

    //  // Act
    //  var comp = new SteelDesignEffectiveLength();
    //  comp.SetSelected(0, 1 + (int)component);
    //  ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
    //  ComponentTestHelper.SetInput(comp, NodeList, 1);
    //  List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

    //  // Assert Max in set
    //  double max = output.Max().As(Unit(component));
    //  Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    //}

    //[Theory]
    //[InlineData(SteelDesignEffectiveLengthHelper.MemberLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.Span)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SpanElements)]
    //[InlineData(SteelDesignEffectiveLengthHelper.StartPosition)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EndPosition)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SpanLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SlendernessRatio)]
    //public void NodeReactionForceMinFromAnalysisCaseTest(SteelDesignEffectiveLengthHelper component) {
    //  // Assemble
    //  var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.BasicFrame, 1);
    //  double? expected = ExpectedAnalysisCaseValues(component).Min();

    //  // Act
    //  var comp = new SteelDesignEffectiveLength();
    //  comp.SetSelected(0, 9 + (int)component);
    //  ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
    //  ComponentTestHelper.SetInput(comp, NodeList, 1);
    //  List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

    //  // Assert Min in set
    //  double min = output.Min().As(Unit(component));
    //  Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    //}

    //[Theory]
    //[InlineData(SteelDesignEffectiveLengthHelper.MemberLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.Span)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SpanElements)]
    //[InlineData(SteelDesignEffectiveLengthHelper.StartPosition)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EndPosition)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SpanLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SlendernessRatio)]
    //public void NodeReactionForceMinFromcombinationCaseTest(SteelDesignEffectiveLengthHelper component) {
    //  // Assemble
    //  var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.BasicFrame, 2);
    //  var values = new List<double?>();
    //  values.AddRange(ExpectedCombinationCaseC2p1Values(component));
    //  values.AddRange(ExpectedCombinationCaseC2p2Values(component));
    //  double? expected = NodeReactionForcesTests.Min(values);

    //  // Act
    //  var comp = new SteelDesignEffectiveLength();
    //  comp.SetSelected(0, 9 + (int)component);
    //  ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
    //  ComponentTestHelper.SetInput(comp, NodeList, 1);

    //  List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

    //  // Assert Min in set
    //  double min = output.Min().As(Unit(component));
    //  Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    //}

    //[Theory]
    //[InlineData(SteelDesignEffectiveLengthHelper.MemberLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.Span)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SpanElements)]
    //[InlineData(SteelDesignEffectiveLengthHelper.StartPosition)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EndPosition)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SpanLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SlendernessRatio)]
    //public void NodeReactionForceValuesFromAnalysisCaseTest(SteelDesignEffectiveLengthHelper component) {
    //  // Assemble
    //  var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.BasicFrame, 1);
    //  List<double?> expected = ExpectedAnalysisCaseValues(component);

    //  // Act
    //  var comp = new SteelDesignEffectiveLength();
    //  ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
    //  ComponentTestHelper.SetInput(comp, NodeList, 1);
    //  List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

    //  // Assert result values
    //  int i = 0;
    //  foreach (IQuantity value in output) {
    //    double x = ResultHelper.RoundToSignificantDigits(value.As(Unit(component)), 4);
    //    Assert.Equal(expected[i++], x);
    //  }
    //}

    //[Theory]
    //[InlineData(SteelDesignEffectiveLengthHelper.MemberLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.Span)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SpanElements)]
    //[InlineData(SteelDesignEffectiveLengthHelper.StartPosition)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EndPosition)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SpanLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveLength)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio)]
    //[InlineData(SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2)]
    //[InlineData(SteelDesignEffectiveLengthHelper.SlendernessRatio)]
    //public void NodeReactionForceValuesFromCombinationCaseTest(SteelDesignEffectiveLengthHelper component) {
    //  // Assemble
    //  var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.BasicFrame, 2);
    //  List<double?> expectedP1 = ExpectedCombinationCaseC2p1Values(component);
    //  List<double?> expectedP2 = ExpectedCombinationCaseC2p2Values(component);

    //  // Act
    //  var comp = new SteelDesignEffectiveLength();
    //  ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
    //  ComponentTestHelper.SetInput(comp, NodeList, 1);
    //  var p1 = new GH_Path(2, 1);
    //  List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component, p1);

    //  // Assert result values
    //  for (int i = 0; i < output.Count; i++) {
    //    double perm = ResultHelper.RoundToSignificantDigits(output[i].As(Unit(component)), 4);
    //    Assert.Equal(expectedP1[i], perm);
    //  }

    //  var p2 = new GH_Path(2, 2);
    //  output = ComponentTestHelper.GetResultOutput(comp, (int)component, p2);

    //  // Assert result values
    //  for (int i = 0; i < output.Count; i++) {
    //    double perm = ResultHelper.RoundToSignificantDigits(output[i].As(Unit(component)), 4);
    //    Assert.Equal(expectedP2[i], perm);
    //  }
    //}

    private List<double> ExpectedAnalysisCaseValues(SteelDesignEffectiveLengthHelper component) {
      switch (component) {
        case SteelDesignEffectiveLengthHelper.MemberLength: return SteelDesignEffectiveLengthA1.MemberLength;
        case SteelDesignEffectiveLengthHelper.Span: return SteelDesignEffectiveLengthA1.Span;
        case SteelDesignEffectiveLengthHelper.SpanElements: return SteelDesignEffectiveLengthA1.SpanElements;
        case SteelDesignEffectiveLengthHelper.StartPosition: return SteelDesignEffectiveLengthA1.StartPosition;
        case SteelDesignEffectiveLengthHelper.EndPosition: return SteelDesignEffectiveLengthA1.EndPosition;
        case SteelDesignEffectiveLengthHelper.SpanLength: return SteelDesignEffectiveLengthA1.SpanLength;
        case SteelDesignEffectiveLengthHelper.EffectiveLength: return SteelDesignEffectiveLengthA1.EffectiveLength;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio: return SteelDesignEffectiveLengthA1.EffectiveLengthEffectiveSpanRatio;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2: return SteelDesignEffectiveLengthA1.EffectiveLengthEffectiveSpanRatio2;
        case SteelDesignEffectiveLengthHelper.SlendernessRatio: return SteelDesignEffectiveLengthA1.SlendernessRatio;
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC1p1Values(SteelDesignEffectiveLengthHelper component) {
      switch (component) {
        case SteelDesignEffectiveLengthHelper.MemberLength: return SteelDesignEffectiveLengthC1P1.MemberLength;
        case SteelDesignEffectiveLengthHelper.Span: return SteelDesignEffectiveLengthC1P1.Span;
        case SteelDesignEffectiveLengthHelper.SpanElements: return SteelDesignEffectiveLengthC1P1.SpanElements;
        case SteelDesignEffectiveLengthHelper.StartPosition: return SteelDesignEffectiveLengthC1P1.StartPosition;
        case SteelDesignEffectiveLengthHelper.EndPosition: return SteelDesignEffectiveLengthC1P1.EndPosition;
        case SteelDesignEffectiveLengthHelper.SpanLength: return SteelDesignEffectiveLengthC1P1.SpanLength;
        case SteelDesignEffectiveLengthHelper.EffectiveLength: return SteelDesignEffectiveLengthC1P1.EffectiveLength;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio: return SteelDesignEffectiveLengthC1P1.EffectiveLengthEffectiveSpanRatio;
        case SteelDesignEffectiveLengthHelper.EffectiveSpanRatio2: return SteelDesignEffectiveLengthC1P1.EffectiveLengthEffectiveSpanRatio2;
        case SteelDesignEffectiveLengthHelper.SlendernessRatio: return SteelDesignEffectiveLengthC1P1.SlendernessRatio;
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
