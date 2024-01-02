using Grasshopper.Kernel.Data;
using GsaGH.Components;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters.Results;
using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class BeamStressesTests {
    private static readonly string ElementList = "2 to 3";
    
    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new BeamStresses();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void Element1dStressElement1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      var comp = new BeamStresses();
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
    public void Element1dStresssElement1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      var comp = new BeamStresses();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Equal(elementIds.Count * 2, paths.Count); // elements * 2 permutations

        var cases = paths.Select(x => x.Indices[0]).ToList();
        foreach (int caseid in cases) {
          Assert.Equal(4, caseid);
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
    [InlineData(ResultStress1dHelperEnum.Axial)]
    [InlineData(ResultStress1dHelperEnum.ShearY)]
    [InlineData(ResultStress1dHelperEnum.ShearZ)]
    [InlineData(ResultStress1dHelperEnum.ByPos)]
    [InlineData(ResultStress1dHelperEnum.ByNeg)]
    [InlineData(ResultStress1dHelperEnum.BzPos)]
    [InlineData(ResultStress1dHelperEnum.BzNeg)]
    [InlineData(ResultStress1dHelperEnum.C1)]
    [InlineData(ResultStress1dHelperEnum.C2)]
    public void Element1dStresssMaxFromAnalysisCaseTest(ResultStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      var comp = new BeamStresses();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, 2, 2); // number of divisions, 2 + ends = 4

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(PressureUnit.Megapascal);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Theory]
    [InlineData(ResultStress1dHelperEnum.Axial)]
    [InlineData(ResultStress1dHelperEnum.ShearY)]
    [InlineData(ResultStress1dHelperEnum.ShearZ)]
    [InlineData(ResultStress1dHelperEnum.ByPos)]
    [InlineData(ResultStress1dHelperEnum.ByNeg)]
    [InlineData(ResultStress1dHelperEnum.BzPos)]
    [InlineData(ResultStress1dHelperEnum.BzNeg)]
    [InlineData(ResultStress1dHelperEnum.C1)]
    [InlineData(ResultStress1dHelperEnum.C2)]
    public void Element1dStresssMaxFromCombinationCaseTest(ResultStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(ExpectedCombinationCaseC4p1Values(component).Max(),
        ExpectedCombinationCaseC4p2Values(component).Max());

      // Act
      var comp = new BeamStresses();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, 2, 2); // number of divisions, 2 + ends = 4

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(PressureUnit.Megapascal);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Theory]
    [InlineData(ResultStress1dHelperEnum.Axial)]
    [InlineData(ResultStress1dHelperEnum.ShearY)]
    [InlineData(ResultStress1dHelperEnum.ShearZ)]
    [InlineData(ResultStress1dHelperEnum.ByPos)]
    [InlineData(ResultStress1dHelperEnum.ByNeg)]
    [InlineData(ResultStress1dHelperEnum.BzPos)]
    [InlineData(ResultStress1dHelperEnum.BzNeg)]
    [InlineData(ResultStress1dHelperEnum.C1)]
    [InlineData(ResultStress1dHelperEnum.C2)]
    public void Element1dStresssMinFromAnalysisCaseTest(ResultStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      var comp = new BeamStresses();
      comp.SetSelected(0, 10 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, 2, 2); // number of divisions, 2 + ends = 4

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(PressureUnit.Megapascal);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    [Theory]
    [InlineData(ResultStress1dHelperEnum.Axial)]
    [InlineData(ResultStress1dHelperEnum.ShearY)]
    [InlineData(ResultStress1dHelperEnum.ShearZ)]
    [InlineData(ResultStress1dHelperEnum.ByPos)]
    [InlineData(ResultStress1dHelperEnum.ByNeg)]
    [InlineData(ResultStress1dHelperEnum.BzPos)]
    [InlineData(ResultStress1dHelperEnum.BzNeg)]
    [InlineData(ResultStress1dHelperEnum.C1)]
    [InlineData(ResultStress1dHelperEnum.C2)]
    public void Element1dStresssMinFromcombinationCaseTest(ResultStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(ExpectedCombinationCaseC4p1Values(component).Min(),
        ExpectedCombinationCaseC4p2Values(component).Min());

      // Act
      var comp = new BeamStresses();
      comp.SetSelected(0, 10 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, 2, 2); // number of divisions, 2 + ends = 4

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(PressureUnit.Megapascal);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    private List<double> ExpectedAnalysisCaseValues(ResultStress1dHelperEnum component) {
      switch (component) {
        case ResultStress1dHelperEnum.Axial: return Element1dStressA1.AxialInMPa();
        case ResultStress1dHelperEnum.ShearY: return Element1dStressA1.SyInMPa();
        case ResultStress1dHelperEnum.ShearZ: return Element1dStressA1.SzInMPa();
        case ResultStress1dHelperEnum.ByPos: return Element1dStressA1.ByPosInMPa();
        case ResultStress1dHelperEnum.ByNeg: return Element1dStressA1.ByNegInMPa();
        case ResultStress1dHelperEnum.BzPos: return Element1dStressA1.BzPosInMPa();
        case ResultStress1dHelperEnum.BzNeg: return Element1dStressA1.BzNegInMPa();
        case ResultStress1dHelperEnum.C1: return Element1dStressA1.C1InMPa();
        case ResultStress1dHelperEnum.C2: return Element1dStressA1.C2InMPa();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p1Values(ResultStress1dHelperEnum component) {
      switch (component) {
        case ResultStress1dHelperEnum.Axial: return Element1dStressC4p1.AxialInMPa();
        case ResultStress1dHelperEnum.ShearY: return Element1dStressC4p1.SyInMPa();
        case ResultStress1dHelperEnum.ShearZ: return Element1dStressC4p1.SzInMPa();
        case ResultStress1dHelperEnum.ByPos: return Element1dStressC4p1.ByPosInMPa();
        case ResultStress1dHelperEnum.ByNeg: return Element1dStressC4p1.ByNegInMPa();
        case ResultStress1dHelperEnum.BzPos: return Element1dStressC4p1.BzPosInMPa();
        case ResultStress1dHelperEnum.BzNeg: return Element1dStressC4p1.BzNegInMPa();
        case ResultStress1dHelperEnum.C1: return Element1dStressC4p1.C1InMPa();
        case ResultStress1dHelperEnum.C2: return Element1dStressC4p1.C2InMPa();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p2Values(ResultStress1dHelperEnum component) {
      switch (component) {
        case ResultStress1dHelperEnum.Axial: return Element1dStressC4p2.AxialInMPa();
        case ResultStress1dHelperEnum.ShearY: return Element1dStressC4p2.SyInMPa();
        case ResultStress1dHelperEnum.ShearZ: return Element1dStressC4p2.SzInMPa();
        case ResultStress1dHelperEnum.ByPos: return Element1dStressC4p2.ByPosInMPa();
        case ResultStress1dHelperEnum.ByNeg: return Element1dStressC4p2.ByNegInMPa();
        case ResultStress1dHelperEnum.BzPos: return Element1dStressC4p2.BzPosInMPa();
        case ResultStress1dHelperEnum.BzNeg: return Element1dStressC4p2.BzNegInMPa();
        case ResultStress1dHelperEnum.C1: return Element1dStressC4p2.C1InMPa();
        case ResultStress1dHelperEnum.C2: return Element1dStressC4p2.C2InMPa();
      }

      throw new NotImplementedException();
    }
  }
}
