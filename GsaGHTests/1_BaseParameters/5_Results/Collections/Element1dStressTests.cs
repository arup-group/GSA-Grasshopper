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
  public class Element1dStressTests {
    public enum ResultStress1dHelperEnum {
      Axial,
      ShearY,
      ShearZ,
      ByPos,
      ByNeg,
      BzPos,
      BzNeg,
      C1,
      C2
    }

    private static readonly string ElementList = "2 to 3";

    [Fact]
    public void Element1dStressElement1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dStress, IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, 1);

      // Assert element IDs
      var expectedIds = result.Model.Model.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element1dStresssElement1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dStress, IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, 1);

      // Assert element IDs
      var expectedIds = result.Model.Model.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(ResultStress1dHelperEnum.Axial)]
    [InlineData(ResultStress1dHelperEnum.ShearY)]
    [InlineData(ResultStress1dHelperEnum.ShearZ)]
    [InlineData(ResultStress1dHelperEnum.ByPos)]
    [InlineData(ResultStress1dHelperEnum.ByNeg)]
    [InlineData(ResultStress1dHelperEnum.BzPos)]
    [InlineData(ResultStress1dHelperEnum.BzNeg)]
    [InlineData(ResultStress1dHelperEnum.C1)]
    [InlineData(ResultStress1dHelperEnum.C2)]
    public void Element1dStresssMaxFromAnalysisCaseTest(ResultStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dStress, IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, 4);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(ResultStress1dHelperEnum.Axial)]
    [InlineData(ResultStress1dHelperEnum.ShearY)]
    [InlineData(ResultStress1dHelperEnum.ShearZ)]
    [InlineData(ResultStress1dHelperEnum.ByPos)]
    [InlineData(ResultStress1dHelperEnum.ByNeg)]
    [InlineData(ResultStress1dHelperEnum.BzPos)]
    [InlineData(ResultStress1dHelperEnum.BzNeg)]
    [InlineData(ResultStress1dHelperEnum.C1)]
    [InlineData(ResultStress1dHelperEnum.C2)]
    public void Element1dStresssMaxFromCombinationCaseTest(ResultStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(ExpectedCombinationCaseC4p1Values(component).Max(),
        ExpectedCombinationCaseC4p2Values(component).Max());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dStress, IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, 4);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(ResultStress1dHelperEnum.Axial)]
    [InlineData(ResultStress1dHelperEnum.ShearY)]
    [InlineData(ResultStress1dHelperEnum.ShearZ)]
    [InlineData(ResultStress1dHelperEnum.ByPos)]
    [InlineData(ResultStress1dHelperEnum.ByNeg)]
    [InlineData(ResultStress1dHelperEnum.BzPos)]
    [InlineData(ResultStress1dHelperEnum.BzNeg)]
    [InlineData(ResultStress1dHelperEnum.C1)]
    [InlineData(ResultStress1dHelperEnum.C2)]
    public void Element1dStresssMinFromAnalysisCaseTest(ResultStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dStress, IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, 4);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultStress1dHelperEnum.Axial)]
    [InlineData(ResultStress1dHelperEnum.ShearY)]
    [InlineData(ResultStress1dHelperEnum.ShearZ)]
    [InlineData(ResultStress1dHelperEnum.ByPos)]
    [InlineData(ResultStress1dHelperEnum.ByNeg)]
    [InlineData(ResultStress1dHelperEnum.BzPos)]
    [InlineData(ResultStress1dHelperEnum.BzNeg)]
    [InlineData(ResultStress1dHelperEnum.C1)]
    [InlineData(ResultStress1dHelperEnum.C2)]
    public void Element1dStresssMinFromcombinationCaseTest(ResultStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(ExpectedCombinationCaseC4p1Values(component).Min(),
        ExpectedCombinationCaseC4p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dStress, IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, 4);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultStress1dHelperEnum.Axial)]
    [InlineData(ResultStress1dHelperEnum.ShearY)]
    [InlineData(ResultStress1dHelperEnum.ShearZ)]
    [InlineData(ResultStress1dHelperEnum.ByPos)]
    [InlineData(ResultStress1dHelperEnum.ByNeg)]
    [InlineData(ResultStress1dHelperEnum.BzPos)]
    [InlineData(ResultStress1dHelperEnum.BzNeg)]
    [InlineData(ResultStress1dHelperEnum.C1)]
    [InlineData(ResultStress1dHelperEnum.C2)]
    public void Element1dStresssValuesFromAnalysisCaseTest(ResultStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);
      int positionsCount = 4;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dStress, IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IEntity1dStress> stressQuantity = resultSet.Subset[id];

        // for analysis case results we expect 4 positions
        Assert.Single(stressQuantity);
        var positions = Enumerable.Range(0, positionsCount).Select(
        k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double x = TestsResultHelper.ResultsHelper(stressQuantity[0].Results[position], component);
          Assert.Equal(expected[i++], x);
        }
      }
    }

    [Theory]
    [InlineData(ResultStress1dHelperEnum.Axial)]
    [InlineData(ResultStress1dHelperEnum.ShearY)]
    [InlineData(ResultStress1dHelperEnum.ShearZ)]
    [InlineData(ResultStress1dHelperEnum.ByPos)]
    [InlineData(ResultStress1dHelperEnum.ByNeg)]
    [InlineData(ResultStress1dHelperEnum.BzPos)]
    [InlineData(ResultStress1dHelperEnum.BzNeg)]
    [InlineData(ResultStress1dHelperEnum.C1)]
    [InlineData(ResultStress1dHelperEnum.C2)]
    public void Element1dStresssValuesFromCombinationCaseTest(ResultStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = ExpectedCombinationCaseC4p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC4p2Values(component);
      int positionsCount = 4;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 1);
      IEntity1dResultSubset<IEntity1dStress, IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet
        = result.Element1dStresses.ResultSubset(elementIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IEntity1dStress> displacementQuantity = resultSet.Subset[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, displacementQuantity.Count);

        var positions = Enumerable.Range(0, positionsCount).Select(
        k => (double)k / (positionsCount - 1)).ToList();
        foreach (double position in positions) {
          double perm1 = TestsResultHelper.ResultsHelper(displacementQuantity[0].Results[position], component);
          Assert.Equal(expectedP1[i], perm1);
          double perm2 = TestsResultHelper.ResultsHelper(displacementQuantity[1].Results[position], component);
          Assert.Equal(expectedP2[i++], perm2);
        }
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultStress1dHelperEnum component) {
      switch (component) {
        case ResultStress1dHelperEnum.Axial: return Element1dStressA1.AxialInMPa();

        case ResultStress1dHelperEnum.ShearY: return Element1dStressA1.SyInMPa();

        case ResultStress1dHelperEnum.ShearZ: return Element1dStressA1.SzInMPa();

        case ResultStress1dHelperEnum.ByPos: return Element1dStressA1.ByPosInMPa();

        case ResultStress1dHelperEnum.ByNeg: return Element1dStressA1.ByNegInMPa();

        case ResultStress1dHelperEnum.BzPos: return Element1dStressA1.BzPosInMPa();

        case ResultStress1dHelperEnum.BzNeg: return Element1dStressA1.BzNegInMPa();

        case ResultStress1dHelperEnum.C1: return Element1dStressA1.C1InMPa();

        case ResultStress1dHelperEnum.C2: return Element1dStressA1.C2InMPa();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p1Values(ResultStress1dHelperEnum component) {
      switch (component) {
        case ResultStress1dHelperEnum.Axial: return Element1dStressC4p1.AxialInMPa();

        case ResultStress1dHelperEnum.ShearY: return Element1dStressC4p1.SyInMPa();

        case ResultStress1dHelperEnum.ShearZ: return Element1dStressC4p1.SzInMPa();

        case ResultStress1dHelperEnum.ByPos: return Element1dStressC4p1.ByPosInMPa();

        case ResultStress1dHelperEnum.ByNeg: return Element1dStressC4p1.ByNegInMPa();

        case ResultStress1dHelperEnum.BzPos: return Element1dStressC4p1.BzPosInMPa();

        case ResultStress1dHelperEnum.BzNeg: return Element1dStressC4p1.BzNegInMPa();

        case ResultStress1dHelperEnum.C1: return Element1dStressC4p1.C1InMPa();

        case ResultStress1dHelperEnum.C2: return Element1dStressC4p1.C2InMPa();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p2Values(ResultStress1dHelperEnum component) {
      switch (component) {
        case ResultStress1dHelperEnum.Axial: return Element1dStressC4p2.AxialInMPa();

        case ResultStress1dHelperEnum.ShearY: return Element1dStressC4p2.SyInMPa();

        case ResultStress1dHelperEnum.ShearZ: return Element1dStressC4p2.SzInMPa();

        case ResultStress1dHelperEnum.ByPos: return Element1dStressC4p2.ByPosInMPa();

        case ResultStress1dHelperEnum.ByNeg: return Element1dStressC4p2.ByNegInMPa();

        case ResultStress1dHelperEnum.BzPos: return Element1dStressC4p2.BzPosInMPa();

        case ResultStress1dHelperEnum.BzNeg: return Element1dStressC4p2.BzNegInMPa();

        case ResultStress1dHelperEnum.C1: return Element1dStressC4p2.C1InMPa();

        case ResultStress1dHelperEnum.C2: return Element1dStressC4p2.C2InMPa();
      }

      throw new NotImplementedException();
    }
  }
}
