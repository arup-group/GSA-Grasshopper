using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Helpers;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using OasysUnits;

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
      IEntity0dResultSubset<IDisplacement, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert node IDs
      var expectedIds = result.Model.ApiModel.Nodes(NodeList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void NodeDisplacementsNodeIdsFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IDisplacement, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

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
    public void NodeDisplacementsMaxFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IDisplacement, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
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
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IDisplacement, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
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
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IDisplacement, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
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
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IDisplacement, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
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
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IDisplacement, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IDisplacement> displacementQuantity = resultSet.Subset[id];

        // for analysis case results we expect only one value in the collection
        Assert.Single(displacementQuantity);

        double x = TestsResultHelper.ResultsHelper(displacementQuantity[0], component);
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
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      IEntity0dResultSubset<IDisplacement, ResultVector6<Entity0dExtremaKey>> resultSet
        = result.NodeDisplacements.ResultSubset(nodeIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        var displacementQuantity = (Collection<IDisplacement>)resultSet.Subset[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, displacementQuantity.Count);

        double perm1 = TestsResultHelper.ResultsHelper(displacementQuantity[0], component);
        Assert.Equal(expectedP1[i], perm1, DoubleComparer.Default);
        double perm2 = TestsResultHelper.ResultsHelper(displacementQuantity[1], component);
        Assert.Equal(expectedP2[i++], perm2, DoubleComparer.Default);
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


  }
}
