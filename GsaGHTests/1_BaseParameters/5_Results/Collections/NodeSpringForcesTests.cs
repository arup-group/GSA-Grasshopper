using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public partial class GsaNodeSpringForcesTests {

    private static readonly string NodeList = "1 to 4";

    [Fact]
    public void NodeReactionForceNodeIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SpringForces, 1);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeSpringForces.ResultSubset(nodeIds);

      // Assert node IDs
      var expectedIds = result.Model.Model.Nodes(NodeList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void NodeReactionForceNodeIdsFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SpringForces, 2);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeSpringForces.ResultSubset(nodeIds);

      // Assert node IDs
      var expectedIds = result.Model.Model.Nodes(NodeList).Keys.ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(NodeComponentHelperEnum.X)]
    [InlineData(NodeComponentHelperEnum.Y)]
    [InlineData(NodeComponentHelperEnum.Z)]
    [InlineData(NodeComponentHelperEnum.Xyz)]
    [InlineData(NodeComponentHelperEnum.Xx)]
    [InlineData(NodeComponentHelperEnum.Yy)]
    [InlineData(NodeComponentHelperEnum.Zz)]
    [InlineData(NodeComponentHelperEnum.Xxyyzz)]
    public void NodeReactionForceMaxFromAnalysisCaseTest(NodeComponentHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SpringForces, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeSpringForces.ResultSubset(nodeIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(NodeComponentHelperEnum.X)]
    [InlineData(NodeComponentHelperEnum.Y)]
    [InlineData(NodeComponentHelperEnum.Z)]
    [InlineData(NodeComponentHelperEnum.Xyz)]
    [InlineData(NodeComponentHelperEnum.Xx)]
    [InlineData(NodeComponentHelperEnum.Yy)]
    [InlineData(NodeComponentHelperEnum.Zz)]
    [InlineData(NodeComponentHelperEnum.Xxyyzz)]
    public void NodeReactionForceMaxFromCombinationCaseTest(NodeComponentHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SpringForces, 2);
      double expected = Math.Max(ExpectedCombinationCaseC2p1Values(component).Max(),
        ExpectedCombinationCaseC2p2Values(component).Max());
      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeSpringForces.ResultSubset(nodeIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(NodeComponentHelperEnum.X)]
    [InlineData(NodeComponentHelperEnum.Y)]
    [InlineData(NodeComponentHelperEnum.Z)]
    [InlineData(NodeComponentHelperEnum.Xyz)]
    [InlineData(NodeComponentHelperEnum.Xx)]
    [InlineData(NodeComponentHelperEnum.Yy)]
    [InlineData(NodeComponentHelperEnum.Zz)]
    [InlineData(NodeComponentHelperEnum.Xxyyzz)]
    public void NodeReactionForceMinFromAnalysisCaseTest(NodeComponentHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SpringForces, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();
      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeSpringForces.ResultSubset(nodeIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(NodeComponentHelperEnum.X)]
    [InlineData(NodeComponentHelperEnum.Y)]
    [InlineData(NodeComponentHelperEnum.Z)]
    [InlineData(NodeComponentHelperEnum.Xyz)]
    [InlineData(NodeComponentHelperEnum.Xx)]
    [InlineData(NodeComponentHelperEnum.Yy)]
    [InlineData(NodeComponentHelperEnum.Zz)]
    [InlineData(NodeComponentHelperEnum.Xxyyzz)]
    public void NodeReactionForceMinFromcombinationCaseTest(NodeComponentHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SpringForces, 2);
      double expected = Math.Min(ExpectedCombinationCaseC2p1Values(component).Min(),
        ExpectedCombinationCaseC2p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeSpringForces.ResultSubset(nodeIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(NodeComponentHelperEnum.X)]
    [InlineData(NodeComponentHelperEnum.Y)]
    [InlineData(NodeComponentHelperEnum.Z)]
    [InlineData(NodeComponentHelperEnum.Xyz)]
    [InlineData(NodeComponentHelperEnum.Xx)]
    [InlineData(NodeComponentHelperEnum.Yy)]
    [InlineData(NodeComponentHelperEnum.Zz)]
    [InlineData(NodeComponentHelperEnum.Xxyyzz)]
    public void NodeReactionForceValuesFromAnalysisCaseTest(NodeComponentHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SpringForces, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeSpringForces.ResultSubset(nodeIds);

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
    [InlineData(NodeComponentHelperEnum.X)]
    [InlineData(NodeComponentHelperEnum.Y)]
    [InlineData(NodeComponentHelperEnum.Z)]
    [InlineData(NodeComponentHelperEnum.Xyz)]
    [InlineData(NodeComponentHelperEnum.Xx)]
    [InlineData(NodeComponentHelperEnum.Yy)]
    [InlineData(NodeComponentHelperEnum.Zz)]
    [InlineData(NodeComponentHelperEnum.Xxyyzz)]
    public void NodeReactionForceValuesFromCombinationCaseTest(NodeComponentHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SpringForces, 2);
      List<double> expectedP1 = ExpectedCombinationCaseC2p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC2p2Values(component);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeSpringForces.ResultSubset(nodeIds);

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

    private List<double> ExpectedAnalysisCaseValues(NodeComponentHelperEnum component) {
      switch (component) {
        case NodeComponentHelperEnum.X: return NodeSpringForcesA1.XInKiloNewtons();

        case NodeComponentHelperEnum.Y: return NodeSpringForcesA1.YInKiloNewtons();

        case NodeComponentHelperEnum.Z: return NodeSpringForcesA1.ZInKiloNewtons();

        case NodeComponentHelperEnum.Xyz: return NodeSpringForcesA1.XyzInKiloNewtons();

        case NodeComponentHelperEnum.Xx: return NodeSpringForcesA1.XxInKiloNewtonsPerMeter();

        case NodeComponentHelperEnum.Yy: return NodeSpringForcesA1.YyInKiloNewtonsPerMeter();

        case NodeComponentHelperEnum.Zz: return NodeSpringForcesA1.ZzInKiloNewtonsPerMeter();

        case NodeComponentHelperEnum.Xxyyzz:
          return NodeSpringForcesA1.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p1Values(NodeComponentHelperEnum component) {
      switch (component) {
        case NodeComponentHelperEnum.X: return NodeSpringForcesC2p1.XInKiloNewtons();

        case NodeComponentHelperEnum.Y: return NodeSpringForcesC2p1.YInKiloNewtons();

        case NodeComponentHelperEnum.Z: return NodeSpringForcesC2p1.ZInKiloNewtons();

        case NodeComponentHelperEnum.Xyz: return NodeSpringForcesC2p1.XyzInKiloNewtons();

        case NodeComponentHelperEnum.Xx: return NodeSpringForcesC2p1.XxInKiloNewtonsPerMeter();

        case NodeComponentHelperEnum.Yy: return NodeSpringForcesC2p1.YyInKiloNewtonsPerMeter();

        case NodeComponentHelperEnum.Zz: return NodeSpringForcesC2p1.ZzInKiloNewtonsPerMeter();

        case NodeComponentHelperEnum.Xxyyzz:
          return NodeSpringForcesC2p1.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p2Values(NodeComponentHelperEnum component) {
      switch (component) {
        case NodeComponentHelperEnum.X: return NodeSpringForcesC2p2.XInKiloNewtons();

        case NodeComponentHelperEnum.Y: return NodeSpringForcesC2p2.YInKiloNewtons();

        case NodeComponentHelperEnum.Z: return NodeSpringForcesC2p2.ZInKiloNewtons();

        case NodeComponentHelperEnum.Xyz: return NodeSpringForcesC2p2.XyzInKiloNewtons();

        case NodeComponentHelperEnum.Xx: return NodeSpringForcesC2p2.XxInKiloNewtonsPerMeter();

        case NodeComponentHelperEnum.Yy: return NodeSpringForcesC2p2.YyInKiloNewtonsPerMeter();

        case NodeComponentHelperEnum.Zz: return NodeSpringForcesC2p2.ZzInKiloNewtonsPerMeter();

        case NodeComponentHelperEnum.Xxyyzz:
          return NodeSpringForcesC2p2.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }
  }
}
