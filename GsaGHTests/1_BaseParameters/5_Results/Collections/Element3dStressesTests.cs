using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Helpers.Import;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Element3dStressesTests {
    private static readonly string ElementList = "6444 6555 7000 7015";

    [Fact]
    public void Element3dStressesElement3dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.AnalysisCaseResult(GsaFile.Element3dSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element3dStresses.ResultSubset(elementIds);

      // Assert element IDs
      var expectedIds = result.Model.Model.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element3dStressesElement3dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.CombinationCaseResult(GsaFile.Element3dSimple, 2);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element3dStresses.ResultSubset(elementIds);

      // Assert element IDs
      var expectedIds = result.Model.Model.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(ResultTensor3HelperEnum.Xx)]
    [InlineData(ResultTensor3HelperEnum.Yy)]
    [InlineData(ResultTensor3HelperEnum.Zz)]
    [InlineData(ResultTensor3HelperEnum.Xy)]
    [InlineData(ResultTensor3HelperEnum.Yz)]
    [InlineData(ResultTensor3HelperEnum.Zx)]
    public void Element3dStressesMaxFromAnalysisCaseTest(ResultTensor3HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.AnalysisCaseResult(GsaFile.Element3dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element3dStresses.ResultSubset(elementIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(ResultTensor3HelperEnum.Xx)]
    [InlineData(ResultTensor3HelperEnum.Yy)]
    [InlineData(ResultTensor3HelperEnum.Zz)]
    [InlineData(ResultTensor3HelperEnum.Xy)]
    [InlineData(ResultTensor3HelperEnum.Yz)]
    [InlineData(ResultTensor3HelperEnum.Zx)]
    public void Element3dStressesMaxFromCombinationCaseTest(ResultTensor3HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.CombinationCaseResult(GsaFile.Element3dSimple, 2);
      double expected = Math.Max(
        ExpectedCombinationCaseC2p1Values(component).Max(),
        ExpectedCombinationCaseC2p2Values(component).Max());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element3dStresses.ResultSubset(elementIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(ResultTensor3HelperEnum.Xx)]
    [InlineData(ResultTensor3HelperEnum.Yy)]
    [InlineData(ResultTensor3HelperEnum.Zz)]
    [InlineData(ResultTensor3HelperEnum.Xy)]
    [InlineData(ResultTensor3HelperEnum.Yz)]
    [InlineData(ResultTensor3HelperEnum.Zx)]
    public void Element3dStressesMinFromAnalysisCaseTest(ResultTensor3HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.AnalysisCaseResult(GsaFile.Element3dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element3dStresses.ResultSubset(elementIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultTensor3HelperEnum.Xx)]
    [InlineData(ResultTensor3HelperEnum.Yy)]
    [InlineData(ResultTensor3HelperEnum.Zz)]
    [InlineData(ResultTensor3HelperEnum.Xy)]
    [InlineData(ResultTensor3HelperEnum.Yz)]
    [InlineData(ResultTensor3HelperEnum.Zx)]
    public void Element3dStressesMinFromcombinationCaseTest(ResultTensor3HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.CombinationCaseResult(GsaFile.Element3dSimple, 2);
      double expected = Math.Min(
        ExpectedCombinationCaseC2p1Values(component).Min(),
        ExpectedCombinationCaseC2p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element3dStresses.ResultSubset(elementIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultTensor3HelperEnum.Xx)]
    [InlineData(ResultTensor3HelperEnum.Yy)]
    [InlineData(ResultTensor3HelperEnum.Zz)]
    [InlineData(ResultTensor3HelperEnum.Xy)]
    [InlineData(ResultTensor3HelperEnum.Yz)]
    [InlineData(ResultTensor3HelperEnum.Zx)]
    public void Element3dStressesValuesFromAnalysisCaseTest(ResultTensor3HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.AnalysisCaseResult(GsaFile.Element3dSimple, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element3dStresses.ResultSubset(elementIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IMeshQuantity<IStress>> stressQuantity = resultSet.Subset[id];

        Assert.Single(stressQuantity);
        foreach (IStress stress in stressQuantity[0].Results()) {
          double x = TestsResultHelper.ResultsHelper(stress, component);
          Assert.Equal(expected[i++], x);
        }
      }
    }

    [Theory]
    [InlineData(ResultTensor3HelperEnum.Xx)]
    [InlineData(ResultTensor3HelperEnum.Yy)]
    [InlineData(ResultTensor3HelperEnum.Zz)]
    [InlineData(ResultTensor3HelperEnum.Xy)]
    [InlineData(ResultTensor3HelperEnum.Yz)]
    [InlineData(ResultTensor3HelperEnum.Zx)]
    public void Element3dStressesValuesFromCombinationCaseTest(ResultTensor3HelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResult2Tests.CombinationCaseResult(GsaFile.Element3dSimple, 2);
      List<double> expectedP1 = ExpectedCombinationCaseC2p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC2p2Values(component);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element3dStresses.ResultSubset(elementIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IMeshQuantity<IStress>> stressQuantity = resultSet.Subset[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, stressQuantity.Count);

        foreach (IStress stress in stressQuantity[0].Results()) {
          double perm1 = TestsResultHelper.ResultsHelper(stress, component);
          Assert.Equal(expectedP1[i++], perm1);
        }
      }

      i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IMeshQuantity<IStress>> stressQuantity = resultSet.Subset[id];

        foreach (IStress stress in stressQuantity[1].Results()) {
          double perm2 = TestsResultHelper.ResultsHelper(stress, component);
          Assert.Equal(expectedP2[i++], perm2);
        }
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultTensor3HelperEnum component) {
      switch (component) {
        case ResultTensor3HelperEnum.Xx: return Element3dStressesA1.XxInMPa();
        case ResultTensor3HelperEnum.Yy: return Element3dStressesA1.YyInMPa();
        case ResultTensor3HelperEnum.Zz: return Element3dStressesA1.ZzInMPa();
        case ResultTensor3HelperEnum.Xy: return Element3dStressesA1.XyInMPa();
        case ResultTensor3HelperEnum.Yz: return Element3dStressesA1.YzInMPa();
        case ResultTensor3HelperEnum.Zx: return Element3dStressesA1.ZxInMPa();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p1Values(
      ResultTensor3HelperEnum component) {
      switch (component) {
        case ResultTensor3HelperEnum.Xx: return Element3dStressesC2p1.XxInMPa();
        case ResultTensor3HelperEnum.Yy: return Element3dStressesC2p1.YyInMPa();
        case ResultTensor3HelperEnum.Zz: return Element3dStressesC2p1.ZzInMPa();
        case ResultTensor3HelperEnum.Xy: return Element3dStressesC2p1.XyInMPa();
        case ResultTensor3HelperEnum.Yz: return Element3dStressesC2p1.YzInMPa();
        case ResultTensor3HelperEnum.Zx: return Element3dStressesC2p1.ZxInMPa();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p2Values(
      ResultTensor3HelperEnum component) {
      switch (component) {
        case ResultTensor3HelperEnum.Xx: return Element3dStressesC2p2.XxInMPa();
        case ResultTensor3HelperEnum.Yy: return Element3dStressesC2p2.YyInMPa();
        case ResultTensor3HelperEnum.Zz: return Element3dStressesC2p2.ZzInMPa();
        case ResultTensor3HelperEnum.Xy: return Element3dStressesC2p2.XyInMPa();
        case ResultTensor3HelperEnum.Yz: return Element3dStressesC2p2.YzInMPa();
        case ResultTensor3HelperEnum.Zx: return Element3dStressesC2p2.ZxInMPa();
      }

      throw new NotImplementedException();
    }
  }
}
