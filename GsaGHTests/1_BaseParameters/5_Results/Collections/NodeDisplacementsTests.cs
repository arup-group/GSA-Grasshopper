using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public partial class NodeDisplacementsTests {

    private static readonly string NodeList = "442 to 468";

    [Fact]
    public void NodeDisplacementsNodeIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

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
      INodeResultSubset<IDisplacement, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

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
    public void NodeDisplacementsMaxFromAnalysisCaseTest(NodeComponentHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

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
    public void NodeDisplacementsMaxFromCombinationCaseTest(NodeComponentHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(ExpectedCombinationCaseC4p1Values(component).Max(),
        ExpectedCombinationCaseC4p2Values(component).Max());

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

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
    public void NodeDisplacementsMinFromAnalysisCaseTest(NodeComponentHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

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
    public void NodeDisplacementsMinFromcombinationCaseTest(NodeComponentHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(ExpectedCombinationCaseC4p1Values(component).Min(),
        ExpectedCombinationCaseC4p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

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
    public void NodeDisplacementsValuesFromAnalysisCaseTest(NodeComponentHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IDisplacement> displacementQuantity = resultSet.Subset[id];

        // for analysis case results we expect only one value in the collection
        Assert.Single(displacementQuantity);

        double x = TestsResultHelper.ResultsHelper(displacementQuantity[0], component);
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
    public void NodeDisplacementsValuesFromCombinationCaseTest(NodeComponentHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = ExpectedCombinationCaseC4p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC4p2Values(component);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        var displacementQuantity = (Collection<IDisplacement>)resultSet.Subset[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, displacementQuantity.Count);

        double perm1 = TestsResultHelper.ResultsHelper(displacementQuantity[0], component);
        Assert.Equal(expectedP1[i], perm1);
        double perm2 = TestsResultHelper.ResultsHelper(displacementQuantity[1], component);
        Assert.Equal(expectedP2[i++], perm2);
      }
    }

    private List<double> ExpectedAnalysisCaseValues(NodeComponentHelperEnum component) {
      switch (component) {
        case NodeComponentHelperEnum.X: return NodeDisplacementsA1.XInMillimeter();

        case NodeComponentHelperEnum.Y: return NodeDisplacementsA1.YInMillimeter();

        case NodeComponentHelperEnum.Z: return NodeDisplacementsA1.ZInMillimeter();

        case NodeComponentHelperEnum.Xyz: return NodeDisplacementsA1.XyzInMillimeter();

        case NodeComponentHelperEnum.Xx: return NodeDisplacementsA1.XxInRadian();

        case NodeComponentHelperEnum.Yy: return NodeDisplacementsA1.YyInRadian();

        case NodeComponentHelperEnum.Zz: return NodeDisplacementsA1.ZzInRadian();

        case NodeComponentHelperEnum.Xxyyzz: return NodeDisplacementsA1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p1Values(NodeComponentHelperEnum component) {
      switch (component) {
        case NodeComponentHelperEnum.X: return NodeDisplacementsC4p1.XInMillimeter();

        case NodeComponentHelperEnum.Y: return NodeDisplacementsC4p1.YInMillimeter();

        case NodeComponentHelperEnum.Z: return NodeDisplacementsC4p1.ZInMillimeter();

        case NodeComponentHelperEnum.Xyz: return NodeDisplacementsC4p1.XyzInMillimeter();

        case NodeComponentHelperEnum.Xx: return NodeDisplacementsC4p1.XxInRadian();

        case NodeComponentHelperEnum.Yy: return NodeDisplacementsC4p1.YyInRadian();

        case NodeComponentHelperEnum.Zz: return NodeDisplacementsC4p1.ZzInRadian();

        case NodeComponentHelperEnum.Xxyyzz: return NodeDisplacementsC4p1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p2Values(NodeComponentHelperEnum component) {
      switch (component) {
        case NodeComponentHelperEnum.X: return NodeDisplacementsC4p2.XInMillimeter();

        case NodeComponentHelperEnum.Y: return NodeDisplacementsC4p2.YInMillimeter();

        case NodeComponentHelperEnum.Z: return NodeDisplacementsC4p2.ZInMillimeter();

        case NodeComponentHelperEnum.Xyz: return NodeDisplacementsC4p2.XyzInMillimeter();

        case NodeComponentHelperEnum.Xx: return NodeDisplacementsC4p2.XxInRadian();

        case NodeComponentHelperEnum.Yy: return NodeDisplacementsC4p2.YyInRadian();

        case NodeComponentHelperEnum.Zz: return NodeDisplacementsC4p2.ZzInRadian();

        case NodeComponentHelperEnum.Xxyyzz: return NodeDisplacementsC4p2.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }
  }
}
