﻿using GH_IO.Serialization;
using Grasshopper.Kernel.Data;
using GsaGH.Components;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters.Results;
using OasysGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class Element2dForcesAndMomentsShearsTests {
    private static readonly string ElementList = "420 430 440 445";

    [Fact]
    public void Element2dShearForcesElement2dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      var comp = new Element2dForcesAndMoments();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Equal(elementIds.Count, paths.Count);

        var cases = paths.Select(x => x.Indices[0]).ToList();
        foreach (int caseid in cases) {
          Assert.Equal(1, caseid);
        }

        var permutations = paths.Select(x => x.Indices[1]).ToList();
        foreach (int permutation in permutations) {
          Assert.Equal(0, permutation);
        }

        var ids = paths.Select(x => x.Indices[2]).ToList();
        for (int j = 0; j < ids.Count; j++) {
          // Assert element IDs
          var expectedIds = result.Model.Model.Elements(ElementList).Keys.OrderBy(x => x).ToList();
          Assert.Equal(expectedIds[j], ids[j]);
        }
      }
    }

    [Fact]
    public void Element2dIShearForcesElement2dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.CombinationCaseResult(GsaFile.Element2dSimple, 2);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      var comp = new Element2dForcesAndMoments();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Equal(elementIds.Count * 2, paths.Count); // elements * 2 permutations

        var cases = paths.Select(x => x.Indices[0]).ToList();
        foreach (int caseid in cases) {
          Assert.Equal(2, caseid);
        }

        var expectedPermutations = new List<int>();
        for (int j = 0; j < elementIds.Count; j++) {
          expectedPermutations.AddRange(Enumerable.Repeat(j + 1, elementIds.Count));
        }
        var permutations = paths.Select(x => x.Indices[1]).ToList();
        for (int j = 0; j < permutations.Count; j++) {
          Assert.Equal(expectedPermutations[j], permutations[j]);
        }

        var expectedIds = elementIds.ToList();
        expectedIds.AddRange(elementIds.ToList()); // add elementlist for each permutation

        var ids = paths.Select(x => x.Indices[2]).ToList();
        for (int j = 0; j < ids.Count; j++) {
          Assert.Equal(expectedIds[j], ids[j]);
        }
      }
    }

    [Theory]
    [InlineData(ResultVector2HelperEnum.Qx)]
    [InlineData(ResultVector2HelperEnum.Qy)]
    public void Element2dShearForcesMaxFromAnalysisCaseTest(ResultVector2HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      var comp = new Element2dForcesAndMoments();
      comp.SetSelected(0, 4 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 3 + (int)component);

      // Assert Max in set
      double max = output.Max().As(ForcePerLengthUnit.KilonewtonPerMeter);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Theory]
    [InlineData(ResultVector2HelperEnum.Qx)]
    [InlineData(ResultVector2HelperEnum.Qy)]
    public void Element2dShearForcesMaxFromCombinationCaseTest(ResultVector2HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Max(ExpectedCombinationCaseC2p1Values(component).Max(),
        ExpectedCombinationCaseC2p2Values(component).Max());

      // Act
      var comp = new Element2dForcesAndMoments();
      comp.SetSelected(0, 4 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 3 + (int)component);

      // Assert Max in set
      double max = output.Max().As(ForcePerLengthUnit.KilonewtonPerMeter);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Theory]
    [InlineData(ResultVector2HelperEnum.Qx)]
    [InlineData(ResultVector2HelperEnum.Qy)]
    public void Element2dShearForcesMinFromAnalysisCaseTest(ResultVector2HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      var comp = new Element2dForcesAndMoments();
      comp.SetSelected(0, 14 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 3 + (int)component);

      // Assert Min in set
      double min = output.Min().As(ForcePerLengthUnit.KilonewtonPerMeter);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    [Theory]
    [InlineData(ResultVector2HelperEnum.Qx)]
    [InlineData(ResultVector2HelperEnum.Qy)]
    public void Element2dShearForcessMinFromcombinationCaseTest(ResultVector2HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Min(ExpectedCombinationCaseC2p1Values(component).Min(),
        ExpectedCombinationCaseC2p2Values(component).Min());

      // Act
      var comp = new Element2dForcesAndMoments();
      comp.SetSelected(0, 14 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 3 + (int)component);

      // Assert Min in set
      double min = output.Min().As(ForcePerLengthUnit.KilonewtonPerMeter);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector2HelperEnum component) {
      switch (component) {
        case ResultVector2HelperEnum.Qx: return Element2dShearForcesA1.QxInKiloNewtonPerMeter();
        case ResultVector2HelperEnum.Qy: return Element2dShearForcesA1.QyInKiloNewtonPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p1Values(ResultVector2HelperEnum component) {
      switch (component) {
        case ResultVector2HelperEnum.Qx: return Element2dShearForcesC2p1.QxInKiloNewtonPerMeter();
        case ResultVector2HelperEnum.Qy: return Element2dShearForcesC2p1.QyInKiloNewtonPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p2Values(ResultVector2HelperEnum component) {
      switch (component) {
        case ResultVector2HelperEnum.Qx: return Element2dShearForcesC2p2.QxInKiloNewtonPerMeter();
        case ResultVector2HelperEnum.Qy: return Element2dShearForcesC2p2.QyInKiloNewtonPerMeter();
      }

      throw new NotImplementedException();
    }
  }
}
