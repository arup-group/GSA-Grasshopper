using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Helpers;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class NodeReactionForcesTests {
    private static readonly string NodeList = "1324 to 1327";

    [Fact]
    public void NodeReactionForceNodeIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> resultSet
      = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert node IDs
      var expectedIds = result.Model.ApiModel.Nodes(NodeList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void NodeReactionForceNodeIdsFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert node IDs
      var expectedIds = result.Model.ApiModel.Nodes(NodeList).Keys.ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
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
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert Max in set
      double? max = TestsResultHelper.ResultsHelper(resultSet, component, true);
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
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert Max in set
      double? max = TestsResultHelper.ResultsHelper(resultSet, component, true);
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
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert Max in set
      double? min = TestsResultHelper.ResultsHelper(resultSet, component, false);
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
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert Max in set
      double? min = TestsResultHelper.ResultsHelper(resultSet, component, false);
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
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IReactionForce> reactionForceQuantity = resultSet.Subset[id];

        // for analysis case results we expect only one value in the collection
        Assert.Single(reactionForceQuantity);

        double? x = TestsResultHelper.ResultsHelper(reactionForceQuantity[0], component);
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
    public void NodeReactionForceValuesFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double?> expectedP1 = ExpectedCombinationCaseC4p1Values(component);
      List<double?> expectedP2 = ExpectedCombinationCaseC4p2Values(component);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IReactionForce> reactionForceQuantity = resultSet.Subset[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, reactionForceQuantity.Count);

        double? perm1 = TestsResultHelper.ResultsHelper(reactionForceQuantity[0], component);
        Assert.Equal(expectedP1[i], perm1, DoubleComparer.Default);
        double? perm2 = TestsResultHelper.ResultsHelper(reactionForceQuantity[1], component);
        Assert.Equal(expectedP2[i++], perm2, DoubleComparer.Default);
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


  }
}
