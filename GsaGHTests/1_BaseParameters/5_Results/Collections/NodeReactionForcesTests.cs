using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public partial class GsaNodeReactionForcesTests {
    private static readonly string NodeList = "1324 to 1327";

    [Fact]
    public void NodeReactionForceNodeIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert node IDs
      var expectedIds = result.Model.Model.Nodes(NodeList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void NodeReactionForceNodeIdsFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert node IDs
      var expectedIds = result.Model.Model.Nodes(NodeList).Keys.ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
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
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
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
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(ExpectedCombinationCaseC4p1Values(component).Max(),
        ExpectedCombinationCaseC4p2Values(component).Max());
      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
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
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();
      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
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
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(ExpectedCombinationCaseC4p1Values(component).Min(),
        ExpectedCombinationCaseC4p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
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
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IInternalForce> reactionForceQuantity = resultSet.Subset[id];

        // for analysis case results we expect only one value in the collection
        Assert.Single(reactionForceQuantity);

        double x = TestsResultHelper.ResultsHelper(reactionForceQuantity[0], component);
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
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = ExpectedCombinationCaseC4p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC4p2Values(component);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IInternalForce> reactionForceQuantity = resultSet.Subset[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, reactionForceQuantity.Count);

        double perm1 = TestsResultHelper.ResultsHelper(reactionForceQuantity[0], component);
        Assert.Equal(expectedP1[i], perm1);
        double perm2 = TestsResultHelper.ResultsHelper(reactionForceQuantity[1], component);
        Assert.Equal(expectedP2[i++], perm2);
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return NodeReactionForcesA1.XInKiloNewtons();

        case ResultVector6HelperEnum.Y: return NodeReactionForcesA1.YInKiloNewtons();

        case ResultVector6HelperEnum.Z: return NodeReactionForcesA1.ZInKiloNewtons();

        case ResultVector6HelperEnum.Xyz: return NodeReactionForcesA1.XyzInKiloNewtons();

        case ResultVector6HelperEnum.Xx: return NodeReactionForcesA1.XxInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Yy: return NodeReactionForcesA1.YyInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Zz: return NodeReactionForcesA1.ZzInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Xxyyzz:
          return NodeReactionForcesA1.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p1Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return NodeReactionForcesC4p1.XInKiloNewtons();

        case ResultVector6HelperEnum.Y: return NodeReactionForcesC4p1.YInKiloNewtons();

        case ResultVector6HelperEnum.Z: return NodeReactionForcesC4p1.ZInKiloNewtons();

        case ResultVector6HelperEnum.Xyz: return NodeReactionForcesC4p1.XyzInKiloNewtons();

        case ResultVector6HelperEnum.Xx: return NodeReactionForcesC4p1.XxInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Yy: return NodeReactionForcesC4p1.YyInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Zz: return NodeReactionForcesC4p1.ZzInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Xxyyzz:
          return NodeReactionForcesC4p1.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p2Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return NodeReactionForcesC4p2.XInKiloNewtons();

        case ResultVector6HelperEnum.Y: return NodeReactionForcesC4p2.YInKiloNewtons();

        case ResultVector6HelperEnum.Z: return NodeReactionForcesC4p2.ZInKiloNewtons();

        case ResultVector6HelperEnum.Xyz: return NodeReactionForcesC4p2.XyzInKiloNewtons();

        case ResultVector6HelperEnum.Xx: return NodeReactionForcesC4p2.XxInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Yy: return NodeReactionForcesC4p2.YyInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Zz: return NodeReactionForcesC4p2.ZzInKiloNewtonsPerMeter();

        case ResultVector6HelperEnum.Xxyyzz:
          return NodeReactionForcesC4p2.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }
  }
}
