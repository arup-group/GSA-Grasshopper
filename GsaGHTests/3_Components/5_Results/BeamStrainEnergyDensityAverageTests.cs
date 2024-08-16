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
  public class BeamStrainEnergyDensityAverageTests {
    private static readonly string ElementList = "2 to 38";

    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new BeamStrainEnergyDensity();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void Element1dAverageStrainEnergyDensityElement1dIdsFromAnalysisCaseTest() {
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
    public void Element1dAverageStrainEnergyDensitysElement1dIdsFromCombinationCaseTest() {
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
    public void Element1dAverageStrainEnergyDensitysMaxFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = Element1dAverageStrainEnergyDensity.A1EnergyInkJ().Max();

      // Act
      var comp = new BeamStrainEnergyDensity();
      comp.SetSelected(0, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 0);

      // Assert Max in set
      double max = output.Max().As(EnergyUnit.Kilojoule);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Fact]
    public void Element1dAverageStrainEnergyDensitysMaxFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(
        Element1dAverageStrainEnergyDensity.C4p1EnergyInkJ().Max(),
        Element1dAverageStrainEnergyDensity.C4p2EnergyInkJ().Max());

      // Act
      var comp = new BeamStrainEnergyDensity();
      comp.SetSelected(0, 1);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 0);

      // Assert Max in set
      double max = output.Max().As(EnergyUnit.Kilojoule);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Fact]
    public void Element1dAverageStrainEnergyDensitysMinFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = Element1dAverageStrainEnergyDensity.A1EnergyInkJ().Min();

      // Act
      var comp = new BeamStrainEnergyDensity();
      comp.SetSelected(0, 2);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 0);

      // Assert Min in set
      double min = output.Min().As(EnergyUnit.Kilojoule);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    [Fact]
    public void Element1dAverageStrainEnergyDensitysMinFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(
        Element1dAverageStrainEnergyDensity.C4p1EnergyInkJ().Min(),
        Element1dAverageStrainEnergyDensity.C4p2EnergyInkJ().Min());

      // Act
      var comp = new BeamStrainEnergyDensity();
      comp.SetSelected(0, 2);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 0);

      // Assert Min in set
      double min = output.Min().As(EnergyUnit.Kilojoule);
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }
  }
}
