using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters.Results;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

using NodeDisplacements = GsaGH.Components.NodeDisplacements;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class NodeDisplacementsTests {

    private static readonly string NodeList = "442 to 468";

    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new NodeDisplacements();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void NodeDisplacementsNodeIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new NodeDisplacements();
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
        var expectedIds = result.Model.ApiModel.Nodes(NodeList).Keys.OrderBy(x => x).ToList();
        Assert.Equal(expectedIds[j], ids[j].Value);
      }
    }

    [Fact]
    public void NodeDisplacementsNodeIdsFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      var comp = new NodeDisplacements();
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
    public void NodeDisplacementsMaxFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      var comp = new NodeDisplacements();
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
    public void NodeDisplacementsMaxFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(ExpectedCombinationCaseC4p1Values(component).Max(),
        ExpectedCombinationCaseC4p2Values(component).Max());

      // Act
      var comp = new NodeDisplacements();
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
    public void NodeDisplacementsMinFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      var comp = new NodeDisplacements();
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
    public void NodeDisplacementsMinFromcombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(ExpectedCombinationCaseC4p1Values(component).Min(),
        ExpectedCombinationCaseC4p2Values(component).Min());

      // Act
      var comp = new NodeDisplacements();
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
    public void NodeDisplacementsValuesFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      var comp = new NodeDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert result values
      int i = 0;
      foreach (IQuantity value in output) {
        double x = value.As(Unit(component));
        Assert.Equal(expected[i++], x, DoubleComparer.Default);
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
    public void NodeDisplacementsValuesFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = ExpectedCombinationCaseC4p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC4p2Values(component);

      // Act
      var comp = new NodeDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, NodeList, 1);
      var p1 = new GH_Path(4, 1);
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component, p1);

      // Assert result values
      for (int i = 0; i < output.Count; i++) {
        double perm = output[i].As(Unit(component));
        Assert.Equal(expectedP1[i], perm, DoubleComparer.Default);
      }

      var p2 = new GH_Path(4, 2);
      output = ComponentTestHelper.GetResultOutput(comp, (int)component, p2);

      // Assert result values
      for (int i = 0; i < output.Count; i++) {
        double perm = output[i].As(Unit(component));
        Assert.Equal(expectedP2[i], perm, DoubleComparer.Default);
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return NodeDisplacementsA1.XInMillimeter();

        case ResultVector6.Y: return NodeDisplacementsA1.YInMillimeter();

        case ResultVector6.Z: return NodeDisplacementsA1.ZInMillimeter();

        case ResultVector6.Xyz: return NodeDisplacementsA1.XyzInMillimeter();

        case ResultVector6.Xx: return NodeDisplacementsA1.XxInRadian();

        case ResultVector6.Yy: return NodeDisplacementsA1.YyInRadian();

        case ResultVector6.Zz: return NodeDisplacementsA1.ZzInRadian();

        case ResultVector6.Xxyyzz: return NodeDisplacementsA1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p1Values(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return NodeDisplacementsC4p1.XInMillimeter();

        case ResultVector6.Y: return NodeDisplacementsC4p1.YInMillimeter();

        case ResultVector6.Z: return NodeDisplacementsC4p1.ZInMillimeter();

        case ResultVector6.Xyz: return NodeDisplacementsC4p1.XyzInMillimeter();

        case ResultVector6.Xx: return NodeDisplacementsC4p1.XxInRadian();

        case ResultVector6.Yy: return NodeDisplacementsC4p1.YyInRadian();

        case ResultVector6.Zz: return NodeDisplacementsC4p1.ZzInRadian();

        case ResultVector6.Xxyyzz: return NodeDisplacementsC4p1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p2Values(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return NodeDisplacementsC4p2.XInMillimeter();

        case ResultVector6.Y: return NodeDisplacementsC4p2.YInMillimeter();

        case ResultVector6.Z: return NodeDisplacementsC4p2.ZInMillimeter();

        case ResultVector6.Xyz: return NodeDisplacementsC4p2.XyzInMillimeter();

        case ResultVector6.Xx: return NodeDisplacementsC4p2.XxInRadian();

        case ResultVector6.Yy: return NodeDisplacementsC4p2.YyInRadian();

        case ResultVector6.Zz: return NodeDisplacementsC4p2.ZzInRadian();

        case ResultVector6.Xxyyzz: return NodeDisplacementsC4p2.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private Enum Unit(ResultVector6 component) {
      Enum unit = LengthUnit.Millimeter;
      if ((int)component > 3) {
        unit = AngleUnit.Radian;
      }
      return unit;
    }
  }
}
