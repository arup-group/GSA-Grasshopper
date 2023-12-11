using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Element2dMomentsTests {

    private static readonly string ElementList = "420 430 440 445";

    [Fact]
    public void Element2dMomentsElement2dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dMoments.ResultSubset(elementIds);

      // Assert element IDs
      var expectedIds = result.Model.Model.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element2dIMomentElement2dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dMoments.ResultSubset(elementIds);

      // Assert element IDs
      var expectedIds = result.Model.Model.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(ResultTensor2AroundAxisHelperEnum.Mx)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.My)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.Mxy)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.WoodArmerX)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.WoodArmerY)]
    public void Element2dMomentsMaxFromAnalysisCaseTest(ResultTensor2AroundAxisHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dMoments.ResultSubset(elementIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(ResultTensor2AroundAxisHelperEnum.Mx)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.My)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.Mxy)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.WoodArmerX)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.WoodArmerY)]
    public void Element2dMomentMaxFromCombinationCaseTest(ResultTensor2AroundAxisHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Max(ExpectedCombinationCaseC2p1Values(component).Max(),
        ExpectedCombinationCaseC2p2Values(component).Max());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dMoments.ResultSubset(elementIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(ResultTensor2AroundAxisHelperEnum.Mx)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.My)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.Mxy)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.WoodArmerX)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.WoodArmerY)]
    public void Element2dMomentsMinFromAnalysisCaseTest(ResultTensor2AroundAxisHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dMoments.ResultSubset(elementIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultTensor2AroundAxisHelperEnum.Mx)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.My)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.Mxy)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.WoodArmerX)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.WoodArmerY)]
    public void Element2dMomentsMinFromcombinationCaseTest(ResultTensor2AroundAxisHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Min(ExpectedCombinationCaseC2p1Values(component).Min(),
        ExpectedCombinationCaseC2p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dMoments.ResultSubset(elementIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultTensor2AroundAxisHelperEnum.Mx)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.My)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.Mxy)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.WoodArmerX)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.WoodArmerY)]
    public void Element2dMomentsValuesFromAnalysisCaseTest(ResultTensor2AroundAxisHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dMoments.ResultSubset(elementIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IMeshQuantity<IMoment2d>> momentQuantity = resultSet.Subset[id];

        Assert.Single(momentQuantity);
        foreach (IMoment2d moment2d in momentQuantity[0].Results()) {
          double x = TestsResultHelper.ResultsHelper(moment2d, component);
          Assert.Equal(expected[i++], x);
        }
      }
    }

    [Theory]
    [InlineData(ResultTensor2AroundAxisHelperEnum.Mx)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.My)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.Mxy)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.WoodArmerX)]
    [InlineData(ResultTensor2AroundAxisHelperEnum.WoodArmerY)]
    public void Element2dMomentsValuesFromCombinationCaseTest(ResultTensor2AroundAxisHelperEnum component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      List<double> expectedP1 = ExpectedCombinationCaseC2p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC2p2Values(component);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dMoments.ResultSubset(elementIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IMeshQuantity<IMoment2d>> momentQuantity = resultSet.Subset[id];

        Assert.Equal(2, momentQuantity.Count);

        foreach (IMoment2d moment2d in momentQuantity[0].Results()) {
          double perm1 = TestsResultHelper.ResultsHelper(moment2d, component);
          Assert.Equal(expectedP1[i++], perm1);
        }
      }

      i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IMeshQuantity<IMoment2d>> momentQuantity = resultSet.Subset[id];


        foreach (IMoment2d moment2d in momentQuantity[1].Results()) {
          double perm2 = TestsResultHelper.ResultsHelper(moment2d, component);
          Assert.Equal(expectedP2[i++], perm2);
        }
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultTensor2AroundAxisHelperEnum component) {
      switch (component) {
        case ResultTensor2AroundAxisHelperEnum.Mx: return Element2dMomentsA1.MxInKiloNewton();
        case ResultTensor2AroundAxisHelperEnum.My: return Element2dMomentsA1.MyInKiloNewton();
        case ResultTensor2AroundAxisHelperEnum.Mxy: return Element2dMomentsA1.MxyInKiloNewton();
        case ResultTensor2AroundAxisHelperEnum.WoodArmerX: return Element2dMomentsA1.WoodArmerXInKiloNewton();
        case ResultTensor2AroundAxisHelperEnum.WoodArmerY: return Element2dMomentsA1.WoodArmerYInKiloNewton();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p1Values(ResultTensor2AroundAxisHelperEnum component) {
      switch (component) {
        case ResultTensor2AroundAxisHelperEnum.Mx: return Element2dMomentsC2p1.MxInKiloNewton();
        case ResultTensor2AroundAxisHelperEnum.My: return Element2dMomentsC2p1.MyInKiloNewton();
        case ResultTensor2AroundAxisHelperEnum.Mxy: return Element2dMomentsC2p1.MxyInKiloNewton();
        case ResultTensor2AroundAxisHelperEnum.WoodArmerX: return Element2dMomentsC2p1.WoodArmerXInKiloNewton();
        case ResultTensor2AroundAxisHelperEnum.WoodArmerY: return Element2dMomentsC2p1.WoodArmerYInKiloNewton();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p2Values(ResultTensor2AroundAxisHelperEnum component) {
      switch (component) {
        case ResultTensor2AroundAxisHelperEnum.Mx: return Element2dMomentsC2p2.MxInKiloNewton();
        case ResultTensor2AroundAxisHelperEnum.My: return Element2dMomentsC2p2.MyInKiloNewton();
        case ResultTensor2AroundAxisHelperEnum.Mxy: return Element2dMomentsC2p2.MxyInKiloNewton();
        case ResultTensor2AroundAxisHelperEnum.WoodArmerX: return Element2dMomentsC2p2.WoodArmerXInKiloNewton();
        case ResultTensor2AroundAxisHelperEnum.WoodArmerY: return Element2dMomentsC2p2.WoodArmerYInKiloNewton();
      }

      throw new NotImplementedException();
    }
  }
}
