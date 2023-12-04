using GH_IO.Serialization;
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
  public class BeamDerivedStressesTests {
    private static readonly string ElementList = "2 to 3";

    [Fact]
    public void Element1dDerivedStressElement1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      var comp = new BeamDerivedStresses();
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
      var result = (GsaResult)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      var comp = new BeamDerivedStresses();
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
    [InlineData(ResultDerivedStress1dHelperEnum.ShearY)]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearZ)]
    [InlineData(ResultDerivedStress1dHelperEnum.Torsion)]
    [InlineData(ResultDerivedStress1dHelperEnum.VonMises)]
    public void Element1dStresssMaxFromAnalysisCaseTest(ResultDerivedStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      var comp = new BeamDerivedStresses();
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
    [InlineData(ResultDerivedStress1dHelperEnum.ShearY)]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearZ)]
    [InlineData(ResultDerivedStress1dHelperEnum.Torsion)]
    [InlineData(ResultDerivedStress1dHelperEnum.VonMises)]
    public void Element1dStresssMaxFromCombinationCaseTest(ResultDerivedStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(ExpectedCombinationCaseC4p1Values(component).Max(),
        ExpectedCombinationCaseC4p2Values(component).Max());

      // Act
      var comp = new BeamDerivedStresses();
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
    [InlineData(ResultDerivedStress1dHelperEnum.ShearY)]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearZ)]
    [InlineData(ResultDerivedStress1dHelperEnum.Torsion)]
    [InlineData(ResultDerivedStress1dHelperEnum.VonMises)]
    public void Element1dStresssMinFromAnalysisCaseTest(ResultDerivedStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      var comp = new BeamDerivedStresses();
      comp.SetSelected(0, 5 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, 2, 2); // number of divisions, 2 + ends = 4

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(PressureUnit.Megapascal);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    [Theory]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearY)]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearZ)]
    [InlineData(ResultDerivedStress1dHelperEnum.Torsion)]
    [InlineData(ResultDerivedStress1dHelperEnum.VonMises)]
    public void Element1dStresssMinFromcombinationCaseTest(ResultDerivedStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(ExpectedCombinationCaseC4p1Values(component).Min(),
        ExpectedCombinationCaseC4p2Values(component).Min());

      // Act
      var comp = new BeamDerivedStresses();
      comp.SetSelected(0, 5 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, 2, 2); // number of divisions, 2 + ends = 4

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(PressureUnit.Megapascal);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    private List<double> ExpectedAnalysisCaseValues(ResultDerivedStress1dHelperEnum component) {
      switch (component) {
        case ResultDerivedStress1dHelperEnum.ShearY: return Element1dDerivedStressA1.SEyInMPa();

        case ResultDerivedStress1dHelperEnum.ShearZ: return Element1dDerivedStressA1.SEzInMPa();

        case ResultDerivedStress1dHelperEnum.Torsion: return Element1dDerivedStressA1.StInMPa();

        case ResultDerivedStress1dHelperEnum.VonMises: return Element1dDerivedStressA1.VonMisesInMPa();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p1Values(ResultDerivedStress1dHelperEnum component) {
      switch (component) {
        case ResultDerivedStress1dHelperEnum.ShearY: return Element1dDerivedStressC4p1.SEyInMPa();

        case ResultDerivedStress1dHelperEnum.ShearZ: return Element1dDerivedStressC4p1.SEzInMPa();

        case ResultDerivedStress1dHelperEnum.Torsion: return Element1dDerivedStressC4p1.StInMPa();

        case ResultDerivedStress1dHelperEnum.VonMises: return Element1dDerivedStressC4p1.VonMisesInMPa();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p2Values(ResultDerivedStress1dHelperEnum component) {
      switch (component) {
        case ResultDerivedStress1dHelperEnum.ShearY: return Element1dDerivedStressC4p2.SEyInMPa();

        case ResultDerivedStress1dHelperEnum.ShearZ: return Element1dDerivedStressC4p2.SEzInMPa();

        case ResultDerivedStress1dHelperEnum.Torsion: return Element1dDerivedStressC4p2.StInMPa();

        case ResultDerivedStress1dHelperEnum.VonMises: return Element1dDerivedStressC4p2.VonMisesInMPa();
      }

      throw new NotImplementedException();
    }
  }
}
