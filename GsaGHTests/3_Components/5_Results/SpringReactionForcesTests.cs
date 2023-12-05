using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using GsaGHTests.Parameters.Results;
using System;
using Xunit;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using GsaGH.Helpers.GsaApi;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class SpringReactionForcesTests {
    private static readonly string NodeList = "1 to 4";

    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new SpringReactionForces();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void NodeReactionForceNodeIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SpringForces, 1);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new SpringReactionForces();
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
        var expectedIds = result.Model.Model.Nodes(NodeList).Keys.OrderBy(x => x).ToList();
        Assert.Equal(expectedIds[j], ids[j].Value);
      }
    }

    [Fact]
    public void NodeReactionForceNodeIdsFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SpringForces, 2);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new SpringReactionForces();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Equal(2, paths.Count);

        var cases = paths.Select(x => x.Indices[0]).ToList();
        foreach (int caseid in cases) {
          Assert.Equal(2, caseid);
        }

        var permutations = paths.Select(x => x.Indices[1]).ToList();
        for (int j = 0; j < permutations.Count; j++) {
          Assert.Equal(j + 1, permutations[j]);
        }
      }

      var ids = (IList<GH_Integer>)ComponentTestHelper.GetListOutput(comp, 8);
      for (int j = 0; j < ids.Count; j++) {
        // Assert element IDs
        var expectedIds = result.Model.Model.Nodes(NodeList).Keys.OrderBy(x => x).ToList();
        Assert.Equal(expectedIds[j], ids[j].Value);
      }
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void NodeReactionForceMaxFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SpringForces, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      var comp = new SpringReactionForces();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(Unit(component));
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void NodeReactionForceMaxFromCombinationCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SpringForces, 2);
      double expected = Math.Max(ExpectedCombinationCaseC2p1Values(component).Max(),
        ExpectedCombinationCaseC2p2Values(component).Max());

      // Act
      var comp = new SpringReactionForces();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(Unit(component));
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void NodeReactionForceMinFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SpringForces, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      var comp = new SpringReactionForces();
      comp.SetSelected(0, 9 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(Unit(component));
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void NodeReactionForceMinFromcombinationCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SpringForces, 2);
      double expected = Math.Min(ExpectedCombinationCaseC2p1Values(component).Min(),
        ExpectedCombinationCaseC2p2Values(component).Min());

      // Act
      var comp = new SpringReactionForces();
      comp.SetSelected(0, 9 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(Unit(component));
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void NodeReactionForceValuesFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SpringForces, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      var comp = new SpringReactionForces();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert result values
      int i = 0;
      foreach (IQuantity value in output) {
        double x = ResultHelper.RoundToSignificantDigits(value.As(Unit(component)), 4);
        Assert.Equal(expected[i++], x);
      }
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void NodeReactionForceValuesFromCombinationCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SpringForces, 2);
      List<double> expectedP1 = ExpectedCombinationCaseC2p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC2p2Values(component);

      // Act
      var comp = new SpringReactionForces();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);
      var p1 = new GH_Path(2, 1);
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component, p1);

      // Assert result values
      for (int i = 0; i < output.Count; i++) {
        double perm = ResultHelper.RoundToSignificantDigits(output[i].As(Unit(component)), 4);
        Assert.Equal(expectedP1[i], perm);
      }

      var p2 = new GH_Path(2, 2);
      output = ComponentTestHelper.GetResultOutput(comp, (int)component, p2);

      // Assert result values
      for (int i = 0; i < output.Count; i++) {
        double perm = ResultHelper.RoundToSignificantDigits(output[i].As(Unit(component)), 4);
        Assert.Equal(expectedP2[i], perm);
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return NodeSpringForcesA1.XInKiloNewtons();

        case ResultVector6HelperEnum.Y: return NodeSpringForcesA1.YInKiloNewtons();

        case ResultVector6HelperEnum.Z: return NodeSpringForcesA1.ZInKiloNewtons();

        case ResultVector6HelperEnum.Xyz: return NodeSpringForcesA1.XyzInKiloNewtons();

        case ResultVector6HelperEnum.Xx: return NodeSpringForcesA1.XxInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Yy: return NodeSpringForcesA1.YyInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Zz: return NodeSpringForcesA1.ZzInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Xxyyzz:
          return NodeSpringForcesA1.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p1Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return NodeSpringForcesC2p1.XInKiloNewtons();

        case ResultVector6HelperEnum.Y: return NodeSpringForcesC2p1.YInKiloNewtons();

        case ResultVector6HelperEnum.Z: return NodeSpringForcesC2p1.ZInKiloNewtons();

        case ResultVector6HelperEnum.Xyz: return NodeSpringForcesC2p1.XyzInKiloNewtons();

        case ResultVector6HelperEnum.Xx: return NodeSpringForcesC2p1.XxInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Yy: return NodeSpringForcesC2p1.YyInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Zz: return NodeSpringForcesC2p1.ZzInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Xxyyzz:
          return NodeSpringForcesC2p1.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p2Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return NodeSpringForcesC2p2.XInKiloNewtons();

        case ResultVector6HelperEnum.Y: return NodeSpringForcesC2p2.YInKiloNewtons();

        case ResultVector6HelperEnum.Z: return NodeSpringForcesC2p2.ZInKiloNewtons();

        case ResultVector6HelperEnum.Xyz: return NodeSpringForcesC2p2.XyzInKiloNewtons();

        case ResultVector6HelperEnum.Xx: return NodeSpringForcesC2p2.XxInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Yy: return NodeSpringForcesC2p2.YyInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Zz: return NodeSpringForcesC2p2.ZzInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Xxyyzz:
          return NodeSpringForcesC2p2.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private Enum Unit(ResultVector6HelperEnum component) {
      Enum unit = ForceUnit.Kilonewton;
      if ((int)component > 3) {
        unit = MomentUnit.KilonewtonMeter;
      }
      return unit;
    }
  }
}
