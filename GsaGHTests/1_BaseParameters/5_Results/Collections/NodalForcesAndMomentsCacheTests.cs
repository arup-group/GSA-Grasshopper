using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Components;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class NodalForcesAndMomentsCacheTests {

    private static readonly string NodeList = "442";

    [Fact]
    public void NodalForcesAndMomentsNodeIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      GsaGH.Parameters.Results.NodalForcesAndMoments resultSet = result.NodalForcesAndMoments.ResultSubset(nodeIds);

      // Assert
      var expectedIds = result.Model.ApiModel.Nodes(NodeList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void NodalForcesAndMomentsNodeIdsFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 2);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      GsaGH.Parameters.Results.NodalForcesAndMoments resultSet = result.NodalForcesAndMoments.ResultSubset(nodeIds);

      // Assert
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
    public void NodalForcesAndMomentsValuesFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      GsaGH.Parameters.Results.NodalForcesAndMoments resultSet = result.NodalForcesAndMoments.ResultSubset(nodeIds);

      // Assert
      IDictionary<int, IReactionForce> forces = resultSet.Subset[442][0];
      Assert.Equal(4, forces.Count);

      List<double> perm = TestsResultHelper.ResultsHelper(forces, component);
      for (int i = 0; i < forces.Count; i++) {
        Assert.Equal(expected[i], perm[i], 1);
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
    public void NodalForcesAndMomentsValuesFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = ExpectedCombinationCaseValues(component);

      // Act
      ReadOnlyCollection<int> nodeIds = result.NodeIds(NodeList);
      GsaGH.Parameters.Results.NodalForcesAndMoments resultSet
        = result.NodalForcesAndMoments.ResultSubset(nodeIds);

      // Assert
      IDictionary<int, IReactionForce> forces = resultSet.Subset[442][0];
      Assert.Equal(4, forces.Count);

      List<double> perm = TestsResultHelper.ResultsHelper(forces, component);
      for (int i = 0; i < forces.Count; i++) {
        Assert.Equal(expected[i], perm[i], 1);
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return NodalForcesAndMomentsA1.XInKiloNewtons();

        case ResultVector6.Y: return NodalForcesAndMomentsA1.YInKiloNewtons();

        case ResultVector6.Z: return NodalForcesAndMomentsA1.ZInKiloNewtons();

        case ResultVector6.Xyz: return NodalForcesAndMomentsA1.XyzInKiloNewtons();

        case ResultVector6.Xx: return NodalForcesAndMomentsA1.XxInKiloNewtonsPerMeter();

        case ResultVector6.Yy: return NodalForcesAndMomentsA1.YyInKiloNewtonsPerMeter();

        case ResultVector6.Zz: return NodalForcesAndMomentsA1.ZzInKiloNewtonsPerMeter();

        case ResultVector6.Xxyyzz: return NodalForcesAndMomentsA1.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return NodalForcesAndMomentsC1.XInKiloNewtons();

        case ResultVector6.Y: return NodalForcesAndMomentsC1.YInKiloNewtons();

        case ResultVector6.Z: return NodalForcesAndMomentsC1.ZInKiloNewtons();

        case ResultVector6.Xyz: return NodalForcesAndMomentsC1.YzInKiloNewtons();

        case ResultVector6.Xx: return NodalForcesAndMomentsC1.XxInKiloNewtonsPerMeter();

        case ResultVector6.Yy: return NodalForcesAndMomentsC1.YyInKiloNewtonsPerMeter();

        case ResultVector6.Zz: return NodalForcesAndMomentsC1.ZzInKiloNewtonsPerMeter();

        case ResultVector6.Xxyyzz: return NodalForcesAndMomentsC1.XxyyzzInKiloNewtonsPerMeter();
      }

      throw new NotImplementedException();
    }
  }
}
