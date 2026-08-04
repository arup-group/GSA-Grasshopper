using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters.Results;

using Newtonsoft.Json.Linq;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class NodalForcesAndMomentsTests {
    private static readonly string NodeList = "442";

    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new GsaGH.Components.NodalForcesAndMoments();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void NodeReactionForceNodeIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new GsaGH.Components.NodalForcesAndMoments();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Single(paths);

        var cases = paths.Select(x => x.Indices[0]).ToList();
        foreach (int caseid in cases) {
          Assert.Equal(1, caseid);
        }

        var permutations = paths.Select(x => x.Indices[1]).ToList();
        foreach (int permutation in permutations) {
          Assert.Equal(0, permutation);
        }
      }

      // Assert
      var ids = (IList<GH_Integer>)ComponentTestHelper.GetListOutput(comp, 8);
      var expectedIds = new List<int>() { 2, 3, 1282, 2571 };
      for (int j = 0; j < ids.Count; j++) {
        Assert.Equal(expectedIds[j], ids[j].Value);
      }
    }

    [Fact]
    public void NodeReactionForceNodeIdsFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 1, new List<int>() { 1 });

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new GsaGH.Components.NodalForcesAndMoments();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Single(paths);

        var cases = paths.Select(x => x.Indices[0]).ToList();
        foreach (int caseid in cases) {
          Assert.Equal(1, caseid);
        }

        var permutations = paths.Select(x => x.Indices[1]).ToList();
        foreach (int permutation in permutations) {
          Assert.Equal(1, permutation);
        }
      }

      // Assert
      var ids = (IList<GH_Integer>)ComponentTestHelper.GetListOutput(comp, 8);
      var expectedIds = new List<int>() { 2, 3, 1282, 2571 };
      for (int j = 0; j < ids.Count; j++) {
        Assert.Equal(expectedIds[j], ids[j].Value);
      }
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void NodeReactionForceValuesFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      var comp = new GsaGH.Components.NodalForcesAndMoments();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert
      int i = 0;
      foreach (IQuantity value in output) {
        double x = value.As(Unit(component));
        Assert.Equal(expected[i++], x, 1);
      }
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void NodeReactionForceValuesFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 1, new List<int>() { 1 });
      List<double> expected = ExpectedCombinationCaseValues(component);

      // Act
      var comp = new GsaGH.Components.NodalForcesAndMoments();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert
      for (int i = 0; i < output.Count; i++) {
        double perm = output[i].As(Unit(component));
        Assert.Equal(expected[i++], perm, 1);
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return NodalForcesAndMomentsA1.XInKiloNewtons();

        case ResultVector6.Y: return NodalForcesAndMomentsA1.YInKiloNewtons();

        case ResultVector6.Z: return NodalForcesAndMomentsA1.ZInKiloNewtons();

        case ResultVector6.Xyz: return NodalForcesAndMomentsA1.XyzInKiloNewtons();

        case ResultVector6.Xx: return NodalForcesAndMomentsA1.XxInKiloNewtonsPerMeter();

        case ResultVector6.Yy: return NodalForcesAndMomentsA1.YyInKiloNewtonsPerMeter();

        case ResultVector6.Zz: return NodalForcesAndMomentsA1.ZzInKiloNewtonsPerMeter();

        case ResultVector6.Xxyyzz: return NodalForcesAndMomentsA1.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return NodalForcesAndMomentsC1.XInKiloNewtons();

        case ResultVector6.Y: return NodalForcesAndMomentsC1.YInKiloNewtons();

        case ResultVector6.Z: return NodalForcesAndMomentsC1.ZInKiloNewtons();

        case ResultVector6.Xyz: return NodalForcesAndMomentsC1.YzInKiloNewtons();

        case ResultVector6.Xx: return NodalForcesAndMomentsC1.XxInKiloNewtonsPerMeter();

        case ResultVector6.Yy: return NodalForcesAndMomentsC1.YyInKiloNewtonsPerMeter();

        case ResultVector6.Zz: return NodalForcesAndMomentsC1.ZzInKiloNewtonsPerMeter();

        case ResultVector6.Xxyyzz: return NodalForcesAndMomentsC1.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private Enum Unit(ResultVector6 component) {
      Enum unit = ForceUnit.Kilonewton;
      if ((int)component > 3) {
        unit = MomentUnit.KilonewtonMeter;
      }
      return unit;
    }
  }
}
