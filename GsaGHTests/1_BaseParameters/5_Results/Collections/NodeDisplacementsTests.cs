using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters.Results;
using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class NodeDisplacementsTests {
    private static readonly string NodeList = "442 to 468";

    [Fact]
    public void NodeDisplacementsNodeIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

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
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

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
    public void NodeDisplacementsMaxFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
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
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void NodeDisplacementsMaxFromCombinationCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
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
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void NodeDisplacementsMinFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
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
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void NodeDisplacementsMinFromcombinationCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
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
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    [InlineData(ResultVector6HelperEnum.Xx)]
    [InlineData(ResultVector6HelperEnum.Yy)]
    [InlineData(ResultVector6HelperEnum.Zz)]
    [InlineData(ResultVector6HelperEnum.Xxyyzz)]
    public void NodeDisplacementsValuesFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      INodeResultSubset<IDisplacement, ResultVector6<NodeExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IDisplacement> displacementQuantity = resultSet.Subset[id];

        // for analysis case results we expect only one value in the collection
        Assert.Single(displacementQuantity);

        double x = TestsResultHelper.ResultsHelper(displacementQuantity[0], component);
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
    public void NodeDisplacementsValuesFromCombinationCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
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

    private List<double> ExpectedAnalysisCaseValues(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return NodeDisplacementsA1.XInMillimeter();

        case ResultVector6HelperEnum.Y: return NodeDisplacementsA1.YInMillimeter();

        case ResultVector6HelperEnum.Z: return NodeDisplacementsA1.ZInMillimeter();

        case ResultVector6HelperEnum.Xyz: return NodeDisplacementsA1.XyzInMillimeter();

        case ResultVector6HelperEnum.Xx: return NodeDisplacementsA1.XxInRadian();

        case ResultVector6HelperEnum.Yy: return NodeDisplacementsA1.YyInRadian();

        case ResultVector6HelperEnum.Zz: return NodeDisplacementsA1.ZzInRadian();

        case ResultVector6HelperEnum.Xxyyzz: return NodeDisplacementsA1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p1Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return NodeDisplacementsC4p1.XInMillimeter();

        case ResultVector6HelperEnum.Y: return NodeDisplacementsC4p1.YInMillimeter();

        case ResultVector6HelperEnum.Z: return NodeDisplacementsC4p1.ZInMillimeter();

        case ResultVector6HelperEnum.Xyz: return NodeDisplacementsC4p1.XyzInMillimeter();

        case ResultVector6HelperEnum.Xx: return NodeDisplacementsC4p1.XxInRadian();

        case ResultVector6HelperEnum.Yy: return NodeDisplacementsC4p1.YyInRadian();

        case ResultVector6HelperEnum.Zz: return NodeDisplacementsC4p1.ZzInRadian();

        case ResultVector6HelperEnum.Xxyyzz: return NodeDisplacementsC4p1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p2Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return NodeDisplacementsC4p2.XInMillimeter();

        case ResultVector6HelperEnum.Y: return NodeDisplacementsC4p2.YInMillimeter();

        case ResultVector6HelperEnum.Z: return NodeDisplacementsC4p2.ZInMillimeter();

        case ResultVector6HelperEnum.Xyz: return NodeDisplacementsC4p2.XyzInMillimeter();

        case ResultVector6HelperEnum.Xx: return NodeDisplacementsC4p2.XxInRadian();

        case ResultVector6HelperEnum.Yy: return NodeDisplacementsC4p2.YyInRadian();

        case ResultVector6HelperEnum.Zz: return NodeDisplacementsC4p2.ZzInRadian();

        case ResultVector6HelperEnum.Xxyyzz: return NodeDisplacementsC4p2.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }


  }
}
