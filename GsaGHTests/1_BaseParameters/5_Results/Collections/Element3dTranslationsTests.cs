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
  public class Element3dTranslationsTests {

    private static readonly string ElementList = "6444 6555 7000 7015";

    [Fact]
    public void Element3dDisplacementsElement3dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element3dSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<ITranslation>, ITranslation, ResultVector3InAxis<Entity2dExtremaKey>> resultSet
        = result.Element3dDisplacements.ResultSubset(elementIds);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element3dDisplacementsElement3dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element3dSimple, 2);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<ITranslation>, ITranslation, ResultVector3InAxis<Entity2dExtremaKey>> resultSet
        = result.Element3dDisplacements.ResultSubset(elementIds);

      // Assert element IDs
      var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    public void Element3dDisplacementsMaxFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element3dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<ITranslation>, ITranslation, ResultVector3InAxis<Entity2dExtremaKey>> resultSet
        = result.Element3dDisplacements.ResultSubset(elementIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    public void Element3dDisplacementsMaxFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element3dSimple, 2);
      double expected = Math.Max(ExpectedCombinationCaseC2p1Values(component).Max(),
        ExpectedCombinationCaseC2p2Values(component).Max());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<ITranslation>, ITranslation, ResultVector3InAxis<Entity2dExtremaKey>> resultSet
        = result.Element3dDisplacements.ResultSubset(elementIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    public void Element3dDisplacementsMinFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element3dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<ITranslation>, ITranslation, ResultVector3InAxis<Entity2dExtremaKey>> resultSet
        = result.Element3dDisplacements.ResultSubset(elementIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    public void Element3dDisplacementsMinFromcombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element3dSimple, 2);
      double expected = Math.Min(ExpectedCombinationCaseC2p1Values(component).Min(),
        ExpectedCombinationCaseC2p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<ITranslation>, ITranslation, ResultVector3InAxis<Entity2dExtremaKey>> resultSet
        = result.Element3dDisplacements.ResultSubset(elementIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    public void Element3dDisplacementsValuesFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element3dSimple, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<ITranslation>, ITranslation, ResultVector3InAxis<Entity2dExtremaKey>> resultSet
        = result.Element3dDisplacements.ResultSubset(elementIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IMeshQuantity<ITranslation>> displacementQuantity = resultSet.Subset[id];

        Assert.Single(displacementQuantity);
        foreach (ITranslation displacement in displacementQuantity[0].Results()) {
          double x = TestsResultHelper.ResultsHelper(displacement, component);
          Assert.Equal(expected[i++], x, DoubleComparer.Default);
        }
      }
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    public void Element3dDisplacementsValuesFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element3dSimple, 2);
      List<double> expectedP1 = ExpectedCombinationCaseC2p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC2p2Values(component);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      IMeshResultSubset<IMeshQuantity<ITranslation>, ITranslation, ResultVector3InAxis<Entity2dExtremaKey>> resultSet
        = result.Element3dDisplacements.ResultSubset(elementIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IMeshQuantity<ITranslation>> displacementQuantity = resultSet.Subset[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, displacementQuantity.Count);

        foreach (ITranslation displacement in displacementQuantity[0].Results()) {
          double perm1 = TestsResultHelper.ResultsHelper(displacement, component);
          Assert.Equal(expectedP1[i++], perm1, DoubleComparer.Default);
        }
      }

      i = 0;
      foreach (int id in resultSet.Ids) {
        IList<IMeshQuantity<ITranslation>> displacementQuantity = resultSet.Subset[id];


        foreach (ITranslation displacement in displacementQuantity[1].Results()) {
          double perm2 = TestsResultHelper.ResultsHelper(displacement, component);
          Assert.Equal(expectedP2[i++], perm2, DoubleComparer.Default);
        }
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return Element3dTranslationsA1.XInMillimeter();
        case ResultVector6.Y: return Element3dTranslationsA1.YInMillimeter();
        case ResultVector6.Z: return Element3dTranslationsA1.ZInMillimeter();
        case ResultVector6.Xyz: return Element3dTranslationsA1.XyzInMillimeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p1Values(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return Element3dTranslationsC2p1.XInMillimeter();
        case ResultVector6.Y: return Element3dTranslationsC2p1.YInMillimeter();
        case ResultVector6.Z: return Element3dTranslationsC2p1.ZInMillimeter();
        case ResultVector6.Xyz: return Element3dTranslationsC2p1.XyzInMillimeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p2Values(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return Element3dTranslationsC2p2.XInMillimeter();
        case ResultVector6.Y: return Element3dTranslationsC2p2.YInMillimeter();
        case ResultVector6.Z: return Element3dTranslationsC2p2.ZInMillimeter();
        case ResultVector6.Xyz: return Element3dTranslationsC2p2.XyzInMillimeter();
      }

      throw new NotImplementedException();
    }
  }
}
