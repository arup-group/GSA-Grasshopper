using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Member1dForcesTests {

    private static readonly string MemberList = "all";

    [Fact]
    public void Member1dForcesMember1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, 5);

      // Assert member IDs
      var expectedIds = result.Model.ApiModel.Members(MemberList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Member1dForesMember1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, 5);

      // Assert member IDs
      var expectedIds = result.Model.ApiModel.Members(MemberList).Keys.OrderBy(x => x).ToList();
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
    public void Member1dForcesMaxFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, 5);

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
    public void Member1dDisplacementsMaxFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = ExpectedCombinationCaseC1Values(component).Max();

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, 5);

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
    public void Member1dDisplacementsMinFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, 5);

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
    public void Member1dDisplacementsMinFromcombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = ExpectedCombinationCaseC1Values(component).Min();

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, 5);

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
    public void Member1dDisplacementsValuesFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dQuantity<IInternalForce>> forcesQuantity = resultSet.Subset[id];

        // for analysis case results we expect 4 positions
        Assert.Single(forcesQuantity);
        var positions = Enumerable.Range(0, positionsCount)
         .Select(k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double x = TestsResultHelper.ResultsHelper(forcesQuantity[0].Results[position],
            component);
          Assert.Equal(expected[i++], x, DoubleComparer.Default);
        }
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
    public void Member1dDisplacementsValuesFromCombinationCaseTest(
      ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);
      List<double> expectedP1 = ExpectedCombinationCaseC1Values(component);
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dQuantity<IInternalForce>> forcesQuantity = resultSet.Subset[id];

        // for C1 case results we expect 1 permutation in the collection
        Assert.Single(forcesQuantity);

        var positions = Enumerable.Range(0, positionsCount)
         .Select(k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double perm1
            = TestsResultHelper.ResultsHelper(forcesQuantity[0].Results[position], component);
          Assert.Equal(expectedP1[i++], perm1, DoubleComparer.Default);
        }
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return Member1dForcesA1.XInKiloNewton();
        case ResultVector6.Y: return Member1dForcesA1.YInKiloNewton();
        case ResultVector6.Z: return Member1dForcesA1.ZInKiloNewton();
        case ResultVector6.Xyz: return Member1dForcesA1.YzInKiloNewton();
        case ResultVector6.Xx: return Member1dForcesA1.XxInKiloNewtonMeter();
        case ResultVector6.Yy: return Member1dForcesA1.YyInKiloNewtonMeter();
        case ResultVector6.Zz: return Member1dForcesA1.ZzInKiloNewtonMeter();
        case ResultVector6.Xxyyzz: return Member1dForcesA1.YyzzInKiloNewtonMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC1Values(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return Member1dForcesC1.XInKiloNewton();
        case ResultVector6.Y: return Member1dForcesC1.YInKiloNewton();
        case ResultVector6.Z: return Member1dForcesC1.ZInKiloNewton();
        case ResultVector6.Xyz: return Member1dForcesC1.YzInKiloNewton();
        case ResultVector6.Xx: return Member1dForcesC1.XxInKiloNewtonMeter();
        case ResultVector6.Yy: return Member1dForcesC1.YyInKiloNewtonMeter();
        case ResultVector6.Zz: return Member1dForcesC1.ZzInKiloNewtonMeter();
        case ResultVector6.Xxyyzz: return Member1dForcesC1.YyzzInKiloNewtonMeter();
      }

      throw new NotImplementedException();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-2)]
    [InlineData(-11)]
    [InlineData(-12)]
    [InlineData(-13)]
    [InlineData(-14)]
    public void SetAxisThrowsExceptionTest(int axisId) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      result.Member1dInternalForces.SetStandardAxis(axisId);
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);

      // Assert
      Assert.Throws<GsaApiException>(() => result.Member1dInternalForces.ResultSubset(memberIds, 5));
    }
  }
}
