using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using OasysUnits;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public partial class NodeDisplacementsTests {
    public enum NodeDisplacementComponent {
      X, 
      Y, 
      Z, 
      Xyz, 
      Xx, 
      Yy, 
      Zz, 
      Xxyyzz
    }

    [Fact]
    public void NodeDisplacementsNodeIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, NodeExtremaVector6> resultSet = 
        result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert node IDs
      var expectedIds = result.Model.Model.Nodes(NodeList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void NodeDisplacementsNodeIdsFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, NodeExtremaVector6> resultSet = 
        result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert node IDs
      var expectedIds = result.Model.Model.Nodes(NodeList).Keys.ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(NodeDisplacementComponent.X)]
    [InlineData(NodeDisplacementComponent.Y)]
    [InlineData(NodeDisplacementComponent.Z)]
    [InlineData(NodeDisplacementComponent.Xyz)]
    [InlineData(NodeDisplacementComponent.Xx)]
    [InlineData(NodeDisplacementComponent.Yy)]
    [InlineData(NodeDisplacementComponent.Zz)]
    [InlineData(NodeDisplacementComponent.Xxyyzz)]
    public void NodeDisplacementsMaxFromAnalysisCaseTest(NodeDisplacementComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, NodeExtremaVector6> resultSet = 
        result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert Max in set
      double max = ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(NodeDisplacementComponent.X)]
    [InlineData(NodeDisplacementComponent.Y)]
    [InlineData(NodeDisplacementComponent.Z)]
    [InlineData(NodeDisplacementComponent.Xyz)]
    [InlineData(NodeDisplacementComponent.Xx)]
    [InlineData(NodeDisplacementComponent.Yy)]
    [InlineData(NodeDisplacementComponent.Zz)]
    [InlineData(NodeDisplacementComponent.Xxyyzz)]
    public void NodeDisplacementsMaxFromCombinationCaseTest(NodeDisplacementComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(
        ExpectedCombinationCaseC4p1Values(component).Max(),
        ExpectedCombinationCaseC4p2Values(component).Max());

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, NodeExtremaVector6> resultSet = 
        result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert Max in set
      double max = ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(NodeDisplacementComponent.X)]
    [InlineData(NodeDisplacementComponent.Y)]
    [InlineData(NodeDisplacementComponent.Z)]
    [InlineData(NodeDisplacementComponent.Xyz)]
    [InlineData(NodeDisplacementComponent.Xx)]
    [InlineData(NodeDisplacementComponent.Yy)]
    [InlineData(NodeDisplacementComponent.Zz)]
    [InlineData(NodeDisplacementComponent.Xxyyzz)]
    public void NodeDisplacementsMinFromAnalysisCaseTest(NodeDisplacementComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, NodeExtremaVector6> resultSet = 
        result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert Max in set
      double min = ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(NodeDisplacementComponent.X)]
    [InlineData(NodeDisplacementComponent.Y)]
    [InlineData(NodeDisplacementComponent.Z)]
    [InlineData(NodeDisplacementComponent.Xyz)]
    [InlineData(NodeDisplacementComponent.Xx)]
    [InlineData(NodeDisplacementComponent.Yy)]
    [InlineData(NodeDisplacementComponent.Zz)]
    [InlineData(NodeDisplacementComponent.Xxyyzz)]
    public void NodeDisplacementsMinFromcombinationCaseTest(NodeDisplacementComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(
        ExpectedCombinationCaseC4p1Values(component).Min(),
        ExpectedCombinationCaseC4p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, NodeExtremaVector6> resultSet = 
        result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert Max in set
      double min = ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(NodeDisplacementComponent.X)]
    [InlineData(NodeDisplacementComponent.Y)]
    [InlineData(NodeDisplacementComponent.Z)]
    [InlineData(NodeDisplacementComponent.Xyz)]
    [InlineData(NodeDisplacementComponent.Xx)]
    [InlineData(NodeDisplacementComponent.Yy)]
    [InlineData(NodeDisplacementComponent.Zz)]
    [InlineData(NodeDisplacementComponent.Xxyyzz)]
    public void NodeDisplacementsValuesFromAnalysisCaseTest(NodeDisplacementComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, NodeExtremaVector6> resultSet = 
        result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IDisplacement> displacementQuantity = resultSet.Subset[id];
        
        // for analysis case results we expect only one value in the collection
        Assert.Single(displacementQuantity);

        double x = ResultsHelper(displacementQuantity[0], component);
        Assert.Equal(expected[i++], x);
      }
    }

    [Theory]
    [InlineData(NodeDisplacementComponent.X)]
    [InlineData(NodeDisplacementComponent.Y)]
    [InlineData(NodeDisplacementComponent.Z)]
    [InlineData(NodeDisplacementComponent.Xyz)]
    [InlineData(NodeDisplacementComponent.Xx)]
    [InlineData(NodeDisplacementComponent.Yy)]
    [InlineData(NodeDisplacementComponent.Zz)]
    [InlineData(NodeDisplacementComponent.Xxyyzz)]
    public void NodeDisplacementsValuesFromCombinationCaseTest(NodeDisplacementComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = ExpectedCombinationCaseC4p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC4p2Values(component);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, NodeExtremaVector6> resultSet = result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        var displacementQuantity = (Collection<IDisplacement>)resultSet.Subset[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, displacementQuantity.Count);

        double perm1 = ResultsHelper(displacementQuantity[0], component);
        Assert.Equal(expectedP1[i], perm1);
        double perm2 = ResultsHelper(displacementQuantity[1], component);
        Assert.Equal(expectedP2[i++], perm2);
      }
    }

    private List<double> ExpectedAnalysisCaseValues(NodeDisplacementComponent component) {
      switch (component) {
        case NodeDisplacementComponent.X:
          return NodeDisplacementsA1.XInMillimeter();

        case NodeDisplacementComponent.Y:
          return NodeDisplacementsA1.YInMillimeter();

        case NodeDisplacementComponent.Z:
          return NodeDisplacementsA1.ZInMillimeter();

        case NodeDisplacementComponent.Xyz:
          return NodeDisplacementsA1.XyzInMillimeter();

        case NodeDisplacementComponent.Xx:
          return NodeDisplacementsA1.XxInRadian();

        case NodeDisplacementComponent.Yy:
          return NodeDisplacementsA1.YyInRadian();

        case NodeDisplacementComponent.Zz:
          return NodeDisplacementsA1.ZzInRadian();

        case NodeDisplacementComponent.Xxyyzz:
          return NodeDisplacementsA1.XxyyzzInRadian();
      }
      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p1Values(NodeDisplacementComponent component) {
      switch (component) {
        case NodeDisplacementComponent.X:
          return NodeDisplacementsC4p1.XInMillimeter();

        case NodeDisplacementComponent.Y:
          return NodeDisplacementsC4p1.YInMillimeter();

        case NodeDisplacementComponent.Z:
          return NodeDisplacementsC4p1.ZInMillimeter();

        case NodeDisplacementComponent.Xyz:
          return NodeDisplacementsC4p1.XyzInMillimeter();

        case NodeDisplacementComponent.Xx:
          return NodeDisplacementsC4p1.XxInRadian();

        case NodeDisplacementComponent.Yy:
          return NodeDisplacementsC4p1.YyInRadian();

        case NodeDisplacementComponent.Zz:
          return NodeDisplacementsC4p1.ZzInRadian();

        case NodeDisplacementComponent.Xxyyzz:
          return NodeDisplacementsC4p1.XxyyzzInRadian();
      }
      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p2Values(NodeDisplacementComponent component) {
      switch (component) {
        case NodeDisplacementComponent.X:
          return NodeDisplacementsC4p2.XInMillimeter();

        case NodeDisplacementComponent.Y:
          return NodeDisplacementsC4p2.YInMillimeter();

        case NodeDisplacementComponent.Z:
          return NodeDisplacementsC4p2.ZInMillimeter();

        case NodeDisplacementComponent.Xyz:
          return NodeDisplacementsC4p2.XyzInMillimeter();

        case NodeDisplacementComponent.Xx:
          return NodeDisplacementsC4p2.XxInRadian();

        case NodeDisplacementComponent.Yy:
          return NodeDisplacementsC4p2.YyInRadian();

        case NodeDisplacementComponent.Zz:
          return NodeDisplacementsC4p2.ZzInRadian();

        case NodeDisplacementComponent.Xxyyzz:
          return NodeDisplacementsC4p2.XxyyzzInRadian();
      }
      throw new NotImplementedException();
    }

    private double ResultsHelper(INodeResultSubset<IDisplacement, NodeExtremaVector6> result, NodeDisplacementComponent component, bool max) {
      double d = 0;
      NodeExtremaVector6 extrema = max ? result.Max : result.Min;
      switch (component) {
        case NodeDisplacementComponent.X:
          d = result.GetExtrema(extrema.X).X.Millimeters;
          break;

        case NodeDisplacementComponent.Y:
          d = result.GetExtrema(extrema.Y).Y.Millimeters;
          break;

        case NodeDisplacementComponent.Z:
          d = result.GetExtrema(extrema.Z).Z.Millimeters;
          break;

        case NodeDisplacementComponent.Xyz:
          d = result.GetExtrema(extrema.Xyz).Xyz.Millimeters;
          break;

        case NodeDisplacementComponent.Xx:
          d = result.GetExtrema(extrema.Xx).Xx.Radians;
          break;

        case NodeDisplacementComponent.Yy:
          d = result.GetExtrema(extrema.Yy).Yy.Radians;
          break;

        case NodeDisplacementComponent.Zz:
          d = result.GetExtrema(extrema.Zz).Zz.Radians;
          break;

        case NodeDisplacementComponent.Xxyyzz:
          d = result.GetExtrema(extrema.Xxyyzz).Xxyyzz.Radians;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    private double ResultsHelper(IDisplacement result, NodeDisplacementComponent component) {
      double d = 0;
      switch (component) {
        case NodeDisplacementComponent.X:
          d = result.X.Millimeters;
          break;

        case NodeDisplacementComponent.Y:
          d = result.Y.Millimeters;
          break;

        case NodeDisplacementComponent.Z:
          d = result.Z.Millimeters;
          break;

        case NodeDisplacementComponent.Xyz:
          d = result.Xyz.Millimeters;
          break;

        case NodeDisplacementComponent.Xx:
          d = result.Xx.Radians;
          break;

        case NodeDisplacementComponent.Yy:
          d = result.Yy.Radians;
          break;

        case NodeDisplacementComponent.Zz:
          d = result.Zz.Radians;
          break;

        case NodeDisplacementComponent.Xxyyzz:
          d = result.Xxyyzz.Radians;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    private static readonly string NodeList = "442 to 468";
  }
}
