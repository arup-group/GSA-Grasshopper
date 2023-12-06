using GH_IO.Serialization;
using Grasshopper.Kernel.Data;
using GsaGH.Components;
using GsaGH.Helpers.GsaApi;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters.Results;
using OasysGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class Member1dDisplacementsTests {
    private static readonly string MemberList = "all";

    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new Member1dDisplacements();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void Member1dDisplacementsMember1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      var comp = new Member1dDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, MemberList, 1);

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Equal(memberIds.Count, paths.Count);

        var cases = paths.Select(x => x.Indices[0]).ToList();
        foreach (int caseid in cases) {
          Assert.Equal(1, caseid);
        }

        var permutations = paths.Select(x => x.Indices[1]).ToList();
        foreach (int permutation in permutations) {
          Assert.Equal(0, permutation);
        }

        var ids = paths.Select(x => x.Indices[2]).ToList();
        for (int j = 0; j < ids.Count; j++) {
          // Assert element IDs
          var expectedIds = result.Model.Model.Elements(MemberList).Keys.OrderBy(x => x).ToList();
          Assert.Equal(expectedIds[j], ids[j]);
        }
      }
    }

    [Fact]
    public void Member1dDisplacementsMember1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);
      result.SelectedPermutationIds = new List<int>() { 1 };

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      var comp = new Member1dDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, MemberList, 1);

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Single(paths); 

        var cases = paths.Select(x => x.Indices[0]).ToList();
        foreach (int caseid in cases) {
          Assert.Equal(1, caseid);
        }

        var expectedPermutations = new List<int>();
        for (int j = 0; j < memberIds.Count; j++) {
          expectedPermutations.AddRange(Enumerable.Repeat(j + 1, memberIds.Count));
        }
        var permutations = paths.Select(x => x.Indices[1]).ToList();
        for (int j = 0; j < permutations.Count; j++) {
          Assert.Equal(expectedPermutations[j], permutations[j]);
        }

        var expectedIds = memberIds.ToList();
        expectedIds.AddRange(memberIds.ToList()); // add elementlist for each permutation

        var ids = paths.Select(x => x.Indices[2]).ToList();
        for (int j = 0; j < ids.Count; j++) {
          Assert.Equal(expectedIds[j], ids[j]);
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
    public void Member1dDisplacementsMaxFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      var comp = new Member1dDisplacements();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(Unit(component));
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
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
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = ExpectedCombinationCaseC1Values(component).Max();

      // Act
      var comp = new Member1dDisplacements();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(Unit(component));
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(max, 4));
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
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      var comp = new Member1dDisplacements();
      comp.SetSelected(0, 9 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(Unit(component));
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
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
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);
      double expected = ExpectedCombinationCaseC1Values(component).Min();

      // Act
      var comp = new Member1dDisplacements();
      comp.SetSelected(0, 9 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(Unit(component));
      Assert.Equal(expected, ResultHelper.RoundToSignificantDigits(min, 4));
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
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.SteelDesignSimple, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IEntity1dDisplacement, IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dDisplacements.ResultSubset(memberIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dDisplacement> displacementQuantity = resultSet.Subset[id];

        // for analysis case results we expect 4 positions
        Assert.Single(displacementQuantity);
        var positions = Enumerable.Range(0, positionsCount)
         .Select(k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double x = TestsResultHelper.ResultsHelper(displacementQuantity[0].Results[position],
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
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignSimple, 1);
      List<double> expectedP1 = ExpectedCombinationCaseC1Values(component);
      int positionsCount = 5;

      // Act
      ReadOnlyCollection<int> memberIds = result.MemberIds(MemberList);
      IEntity1dResultSubset<IEntity1dDisplacement, IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet
        = result.Member1dDisplacements.ResultSubset(memberIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IEntity1dDisplacement> displacementQuantity = resultSet.Subset[id];

        // for C1 case results we expect 1 permutation in the collection
        Assert.Single(displacementQuantity);

        var positions = Enumerable.Range(0, positionsCount)
         .Select(k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double perm1
            = TestsResultHelper.ResultsHelper(displacementQuantity[0].Results[position], component);
          Assert.Equal(expectedP1[i++], perm1);
        }
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Member1dDisplacementsA1.XInMillimeter();

        case ResultVector6HelperEnum.Y: return Member1dDisplacementsA1.YInMillimeter();

        case ResultVector6HelperEnum.Z: return Member1dDisplacementsA1.ZInMillimeter();

        case ResultVector6HelperEnum.Xyz: return Member1dDisplacementsA1.XyzInMillimeter();

        case ResultVector6HelperEnum.Xx: return Member1dDisplacementsA1.XxInRadian();

        case ResultVector6HelperEnum.Yy: return Member1dDisplacementsA1.YyInRadian();

        case ResultVector6HelperEnum.Zz: return Member1dDisplacementsA1.ZzInRadian();

        case ResultVector6HelperEnum.Xxyyzz: return Member1dDisplacementsA1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC1Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Member1dDisplacementsC1.XInMillimeter();

        case ResultVector6HelperEnum.Y: return Member1dDisplacementsC1.YInMillimeter();

        case ResultVector6HelperEnum.Z: return Member1dDisplacementsC1.ZInMillimeter();

        case ResultVector6HelperEnum.Xyz: return Member1dDisplacementsC1.XyzInMillimeter();

        case ResultVector6HelperEnum.Xx: return Member1dDisplacementsC1.XxInRadian();

        case ResultVector6HelperEnum.Yy: return Member1dDisplacementsC1.YyInRadian();

        case ResultVector6HelperEnum.Zz: return Member1dDisplacementsC1.ZzInRadian();

        case ResultVector6HelperEnum.Xxyyzz: return Member1dDisplacementsC1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private Enum Unit(ResultVector6HelperEnum component) {
      Enum unit = LengthUnit.Millimeter;
      if ((int)component > 3) {
        unit = AngleUnit.Radian;
      }
      return unit;
    }
  }
}
