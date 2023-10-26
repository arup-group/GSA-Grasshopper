using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public partial class GsaNodeReactionForcesTests {
    public enum GsaNodeReactionForceComponent {
      X,
      Y,
      Z,
      Xyz,
      Xx,
      Yy,
      Zz,
      Xxyyzz,
    }

    private static readonly string NodeList = "1324 to 1327";

    [Fact]
    public void NodeReactionForceNodeIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, NodeExtremaVector6> resultSet
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
      INodeResultSubset<IInternalForce, NodeExtremaVector6> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert node IDs
      var expectedIds = result.Model.Model.Nodes(NodeList).Keys.ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(GsaNodeReactionForceComponent.X)]
    [InlineData(GsaNodeReactionForceComponent.Y)]
    [InlineData(GsaNodeReactionForceComponent.Z)]
    [InlineData(GsaNodeReactionForceComponent.Xyz)]
    [InlineData(GsaNodeReactionForceComponent.Xx)]
    [InlineData(GsaNodeReactionForceComponent.Yy)]
    [InlineData(GsaNodeReactionForceComponent.Zz)]
    [InlineData(GsaNodeReactionForceComponent.Xxyyzz)]
    public void NodeReactionForceMaxFromAnalysisCaseTest(GsaNodeReactionForceComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double? expected = ExpectedAnalysisCaseValues(component).Where(item => item != null)
       .Cast<double>().Max();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, NodeExtremaVector6> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert Max in set
      double max = ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(GsaNodeReactionForceComponent.X)]
    [InlineData(GsaNodeReactionForceComponent.Y)]
    [InlineData(GsaNodeReactionForceComponent.Z)]
    [InlineData(GsaNodeReactionForceComponent.Xyz)]
    [InlineData(GsaNodeReactionForceComponent.Xx)]
    [InlineData(GsaNodeReactionForceComponent.Yy)]
    [InlineData(GsaNodeReactionForceComponent.Zz)]
    [InlineData(GsaNodeReactionForceComponent.Xxyyzz)]
    public void NodeReactionForceMaxFromCombinationCaseTest(
      GsaNodeReactionForceComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(
        ExpectedCombinationCaseC4p1Values(component).Where(item => item != null).Cast<double>()
         .Max(),
        ExpectedCombinationCaseC4p2Values(component).Where(item => item != null).Cast<double>()
         .Max());
      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, NodeExtremaVector6> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert Max in set
      double max = ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(GsaNodeReactionForceComponent.X)]
    [InlineData(GsaNodeReactionForceComponent.Y)]
    [InlineData(GsaNodeReactionForceComponent.Z)]
    [InlineData(GsaNodeReactionForceComponent.Xyz)]
    [InlineData(GsaNodeReactionForceComponent.Xx)]
    [InlineData(GsaNodeReactionForceComponent.Yy)]
    [InlineData(GsaNodeReactionForceComponent.Zz)]
    [InlineData(GsaNodeReactionForceComponent.Xxyyzz)]
    public void NodeReactionForceMinFromAnalysisCaseTest(GsaNodeReactionForceComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Where(item => item != null)
       .Cast<double>().Min();
      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, NodeExtremaVector6> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert Max in set
      double min = ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(GsaNodeReactionForceComponent.X)]
    [InlineData(GsaNodeReactionForceComponent.Y)]
    [InlineData(GsaNodeReactionForceComponent.Z)]
    [InlineData(GsaNodeReactionForceComponent.Xyz)]
    [InlineData(GsaNodeReactionForceComponent.Xx)]
    [InlineData(GsaNodeReactionForceComponent.Yy)]
    [InlineData(GsaNodeReactionForceComponent.Zz)]
    [InlineData(GsaNodeReactionForceComponent.Xxyyzz)]
    public void NodeReactionForceMinFromcombinationCaseTest(
      GsaNodeReactionForceComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(
        ExpectedCombinationCaseC4p1Values(component).Where(item => item != null).Cast<double>()
         .Min(),
        ExpectedCombinationCaseC4p2Values(component).Where(item => item != null).Cast<double>()
         .Min());

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, NodeExtremaVector6> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert Max in set
      double min = ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(GsaNodeReactionForceComponent.X)]
    [InlineData(GsaNodeReactionForceComponent.Y)]
    [InlineData(GsaNodeReactionForceComponent.Z)]
    [InlineData(GsaNodeReactionForceComponent.Xyz)]
    [InlineData(GsaNodeReactionForceComponent.Xx)]
    [InlineData(GsaNodeReactionForceComponent.Yy)]
    [InlineData(GsaNodeReactionForceComponent.Zz)]
    [InlineData(GsaNodeReactionForceComponent.Xxyyzz)]
    public void NodeReactionForceValuesFromAnalysisCaseTest(
      GsaNodeReactionForceComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double?> expected = ExpectedAnalysisCaseValues(component);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, NodeExtremaVector6> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IInternalForce> reactionForceQuantity = resultSet.Subset[id];

        // for analysis case results we expect only one value in the collection
        Assert.Single(reactionForceQuantity);

        double x = ResultsHelper(reactionForceQuantity[0], component);
        Assert.Equal(expected[i++], x);
      }
    }

    [Theory]
    [InlineData(GsaNodeReactionForceComponent.X)]
    [InlineData(GsaNodeReactionForceComponent.Y)]
    [InlineData(GsaNodeReactionForceComponent.Z)]
    [InlineData(GsaNodeReactionForceComponent.Xyz)]
    [InlineData(GsaNodeReactionForceComponent.Xx)]
    [InlineData(GsaNodeReactionForceComponent.Yy)]
    [InlineData(GsaNodeReactionForceComponent.Zz)]
    [InlineData(GsaNodeReactionForceComponent.Xxyyzz)]
    public void NodeReactionForceValuesFromCombinationCaseTest(
      GsaNodeReactionForceComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double?> expectedP1 = ExpectedCombinationCaseC4p1Values(component);
      List<double?> expectedP2 = ExpectedCombinationCaseC4p2Values(component);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IInternalForce, NodeExtremaVector6> resultSet
        = result.NodeReactionForces.ResultSubset(nodeIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        var reactionForceQuantity = (Collection<IInternalForce>)resultSet.Subset[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, reactionForceQuantity.Count);

        double perm1 = ResultsHelper(reactionForceQuantity[0], component);
        Assert.Equal(expectedP1[i], perm1);
        double perm2 = ResultsHelper(reactionForceQuantity[1], component);
        Assert.Equal(expectedP2[i++], perm2);
      }
    }

    private List<double?> ExpectedAnalysisCaseValues(GsaNodeReactionForceComponent component) {
      switch (component) {
        case GsaNodeReactionForceComponent.X: return NodeReactionForcesA1.XInKiloNewtons();

        case GsaNodeReactionForceComponent.Y: return NodeReactionForcesA1.YInKiloNewtons();

        case GsaNodeReactionForceComponent.Z: return NodeReactionForcesA1.ZInKiloNewtons();

        case GsaNodeReactionForceComponent.Xyz: return NodeReactionForcesA1.XyzInKiloNewtons();

        case GsaNodeReactionForceComponent.Xx:
          return NodeReactionForcesA1.XxInKiloNewtonsPerMeter();

        case GsaNodeReactionForceComponent.Yy:
          return NodeReactionForcesA1.YyInKiloNewtonsPerMeter();

        case GsaNodeReactionForceComponent.Zz:
          return NodeReactionForcesA1.ZzInKiloNewtonsPerMeter();

        case GsaNodeReactionForceComponent.Xxyyzz:
          return NodeReactionForcesA1.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double?> ExpectedCombinationCaseC4p1Values(
      GsaNodeReactionForceComponent component) {
      switch (component) {
        case GsaNodeReactionForceComponent.X: return NodeReactionForcesC4p1.XInKiloNewtons();

        case GsaNodeReactionForceComponent.Y: return NodeReactionForcesC4p1.YInKiloNewtons();

        case GsaNodeReactionForceComponent.Z: return NodeReactionForcesC4p1.ZInKiloNewtons();

        case GsaNodeReactionForceComponent.Xyz: return NodeReactionForcesC4p1.XyzInKiloNewtons();

        case GsaNodeReactionForceComponent.Xx:
          return NodeReactionForcesC4p1.XxInKiloNewtonsPerMeter();

        case GsaNodeReactionForceComponent.Yy:
          return NodeReactionForcesC4p1.YyInKiloNewtonsPerMeter();

        case GsaNodeReactionForceComponent.Zz:
          return NodeReactionForcesC4p1.ZzInKiloNewtonsPerMeter();

        case GsaNodeReactionForceComponent.Xxyyzz:
          return NodeReactionForcesC4p1.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double?> ExpectedCombinationCaseC4p2Values(
      GsaNodeReactionForceComponent component) {
      switch (component) {
        case GsaNodeReactionForceComponent.X: return NodeReactionForcesC4p2.XInKiloNewtons();

        case GsaNodeReactionForceComponent.Y: return NodeReactionForcesC4p2.YInKiloNewtons();

        case GsaNodeReactionForceComponent.Z: return NodeReactionForcesC4p2.ZInKiloNewtons();

        case GsaNodeReactionForceComponent.Xyz: return NodeReactionForcesC4p2.XyzInKiloNewtons();

        case GsaNodeReactionForceComponent.Xx:
          return NodeReactionForcesC4p2.XxInKiloNewtonsPerMeter();

        case GsaNodeReactionForceComponent.Yy:
          return NodeReactionForcesC4p2.YyInKiloNewtonsPerMeter();

        case GsaNodeReactionForceComponent.Zz:
          return NodeReactionForcesC4p2.ZzInKiloNewtonsPerMeter();

        case GsaNodeReactionForceComponent.Xxyyzz:
          return NodeReactionForcesC4p2.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private double ResultsHelper(
      INodeResultSubset<IInternalForce, NodeExtremaVector6> result,
      GsaNodeReactionForceComponent component, bool max) {
      double d = 0;
      NodeExtremaVector6 extrema = max ? result.Max : result.Min;
      switch (component) {
        case GsaNodeReactionForceComponent.X:
          d = result.GetExtrema(extrema.X).X.Kilonewtons;
          break;

        case GsaNodeReactionForceComponent.Y:
          d = result.GetExtrema(extrema.Y).Y.Kilonewtons;
          break;

        case GsaNodeReactionForceComponent.Z:
          d = result.GetExtrema(extrema.Z).Z.Kilonewtons;
          break;

        case GsaNodeReactionForceComponent.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Kilonewtons;
          break;

        case GsaNodeReactionForceComponent.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.KilonewtonMeters;
          break;

        case GsaNodeReactionForceComponent.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.KilonewtonMeters;
          break;

        case GsaNodeReactionForceComponent.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.KilonewtonMeters;
          break;

        case GsaNodeReactionForceComponent.Xxyyzz:
          d = result.GetExtrema(extrema.Xxyyzz).Xxyyzz.KilonewtonMeters;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    private double ResultsHelper(IInternalForce result, GsaNodeReactionForceComponent component) {
      double d = 0;
      switch (component) {
        case GsaNodeReactionForceComponent.X:
          d = result.X.Kilonewtons;
          break;

        case GsaNodeReactionForceComponent.Y:
          d = result.Y.Kilonewtons;
          break;

        case GsaNodeReactionForceComponent.Z:
          d = result.Z.Kilonewtons;
          break;

        case GsaNodeReactionForceComponent.Xyz:
          d = result.Xyz.Kilonewtons;
          break;

        case GsaNodeReactionForceComponent.Xx:
          d = result.Xx.KilonewtonMeters;
          break;

        case GsaNodeReactionForceComponent.Yy:
          d = result.Yy.KilonewtonMeters;
          break;

        case GsaNodeReactionForceComponent.Zz:
          d = result.Zz.KilonewtonMeters;
          break;

        case GsaNodeReactionForceComponent.Xxyyzz:
          d = result.Xxyyzz.KilonewtonMeters;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }
  }
}
