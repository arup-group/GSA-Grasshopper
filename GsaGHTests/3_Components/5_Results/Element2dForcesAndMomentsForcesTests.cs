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
  public class Element2dForcesAndMomentsForcesTests {
    private static readonly string ElementList = "420 430 440 445";

    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new Element2dForcesAndMoments();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void Element2dForcesElement2dIdsFromAnalysisCaseTest() {
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
    public void Element2dForcesElement2dIdsFromCombinationCaseTest() {
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
    [InlineData(ResultTensor2InAxisHelperEnum.Nx)]
    [InlineData(ResultTensor2InAxisHelperEnum.Ny)]
    [InlineData(ResultTensor2InAxisHelperEnum.Nxy)]
    public void Element2dForcesMaxFromAnalysisCaseTest(ResultTensor2InAxisHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      var comp = new Element2dForcesAndMoments();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(ForcePerLengthUnit.KilonewtonPerMeter);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Theory]
    [InlineData(ResultTensor2InAxisHelperEnum.Nx)]
    [InlineData(ResultTensor2InAxisHelperEnum.Ny)]
    [InlineData(ResultTensor2InAxisHelperEnum.Nxy)]
    public void Element2dForcesMaxFromCombinationCaseTest(ResultTensor2InAxisHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Max(ExpectedCombinationCaseC2p1Values(component).Max(),
        ExpectedCombinationCaseC2p2Values(component).Max());

      // Act
      var comp = new Element2dForcesAndMoments();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(ForcePerLengthUnit.KilonewtonPerMeter);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Theory]
    [InlineData(ResultTensor2InAxisHelperEnum.Nx)]
    [InlineData(ResultTensor2InAxisHelperEnum.Ny)]
    [InlineData(ResultTensor2InAxisHelperEnum.Nxy)]
    public void Element2dForcesMinFromAnalysisCaseTest(ResultTensor2InAxisHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      var comp = new Element2dForcesAndMoments();
      comp.SetSelected(0, 11 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(ForcePerLengthUnit.KilonewtonPerMeter);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    [Theory]
    [InlineData(ResultTensor2InAxisHelperEnum.Nx)]
    [InlineData(ResultTensor2InAxisHelperEnum.Ny)]
    [InlineData(ResultTensor2InAxisHelperEnum.Nxy)]
    public void Element2dForcesMinFromcombinationCaseTest(ResultTensor2InAxisHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Min(ExpectedCombinationCaseC2p1Values(component).Min(),
        ExpectedCombinationCaseC2p2Values(component).Min());

      // Act
      var comp = new Element2dForcesAndMoments();
      comp.SetSelected(0, 11 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(ForcePerLengthUnit.KilonewtonPerMeter);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    private List<double> ExpectedAnalysisCaseValues(ResultTensor2InAxisHelperEnum component) {
      switch (component) {
        case ResultTensor2InAxisHelperEnum.Nx: return Element2dForcesA1.NxInKiloNewtonPerMeter();
        case ResultTensor2InAxisHelperEnum.Ny: return Element2dForcesA1.NyInKiloNewtonPerMeter();
        case ResultTensor2InAxisHelperEnum.Nxy: return Element2dForcesA1.NxyInKiloNewtonPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p1Values(ResultTensor2InAxisHelperEnum component) {
      switch (component) {
        case ResultTensor2InAxisHelperEnum.Nx: return Element2dForcesC2p1.NxInKiloNewtonPerMeter();
        case ResultTensor2InAxisHelperEnum.Ny: return Element2dForcesC2p1.NyInKiloNewtonPerMeter();
        case ResultTensor2InAxisHelperEnum.Nxy: return Element2dForcesC2p1.NxyInKiloNewtonPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p2Values(ResultTensor2InAxisHelperEnum component) {
      switch (component) {
        case ResultTensor2InAxisHelperEnum.Nx: return Element2dForcesC2p2.NxInKiloNewtonPerMeter();
        case ResultTensor2InAxisHelperEnum.Ny: return Element2dForcesC2p2.NyInKiloNewtonPerMeter();
        case ResultTensor2InAxisHelperEnum.Nxy: return Element2dForcesC2p2.NxyInKiloNewtonPerMeter();
      }

      throw new NotImplementedException();
    }
  }
}
