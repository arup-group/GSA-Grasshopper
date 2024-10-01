using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

using Xunit;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class BeamStrainEnergyDensityTests {
    private static readonly string ElementList = "2 to 3";

    [Fact]
    public void Element1dStrainEnergyDensityElement1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      var comp = new BeamStrainEnergyDensity();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, 0);
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
        var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
        Assert.Equal(expectedIds[j], ids[j]);
      }
    }

    [Fact]
    public void Element1dStrainEnergyDensitysElement1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      var comp = new BeamStrainEnergyDensity();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, 0);
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

    [Fact]
    public void Element1dStrainEnergyDensitysMaxFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = Element1dStrainEnergyDensity.A1EnergyInkJ().Max();

      // Act
      var comp = new BeamStrainEnergyDensity();
      comp.SetSelected(0, 1);
      comp.SetAnalysis(new List<bool> { false });
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, 3, 2); // number of divisions, 3 + ends = 5

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 0);

      // Assert Max in set
      double max = output.Max().As(EnergyUnit.Kilojoule);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Fact]
    public void Element1dStrainEnergyDensitysMaxFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(Element1dStrainEnergyDensity.C4p1EnergyInkJ().Max(),
        Element1dStrainEnergyDensity.C4p2EnergyInkJ().Max());

      // Act
      var comp = new BeamStrainEnergyDensity();
      comp.SetSelected(0, 1);
      comp.SetAnalysis(new List<bool> { false });
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, 3, 2); // number of divisions, 3 + ends = 5

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 0);

      // Assert Max in set
      double max = output.Max().As(EnergyUnit.Kilojoule);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Fact]
    public void Element1dStrainEnergyDensitysMinFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = Element1dStrainEnergyDensity.A1EnergyInkJ().Min();

      // Act
      var comp = new BeamStrainEnergyDensity();
      comp.SetSelected(0, 2);
      comp.SetAnalysis(new List<bool> { false });
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, 3, 2); // number of divisions, 3 + ends = 5

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 0);

      // Assert Min in set
      double min = output.Min().As(EnergyUnit.Kilojoule);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    [Fact]
    public void Element1dStrainEnergyDensitysMinFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(Element1dStrainEnergyDensity.C4p1EnergyInkJ().Min(),
        Element1dStrainEnergyDensity.C4p2EnergyInkJ().Min());

      // Act
      var comp = new BeamStrainEnergyDensity();
      comp.SetSelected(0, 2);
      comp.SetAnalysis(new List<bool> { false });
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, 3, 2); // number of divisions, 3 + ends = 5

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 0);

      // Assert Min in set
      double min = output.Min().As(EnergyUnit.Kilojoule);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    [Fact]
    public void Element1dStrainEnergyDensityToggleAverageTest() {
      var comp = new BeamStrainEnergyDensity();
      Assert.Equal(2, comp.Params.Input.Count);
      comp.SetAnalysis(new List<bool> { false });
      Assert.Equal(3, comp.Params.Input.Count);
      comp.SetAnalysis(new List<bool> { true });
      Assert.Equal(2, comp.Params.Input.Count);
    }
  }
}
