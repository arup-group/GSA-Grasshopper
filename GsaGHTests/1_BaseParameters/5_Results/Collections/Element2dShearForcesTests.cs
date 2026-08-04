using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Helpers;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class Element2dShearForcesTests {

    private static readonly string ElementList = "420 430 440 445";

    [Fact]
    public void Element2dShearForcesElement2dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> resultSet
        = result.Element2dShearForces.ResultSubset(elementIds);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element2dIShearForcesElement2dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> resultSet
        = result.Element2dShearForces.ResultSubset(elementIds);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(ResultVector2.Qx)]
    [InlineData(ResultVector2.Qy)]
    public void Element2dShearForcesMaxFromAnalysisCaseTest(ResultVector2 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> resultSet
        = result.Element2dShearForces.ResultSubset(elementIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultVector2.Qx)]
    [InlineData(ResultVector2.Qy)]
    public void Element2dShearForcesMaxFromCombinationCaseTest(ResultVector2 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Max(ExpectedCombinationCaseC2p1Values(component).Max(),
        ExpectedCombinationCaseC2p2Values(component).Max());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> resultSet
        = result.Element2dShearForces.ResultSubset(elementIds);
      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultVector2.Qx)]
    [InlineData(ResultVector2.Qy)]
    public void Element2dShearForcesMinFromAnalysisCaseTest(ResultVector2 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> resultSet
        = result.Element2dShearForces.ResultSubset(elementIds);
      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultVector2.Qx)]
    [InlineData(ResultVector2.Qy)]
    public void Element2dShearForcessMinFromcombinationCaseTest(ResultVector2 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Min(ExpectedCombinationCaseC2p1Values(component).Min(),
        ExpectedCombinationCaseC2p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> resultSet
        = result.Element2dShearForces.ResultSubset(elementIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultVector2.Qx)]
    [InlineData(ResultVector2.Qy)]
    public void Element2dShearForcesValuesFromAnalysisCaseTest(ResultVector2 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> resultSet
        = result.Element2dShearForces.ResultSubset(elementIds);
      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IMeshQuantity<IShear2d>> shearQuantity = resultSet.Subset[id];

        Assert.Single(shearQuantity);
        foreach (IShear2d shear2d in shearQuantity[0].Results()) {
          double x = TestsResultHelper.ResultsHelper(shear2d, component);
          Assert.Equal(expected[i++], x, DoubleComparer.Default);
        }
      }
    }

    [Theory]
    [InlineData(ResultVector2.Qx)]
    [InlineData(ResultVector2.Qy)]
    public void Element2dShearForcesValuesFromCombinationCaseTest(ResultVector2 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      List<double> expectedP1 = ExpectedCombinationCaseC2p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC2p2Values(component);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> resultSet
        = result.Element2dShearForces.ResultSubset(elementIds);
      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IMeshQuantity<IShear2d>> shearQuantity = resultSet.Subset[id];

        Assert.Equal(2, shearQuantity.Count);

        foreach (IShear2d shear2d in shearQuantity[0].Results()) {
          double perm1 = TestsResultHelper.ResultsHelper(shear2d, component);
          Assert.Equal(expectedP1[i++], perm1, DoubleComparer.Default);
        }
      }

      i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IMeshQuantity<IShear2d>> shearQuantity = resultSet.Subset[id];


        foreach (IShear2d shear2d in shearQuantity[1].Results()) {
          double perm2 = TestsResultHelper.ResultsHelper(shear2d, component);
          Assert.Equal(expectedP2[i++], perm2, DoubleComparer.Default);
        }
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector2 component) {
      switch (component) {
        case ResultVector2.Qx: return Element2dShearForcesA1.QxInKiloNewtonPerMeter();
        case ResultVector2.Qy: return Element2dShearForcesA1.QyInKiloNewtonPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p1Values(ResultVector2 component) {
      switch (component) {
        case ResultVector2.Qx: return Element2dShearForcesC2p1.QxInKiloNewtonPerMeter();
        case ResultVector2.Qy: return Element2dShearForcesC2p1.QyInKiloNewtonPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p2Values(ResultVector2 component) {
      switch (component) {
        case ResultVector2.Qx: return Element2dShearForcesC2p2.QxInKiloNewtonPerMeter();
        case ResultVector2.Qy: return Element2dShearForcesC2p2.QyInKiloNewtonPerMeter();
      }

      throw new NotImplementedException();
    }
  }
}
