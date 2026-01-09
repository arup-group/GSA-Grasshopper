using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Helpers;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters;
using GsaGHTests.Parameters.Results;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

using GsaResultTests = GsaGHTests.Parameters.Results.GsaResultTests;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class ReactionForcesTests {
    private static readonly string NodeList = "1324 to 1327";

    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new ReactionForces();
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
      var comp = new ReactionForces();
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

      var ids = (IList<GH_Integer>)ComponentTestHelper.GetListOutput(comp, 8);
      for (int j = 0; j < ids.Count; j++) {
        // Assert element IDs
        var expectedIds = result.Model.ApiModel.Nodes(NodeList).Keys.ToList();
        Assert.Contains<int>(expectedIds[j], ids.Select(i => i.Value));
      }
    }

    [Fact]
    public void NodeReactionForceNodeIdsFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new ReactionForces();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Equal(2, paths.Count);

        var cases = paths.Select(x => x.Indices[0]).ToList();
        foreach (int caseid in cases) {
          Assert.Equal(4, caseid);
        }

        var permutations = paths.Select(x => x.Indices[1]).ToList();
        for (int j = 0; j < permutations.Count; j++) {
          Assert.Equal(j + 1, permutations[j]);
        }
      }

      var ids = (IList<GH_Integer>)ComponentTestHelper.GetListOutput(comp, 8);
      for (int j = 0; j < ids.Count; j++) {
        // Assert element IDs
        var expectedIds = result.Model.ApiModel.Nodes(NodeList).Keys.OrderBy(x => x).ToList();
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
    public void NodeReactionForceMaxFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double? expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      var comp = new ReactionForces();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(Unit(component));
      Assert.Equal(expected, max, DoubleComparer.Default);
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
    public void NodeReactionForceMaxFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      var values = new List<double?>();
      values.AddRange(ExpectedCombinationCaseC4p1Values(component));
      values.AddRange(ExpectedCombinationCaseC4p2Values(component));
      double? expected = MaxMinHelper.Max(values);

      // Act
      var comp = new ReactionForces();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(Unit(component));
      Assert.Equal(expected, max, DoubleComparer.Default);
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
    public void NodeReactionForceMinFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double? expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      var comp = new ReactionForces();
      comp.SetSelected(0, 9 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(Unit(component));
      Assert.Equal(expected, min, DoubleComparer.Default);
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
    public void NodeReactionForceMinFromcombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      var values = new List<double?>();
      values.AddRange(ExpectedCombinationCaseC4p1Values(component));
      values.AddRange(ExpectedCombinationCaseC4p2Values(component));
      double? expected = MaxMinHelper.Min(values);

      // Act
      var comp = new ReactionForces();
      comp.SetSelected(0, 9 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(Unit(component));
      Assert.Equal(expected, min, DoubleComparer.Default);
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
      List<double?> expected = ExpectedAnalysisCaseValues(component);

      // Act
      var comp = new ReactionForces();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert result values
      int i = 0;
      foreach (IQuantity value in output) {
        if (expected[i] == null) {
          Assert.Null(value);
        } else {
          double? x = value.As(Unit(component));
          Assert.Equal(expected[i], x, DoubleComparer.Default);
        }
        i++;
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
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double?> expectedP1 = ExpectedCombinationCaseC4p1Values(component);
      List<double?> expectedP2 = ExpectedCombinationCaseC4p2Values(component);

      // Act
      var comp = new ReactionForces();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);
      var p1 = new GH_Path(4, 1);
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component, p1);

      // Assert result values
      for (int i = 0; i < output.Count; i++) {
        if (expectedP1[i] == null) {
          Assert.Null(output[i]);
        } else {
          double? perm = output[i].As(Unit(component));
          Assert.Equal(expectedP1[i], perm, DoubleComparer.Default);
        }
      }

      var p2 = new GH_Path(4, 2);
      output = ComponentTestHelper.GetResultOutput(comp, (int)component, p2);

      // Assert result values
      for (int i = 0; i < output.Count; i++) {
        if (expectedP2[i] == null) {
          Assert.Null(output[i]);
        } else {
          double? perm = output[i].As(Unit(component));
          Assert.Equal(expectedP2[i], perm, DoubleComparer.Default);
        }
      }
    }

    private List<double?> ExpectedAnalysisCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return NodeReactionForcesA1.XInKiloNewtons();

        case ResultVector6.Y: return NodeReactionForcesA1.YInKiloNewtons();

        case ResultVector6.Z: return NodeReactionForcesA1.ZInKiloNewtons();

        case ResultVector6.Xyz: return NodeReactionForcesA1.YzInKiloNewtons();

        case ResultVector6.Xx: return NodeReactionForcesA1.XxInKiloNewtonsPerMeter();

        case ResultVector6.Yy: return NodeReactionForcesA1.YyInKiloNewtonsPerMeter();

        case ResultVector6.Zz: return NodeReactionForcesA1.ZzInKiloNewtonsPerMeter();

        case ResultVector6.Xxyyzz:
          return NodeReactionForcesA1.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double?> ExpectedCombinationCaseC4p1Values(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return NodeReactionForcesC4p1.XInKiloNewtons();

        case ResultVector6.Y: return NodeReactionForcesC4p1.YInKiloNewtons();

        case ResultVector6.Z: return NodeReactionForcesC4p1.ZInKiloNewtons();

        case ResultVector6.Xyz: return NodeReactionForcesC4p1.YzInKiloNewtons();

        case ResultVector6.Xx: return NodeReactionForcesC4p1.XxInKiloNewtonsPerMeter();

        case ResultVector6.Yy: return NodeReactionForcesC4p1.YyInKiloNewtonsPerMeter();

        case ResultVector6.Zz: return NodeReactionForcesC4p1.ZzInKiloNewtonsPerMeter();

        case ResultVector6.Xxyyzz:
          return NodeReactionForcesC4p1.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double?> ExpectedCombinationCaseC4p2Values(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return NodeReactionForcesC4p2.XInKiloNewtons();

        case ResultVector6.Y: return NodeReactionForcesC4p2.YInKiloNewtons();

        case ResultVector6.Z: return NodeReactionForcesC4p2.ZInKiloNewtons();

        case ResultVector6.Xyz: return NodeReactionForcesC4p2.YzInKiloNewtons();

        case ResultVector6.Xx: return NodeReactionForcesC4p2.XxInKiloNewtonsPerMeter();

        case ResultVector6.Yy: return NodeReactionForcesC4p2.YyInKiloNewtonsPerMeter();

        case ResultVector6.Zz: return NodeReactionForcesC4p2.ZzInKiloNewtonsPerMeter();

        case ResultVector6.Xxyyzz:
          return NodeReactionForcesC4p2.XxyyzzInKiloNewtonsPerMeter();
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
