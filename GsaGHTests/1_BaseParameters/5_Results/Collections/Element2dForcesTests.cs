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
  public class Element2dForcesTests {

    private static readonly string ElementList = "420 430 440 445";

    [Fact]
    public void Element2dForcesElement2dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dForces.ResultSubset(elementIds);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element2dForcesElement2dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dForces.ResultSubset(elementIds);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(ResultTensor2InAxis.Nx)]
    [InlineData(ResultTensor2InAxis.Ny)]
    [InlineData(ResultTensor2InAxis.Nxy)]
    public void Element2dForcesMaxFromAnalysisCaseTest(ResultTensor2InAxis component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dForces.ResultSubset(elementIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultTensor2InAxis.Nx)]
    [InlineData(ResultTensor2InAxis.Ny)]
    [InlineData(ResultTensor2InAxis.Nxy)]
    public void Element2dForcesMaxFromCombinationCaseTest(ResultTensor2InAxis component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Max(ExpectedCombinationCaseC2p1Values(component).Max(),
        ExpectedCombinationCaseC2p2Values(component).Max());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dForces.ResultSubset(elementIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultTensor2InAxis.Nx)]
    [InlineData(ResultTensor2InAxis.Ny)]
    [InlineData(ResultTensor2InAxis.Nxy)]
    public void Element2dForcesMinFromAnalysisCaseTest(ResultTensor2InAxis component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dForces.ResultSubset(elementIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultTensor2InAxis.Nx)]
    [InlineData(ResultTensor2InAxis.Ny)]
    [InlineData(ResultTensor2InAxis.Nxy)]
    public void Element2dForcesMinFromcombinationCaseTest(ResultTensor2InAxis component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Min(ExpectedCombinationCaseC2p1Values(component).Min(),
        ExpectedCombinationCaseC2p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dForces.ResultSubset(elementIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultTensor2InAxis.Nx)]
    [InlineData(ResultTensor2InAxis.Ny)]
    [InlineData(ResultTensor2InAxis.Nxy)]
    public void Element2dForcesValuesFromAnalysisCaseTest(ResultTensor2InAxis component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dForces.ResultSubset(elementIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IMeshQuantity<IForce2d>> forceQuantity = resultSet.Subset[id];

        Assert.Single(forceQuantity);
        foreach (IForce2d force2d in forceQuantity[0].Results()) {
          double x = TestsResultHelper.ResultsHelper(force2d, component);
          Assert.Equal(expected[i++], x, DoubleComparer.Default);
        }
      }
    }

    [Theory]
    [InlineData(ResultTensor2InAxis.Nx)]
    [InlineData(ResultTensor2InAxis.Ny)]
    [InlineData(ResultTensor2InAxis.Nxy)]
    public void Element2dForcesValuesFromCombinationCaseTest(ResultTensor2InAxis component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      List<double> expectedP1 = ExpectedCombinationCaseC2p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC2p2Values(component);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IMeshResultSubset<IMeshQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> resultSet
        = result.Element2dForces.ResultSubset(elementIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IMeshQuantity<IForce2d>> forceQuantity = resultSet.Subset[id];

        Assert.Equal(2, forceQuantity.Count);

        foreach (IForce2d force2d in forceQuantity[0].Results()) {
          double perm1 = TestsResultHelper.ResultsHelper(force2d, component);
          Assert.Equal(expectedP1[i++], perm1, DoubleComparer.Default);
        }
      }

      i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IMeshQuantity<IForce2d>> forceQuantity = resultSet.Subset[id];


        foreach (IForce2d force2d in forceQuantity[1].Results()) {
          double perm2 = TestsResultHelper.ResultsHelper(force2d, component);
          Assert.Equal(expectedP2[i++], perm2, DoubleComparer.Default);
        }
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultTensor2InAxis component) {
      switch (component) {
        case ResultTensor2InAxis.Nx: return Element2dForcesA1.NxInKiloNewtonPerMeter();
        case ResultTensor2InAxis.Ny: return Element2dForcesA1.NyInKiloNewtonPerMeter();
        case ResultTensor2InAxis.Nxy: return Element2dForcesA1.NxyInKiloNewtonPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p1Values(ResultTensor2InAxis component) {
      switch (component) {
        case ResultTensor2InAxis.Nx: return Element2dForcesC2p1.NxInKiloNewtonPerMeter();
        case ResultTensor2InAxis.Ny: return Element2dForcesC2p1.NyInKiloNewtonPerMeter();
        case ResultTensor2InAxis.Nxy: return Element2dForcesC2p1.NxyInKiloNewtonPerMeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p2Values(ResultTensor2InAxis component) {
      switch (component) {
        case ResultTensor2InAxis.Nx: return Element2dForcesC2p2.NxInKiloNewtonPerMeter();
        case ResultTensor2InAxis.Ny: return Element2dForcesC2p2.NyInKiloNewtonPerMeter();
        case ResultTensor2InAxis.Nxy: return Element2dForcesC2p2.NxyInKiloNewtonPerMeter();
      }

      throw new NotImplementedException();
    }
  }
}
