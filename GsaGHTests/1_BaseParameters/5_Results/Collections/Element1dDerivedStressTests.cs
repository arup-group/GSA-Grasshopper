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
  public class Element1dDerivedStressTests {
    public enum ResultDerivedStress1dHelperEnum {
      ShearY,
      ShearZ,
      Torsion,
      VonMises
    }

    private static readonly string ElementList = "2 to 3";

    [Fact]
    public void Element1dDerivedStressElement1dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList);
      IElement1dResultSubset<IElement1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Element1dExtremaKey>> resultSet
        = result.Element1dDerivedStresses.ResultSubset(elementIds, 1);

      // Assert element IDs
      var expectedIds = result.Model.Model.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element1dStresssElement1dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList);
      IElement1dResultSubset<IElement1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Element1dExtremaKey>> resultSet
        = result.Element1dDerivedStresses.ResultSubset(elementIds, 1);

      // Assert element IDs
      var expectedIds = result.Model.Model.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearY)]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearZ)]
    [InlineData(ResultDerivedStress1dHelperEnum.Torsion)]
    [InlineData(ResultDerivedStress1dHelperEnum.VonMises)]
    public void Element1dStresssMaxFromAnalysisCaseTest(ResultDerivedStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList);
      IElement1dResultSubset<IElement1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Element1dExtremaKey>> resultSet
        = result.Element1dDerivedStresses.ResultSubset(elementIds, 4);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearY)]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearZ)]
    [InlineData(ResultDerivedStress1dHelperEnum.Torsion)]
    [InlineData(ResultDerivedStress1dHelperEnum.VonMises)]
    public void Element1dStresssMaxFromCombinationCaseTest(ResultDerivedStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(ExpectedCombinationCaseC4p1Values(component).Max(),
        ExpectedCombinationCaseC4p2Values(component).Max());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList);
      IElement1dResultSubset<IElement1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Element1dExtremaKey>> resultSet
        = result.Element1dDerivedStresses.ResultSubset(elementIds, 4);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearY)]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearZ)]
    [InlineData(ResultDerivedStress1dHelperEnum.Torsion)]
    [InlineData(ResultDerivedStress1dHelperEnum.VonMises)]
    public void Element1dStresssMinFromAnalysisCaseTest(ResultDerivedStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList);
      IElement1dResultSubset<IElement1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Element1dExtremaKey>> resultSet
        = result.Element1dDerivedStresses.ResultSubset(elementIds, 4);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearY)]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearZ)]
    [InlineData(ResultDerivedStress1dHelperEnum.Torsion)]
    [InlineData(ResultDerivedStress1dHelperEnum.VonMises)]
    public void Element1dStresssMinFromcombinationCaseTest(ResultDerivedStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(ExpectedCombinationCaseC4p1Values(component).Min(),
        ExpectedCombinationCaseC4p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList);
      IElement1dResultSubset<IElement1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Element1dExtremaKey>> resultSet
        = result.Element1dDerivedStresses.ResultSubset(elementIds, 4);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearY)]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearZ)]
    [InlineData(ResultDerivedStress1dHelperEnum.Torsion)]
    [InlineData(ResultDerivedStress1dHelperEnum.VonMises)]
    public void Element1dStresssValuesFromAnalysisCaseTest(ResultDerivedStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);
      int positionsCount = 4;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList);
      IElement1dResultSubset<IElement1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Element1dExtremaKey>> resultSet
        = result.Element1dDerivedStresses.ResultSubset(elementIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IElement1dDerivedStress> stressQuantity = resultSet.Subset[id];

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
    [InlineData(ResultDerivedStress1dHelperEnum.ShearY)]
    [InlineData(ResultDerivedStress1dHelperEnum.ShearZ)]
    [InlineData(ResultDerivedStress1dHelperEnum.Torsion)]
    [InlineData(ResultDerivedStress1dHelperEnum.VonMises)]
    public void Element1dStresssValuesFromCombinationCaseTest(ResultDerivedStress1dHelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = ExpectedCombinationCaseC4p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC4p2Values(component);
      int positionsCount = 4;

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList);
      IElement1dResultSubset<IElement1dDerivedStress, IStress1dDerived, ResultDerivedStress1d<Element1dExtremaKey>> resultSet
        = result.Element1dDerivedStresses.ResultSubset(elementIds, positionsCount);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IElement1dDerivedStress> displacementQuantity = resultSet.Subset[id];

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

    private List<double> ExpectedAnalysisCaseValues(ResultDerivedStress1dHelperEnum component) {
      switch (component) {
        case ResultDerivedStress1dHelperEnum.ShearY: return Element1dDerivedStressA1.SEyInMPa();

        case ResultDerivedStress1dHelperEnum.ShearZ: return Element1dDerivedStressA1.SEzInMPa();

        case ResultDerivedStress1dHelperEnum.Torsion: return Element1dDerivedStressA1.StInMPa();

        case ResultDerivedStress1dHelperEnum.VonMises: return Element1dDerivedStressA1.VonMisesInMPa();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p1Values(ResultDerivedStress1dHelperEnum component) {
      switch (component) {
        case ResultDerivedStress1dHelperEnum.ShearY: return Element1dDerivedStressC4p1.SEyInMPa();

        case ResultDerivedStress1dHelperEnum.ShearZ: return Element1dDerivedStressC4p1.SEzInMPa();

        case ResultDerivedStress1dHelperEnum.Torsion: return Element1dDerivedStressC4p1.StInMPa();

        case ResultDerivedStress1dHelperEnum.VonMises: return Element1dDerivedStressC4p1.VonMisesInMPa();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p2Values(ResultDerivedStress1dHelperEnum component) {
      switch (component) {
        case ResultDerivedStress1dHelperEnum.ShearY: return Element1dDerivedStressC4p2.SEyInMPa();

        case ResultDerivedStress1dHelperEnum.ShearZ: return Element1dDerivedStressC4p2.SEzInMPa();

        case ResultDerivedStress1dHelperEnum.Torsion: return Element1dDerivedStressC4p2.StInMPa();

        case ResultDerivedStress1dHelperEnum.VonMises: return Element1dDerivedStressC4p2.VonMisesInMPa();
      }

      throw new NotImplementedException();
    }
  }
}
