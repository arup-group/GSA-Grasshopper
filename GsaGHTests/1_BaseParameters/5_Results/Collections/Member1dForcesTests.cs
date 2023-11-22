using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Member1dForcesTests {

    private static readonly string MemberList = "all";

    [Fact]
    public void Member1dForcesMember1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, 5);

      // Assert member IDs
      var expectedIds = result.Model.Model.Members(MemberList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Member1dForesMember1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, 5);

      // Assert member IDs
      var expectedIds = result.Model.Model.Members(MemberList).Keys.OrderBy(x => x).ToList();
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
    public void Member1dForcesMaxFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, 5);

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
    public void Member1dDisplacementsMaxFromCombinationCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = ExpectedCombinationCaseC1Values(component).Max();

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, 5);

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
    public void Member1dDisplacementsMinFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, 5);

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
    public void Member1dDisplacementsMinFromcombinationCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = ExpectedCombinationCaseC1Values(component).Min();

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, 5);

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
    public void Member1dDisplacementsValuesFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IEntity1dInternalForce> forcesQuantity = resultSet.Subset[id];

        // for analysis case results we expect 4 positions
        Assert.Single(forcesQuantity);
        var positions = Enumerable.Range(0, positionsCount)
         .Select(k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double x = TestsResultHelper.ResultsHelper(forcesQuantity[0].Results[position],
            component);
          Assert.Equal(expected[i++], x);
        }
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
    public void Member1dDisplacementsValuesFromCombinationCaseTest(
      ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);
      List<double> expectedP1 = ExpectedCombinationCaseC1Values(component);
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IEntity1dInternalForce, IInternalForce, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dInternalForces.ResultSubset(memberIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IEntity1dInternalForce> forcesQuantity = resultSet.Subset[id];

        // for C1 case results we expect 1 permutation in the collection
        Assert.Single(forcesQuantity);

        var positions = Enumerable.Range(0, positionsCount)
         .Select(k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double perm1
            = TestsResultHelper.ResultsHelper(forcesQuantity[0].Results[position], component);
          Assert.Equal(expectedP1[i++], perm1);
        }
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Member1dForcesA1.XInKiloNewton();

        case ResultVector6HelperEnum.Y: return Member1dForcesA1.YInKiloNewton();

        case ResultVector6HelperEnum.Z: return Member1dForcesA1.ZInKiloNewton();

        case ResultVector6HelperEnum.Xyz: return Member1dForcesA1.XyzInKiloNewton();

        case ResultVector6HelperEnum.Xx: return Member1dForcesA1.XxInKiloNewtonMeter();

        case ResultVector6HelperEnum.Yy: return Member1dForcesA1.YyInKiloNewtonMeter();

        case ResultVector6HelperEnum.Zz: return Member1dForcesA1.ZzInKiloNewtonMeter();

        case ResultVector6HelperEnum.Xxyyzz: return Member1dForcesA1.XxyyzzInKiloNewtonMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC1Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Member1dForcesC1.XInKiloNewton();

        case ResultVector6HelperEnum.Y: return Member1dForcesC1.YInKiloNewton();

        case ResultVector6HelperEnum.Z: return Member1dForcesC1.ZInKiloNewton();

        case ResultVector6HelperEnum.Xyz: return Member1dForcesC1.XyzInKiloNewton();

        case ResultVector6HelperEnum.Xx: return Member1dForcesC1.XxInKiloNewtonMeter();

        case ResultVector6HelperEnum.Yy: return Member1dForcesC1.YyInKiloNewtonMeter();

        case ResultVector6HelperEnum.Zz: return Member1dForcesC1.ZzInKiloNewtonMeter();

        case ResultVector6HelperEnum.Xxyyzz: return Member1dForcesC1.XxyyzzInKiloNewtonMeter();
      }

      throw new NotImplementedException();
    }
  }
}