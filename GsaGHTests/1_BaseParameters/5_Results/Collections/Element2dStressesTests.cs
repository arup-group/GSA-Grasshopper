using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Element2dStressesTests {
    private static readonly string ElementList = "420 430 440 445";

    [Theory]
    [InlineData(Layer2d.Top)]
    [InlineData(Layer2d.Middle)]
    [InlineData(Layer2d.Bottom)]
    public void Element2dStressesElement2dIdsFromAnalysisCaseTest(Layer2d layer) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element2dStresses.ResultSubset(elementIds, layer);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(Layer2d.Top)]
    [InlineData(Layer2d.Middle)]
    [InlineData(Layer2d.Bottom)]
    public void Element2dStressesElement2dIdsFromCombinationCaseTest(Layer2d layer) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element2dStresses.ResultSubset(elementIds, layer);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(ResultTensor3.Xx, Layer2d.Top)]
    [InlineData(ResultTensor3.Yy, Layer2d.Top)]
    [InlineData(ResultTensor3.Zz, Layer2d.Top)]
    [InlineData(ResultTensor3.Xy, Layer2d.Top)]
    [InlineData(ResultTensor3.Yz, Layer2d.Top)]
    [InlineData(ResultTensor3.Zx, Layer2d.Top)]
    [InlineData(ResultTensor3.Xx, Layer2d.Middle)]
    [InlineData(ResultTensor3.Yy, Layer2d.Middle)]
    [InlineData(ResultTensor3.Zz, Layer2d.Middle)]
    [InlineData(ResultTensor3.Xy, Layer2d.Middle)]
    [InlineData(ResultTensor3.Yz, Layer2d.Middle)]
    [InlineData(ResultTensor3.Zx, Layer2d.Middle)]
    [InlineData(ResultTensor3.Xx, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Yy, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Zz, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Xy, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Yz, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Zx, Layer2d.Bottom)]
    public void Element2dStressesMaxFromAnalysisCaseTest(ResultTensor3 component, Layer2d layer) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component, layer).Max();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element2dStresses.ResultSubset(elementIds, layer);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(ResultTensor3.Xx, Layer2d.Top)]
    [InlineData(ResultTensor3.Yy, Layer2d.Top)]
    [InlineData(ResultTensor3.Zz, Layer2d.Top)]
    [InlineData(ResultTensor3.Xy, Layer2d.Top)]
    [InlineData(ResultTensor3.Yz, Layer2d.Top)]
    [InlineData(ResultTensor3.Zx, Layer2d.Top)]
    [InlineData(ResultTensor3.Xx, Layer2d.Middle)]
    [InlineData(ResultTensor3.Yy, Layer2d.Middle)]
    [InlineData(ResultTensor3.Zz, Layer2d.Middle)]
    [InlineData(ResultTensor3.Xy, Layer2d.Middle)]
    [InlineData(ResultTensor3.Yz, Layer2d.Middle)]
    [InlineData(ResultTensor3.Zx, Layer2d.Middle)]
    [InlineData(ResultTensor3.Xx, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Yy, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Zz, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Xy, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Yz, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Zx, Layer2d.Bottom)]
    public void Element2dStressesMaxFromCombinationCaseTest(
      ResultTensor3 component, Layer2d layer) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Max(
        ExpectedCombinationCaseC2p1Values(component, layer).Max(),
        ExpectedCombinationCaseC2p2Values(component, layer).Max());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element2dStresses.ResultSubset(elementIds, layer);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(ResultTensor3.Xx, Layer2d.Top)]
    [InlineData(ResultTensor3.Yy, Layer2d.Top)]
    [InlineData(ResultTensor3.Zz, Layer2d.Top)]
    [InlineData(ResultTensor3.Xy, Layer2d.Top)]
    [InlineData(ResultTensor3.Yz, Layer2d.Top)]
    [InlineData(ResultTensor3.Zx, Layer2d.Top)]
    [InlineData(ResultTensor3.Xx, Layer2d.Middle)]
    [InlineData(ResultTensor3.Yy, Layer2d.Middle)]
    [InlineData(ResultTensor3.Zz, Layer2d.Middle)]
    [InlineData(ResultTensor3.Xy, Layer2d.Middle)]
    [InlineData(ResultTensor3.Yz, Layer2d.Middle)]
    [InlineData(ResultTensor3.Zx, Layer2d.Middle)]
    [InlineData(ResultTensor3.Xx, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Yy, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Zz, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Xy, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Yz, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Zx, Layer2d.Bottom)]
    public void Element2dStressesMinFromAnalysisCaseTest(
      ResultTensor3 component, Layer2d layer) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component, layer).Min();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element2dStresses.ResultSubset(elementIds, layer);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultTensor3.Xx, Layer2d.Top)]
    [InlineData(ResultTensor3.Yy, Layer2d.Top)]
    [InlineData(ResultTensor3.Zz, Layer2d.Top)]
    [InlineData(ResultTensor3.Xy, Layer2d.Top)]
    [InlineData(ResultTensor3.Yz, Layer2d.Top)]
    [InlineData(ResultTensor3.Zx, Layer2d.Top)]
    [InlineData(ResultTensor3.Xx, Layer2d.Middle)]
    [InlineData(ResultTensor3.Yy, Layer2d.Middle)]
    [InlineData(ResultTensor3.Zz, Layer2d.Middle)]
    [InlineData(ResultTensor3.Xy, Layer2d.Middle)]
    [InlineData(ResultTensor3.Yz, Layer2d.Middle)]
    [InlineData(ResultTensor3.Zx, Layer2d.Middle)]
    [InlineData(ResultTensor3.Xx, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Yy, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Zz, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Xy, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Yz, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Zx, Layer2d.Bottom)]
    public void Element2dStressesMinFromcombinationCaseTest(
      ResultTensor3 component, Layer2d layer) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Min(
        ExpectedCombinationCaseC2p1Values(component, layer).Min(),
        ExpectedCombinationCaseC2p2Values(component, layer).Min());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element2dStresses.ResultSubset(elementIds, layer);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultTensor3.Xx, Layer2d.Top)]
    [InlineData(ResultTensor3.Yy, Layer2d.Top)]
    [InlineData(ResultTensor3.Zz, Layer2d.Top)]
    [InlineData(ResultTensor3.Xy, Layer2d.Top)]
    [InlineData(ResultTensor3.Yz, Layer2d.Top)]
    [InlineData(ResultTensor3.Zx, Layer2d.Top)]
    [InlineData(ResultTensor3.Xx, Layer2d.Middle)]
    [InlineData(ResultTensor3.Yy, Layer2d.Middle)]
    [InlineData(ResultTensor3.Zz, Layer2d.Middle)]
    [InlineData(ResultTensor3.Xy, Layer2d.Middle)]
    [InlineData(ResultTensor3.Yz, Layer2d.Middle)]
    [InlineData(ResultTensor3.Zx, Layer2d.Middle)]
    [InlineData(ResultTensor3.Xx, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Yy, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Zz, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Xy, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Yz, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Zx, Layer2d.Bottom)]
    public void Element2dStressesValuesFromAnalysisCaseTest(
      ResultTensor3 component, Layer2d layer) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component, layer);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element2dStresses.ResultSubset(elementIds, layer);

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
    [InlineData(ResultTensor3.Xx, Layer2d.Top)]
    [InlineData(ResultTensor3.Yy, Layer2d.Top)]
    [InlineData(ResultTensor3.Zz, Layer2d.Top)]
    [InlineData(ResultTensor3.Xy, Layer2d.Top)]
    [InlineData(ResultTensor3.Yz, Layer2d.Top)]
    [InlineData(ResultTensor3.Zx, Layer2d.Top)]
    [InlineData(ResultTensor3.Xx, Layer2d.Middle)]
    [InlineData(ResultTensor3.Yy, Layer2d.Middle)]
    [InlineData(ResultTensor3.Zz, Layer2d.Middle)]
    [InlineData(ResultTensor3.Xy, Layer2d.Middle)]
    [InlineData(ResultTensor3.Yz, Layer2d.Middle)]
    [InlineData(ResultTensor3.Zx, Layer2d.Middle)]
    [InlineData(ResultTensor3.Xx, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Yy, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Zz, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Xy, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Yz, Layer2d.Bottom)]
    [InlineData(ResultTensor3.Zx, Layer2d.Bottom)]
    public void Element2dStressesValuesFromCombinationCaseTest(
      ResultTensor3 component, Layer2d layer) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      List<double> expectedP1 = ExpectedCombinationCaseC2p1Values(component, layer);
      List<double> expectedP2 = ExpectedCombinationCaseC2p2Values(component, layer);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet
        = result.Element2dStresses.ResultSubset(elementIds, layer);

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

    private List<double> ExpectedAnalysisCaseValues(ResultTensor3 component, Layer2d layer) {
      switch (component) {
        case ResultTensor3.Xx: return Element2dStressesA1.XxInMPa(layer);
        case ResultTensor3.Yy: return Element2dStressesA1.YyInMPa(layer);
        case ResultTensor3.Zz: return Element2dStressesA1.ZzInMPa(layer);
        case ResultTensor3.Xy: return Element2dStressesA1.XyInMPa(layer);
        case ResultTensor3.Yz: return Element2dStressesA1.YzInMPa(layer);
        case ResultTensor3.Zx: return Element2dStressesA1.ZxInMPa(layer);
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p1Values(
      ResultTensor3 component, Layer2d layer) {
      switch (component) {
        case ResultTensor3.Xx: return Element2dStressesC2p1.XxInMPa(layer);
        case ResultTensor3.Yy: return Element2dStressesC2p1.YyInMPa(layer);
        case ResultTensor3.Zz: return Element2dStressesC2p1.ZzInMPa(layer);
        case ResultTensor3.Xy: return Element2dStressesC2p1.XyInMPa(layer);
        case ResultTensor3.Yz: return Element2dStressesC2p1.YzInMPa(layer);
        case ResultTensor3.Zx: return Element2dStressesC2p1.ZxInMPa(layer);
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p2Values(
      ResultTensor3 component, Layer2d layer) {
      switch (component) {
        case ResultTensor3.Xx: return Element2dStressesC2p2.XxInMPa(layer);
        case ResultTensor3.Yy: return Element2dStressesC2p2.YyInMPa(layer);
        case ResultTensor3.Zz: return Element2dStressesC2p2.ZzInMPa(layer);
        case ResultTensor3.Xy: return Element2dStressesC2p2.XyInMPa(layer);
        case ResultTensor3.Yz: return Element2dStressesC2p2.YzInMPa(layer);
        case ResultTensor3.Zx: return Element2dStressesC2p2.ZxInMPa(layer);
      }

      throw new NotImplementedException();
    }
  }
}
