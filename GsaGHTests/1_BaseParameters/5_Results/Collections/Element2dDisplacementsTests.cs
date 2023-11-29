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
  public class Element2dDisplacementsTests {

    private static readonly string ElementList = "420 430 440 445";

    [Fact]
    public void Element2dDisplacementsElement2dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IEntity2dResultSubset<IEntity2dQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> resultSet
        = result.Element2dDisplacements.ResultSubset(elementIds);

      // Assert element IDs
      var expectedIds = result.Model.Model.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void Element2dDisplacementsElement2dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.Element2dSimple, 2);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IEntity2dResultSubset<IEntity2dQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> resultSet
        = result.Element2dDisplacements.ResultSubset(elementIds);

      // Assert element IDs
      var expectedIds = result.Model.Model.Elements(ElementList).Keys.OrderBy(x => x).ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    public void Element2dDisplacementsMaxFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IEntity2dResultSubset<IEntity2dQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> resultSet
        = result.Element2dDisplacements.ResultSubset(elementIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    public void Element2dDisplacementsMaxFromCombinationCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Max(ExpectedCombinationCaseC2p1Values(component).Max(),
        ExpectedCombinationCaseC2p2Values(component).Max());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IEntity2dResultSubset<IEntity2dQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> resultSet
        = result.Element2dDisplacements.ResultSubset(elementIds);

      // Assert Max in set
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    public void Element2dDisplacementsMinFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IEntity2dResultSubset<IEntity2dQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> resultSet
        = result.Element2dDisplacements.ResultSubset(elementIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    public void Element2dDisplacementsMinFromcombinationCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Min(ExpectedCombinationCaseC2p1Values(component).Min(),
        ExpectedCombinationCaseC2p2Values(component).Min());

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IEntity2dResultSubset<IEntity2dQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> resultSet
        = result.Element2dDisplacements.ResultSubset(elementIds);

      // Assert Max in set
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    public void Element2dDisplacementsValuesFromAnalysisCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IEntity2dResultSubset<IEntity2dQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> resultSet
        = result.Element2dDisplacements.ResultSubset(elementIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IEntity2dQuantity<IDisplacement>> displacementQuantity = resultSet.Subset[id];

        Assert.Single(displacementQuantity);
        foreach (IDisplacement displacement in displacementQuantity[0].Results()) {
          double x = TestsResultHelper.ResultsHelper(displacement, component);
          Assert.Equal(expected[i++], x);
        }
      }
    }

    [Theory]
    [InlineData(ResultVector6HelperEnum.X)]
    [InlineData(ResultVector6HelperEnum.Y)]
    [InlineData(ResultVector6HelperEnum.Z)]
    [InlineData(ResultVector6HelperEnum.Xyz)]
    public void Element2dDisplacementsValuesFromCombinationCaseTest(ResultVector6HelperEnum component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      List<double> expectedP1 = ExpectedCombinationCaseC2p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC2p2Values(component);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      IEntity2dResultSubset<IEntity2dQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> resultSet
        = result.Element2dDisplacements.ResultSubset(elementIds);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IEntity2dQuantity<IDisplacement>> displacementQuantity = resultSet.Subset[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, displacementQuantity.Count);

        foreach (IDisplacement displacement in displacementQuantity[0].Results()) {
          double perm1 = TestsResultHelper.ResultsHelper(displacement, component);
          Assert.Equal(expectedP1[i++], perm1);
        }
      }

      i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IEntity2dQuantity<IDisplacement>> displacementQuantity = resultSet.Subset[id];


        foreach (IDisplacement displacement in displacementQuantity[1].Results()) {
          double perm2 = TestsResultHelper.ResultsHelper(displacement, component);
          Assert.Equal(expectedP2[i++], perm2);
        }
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Element2dDisplacementsA1.XInMillimeter();
        case ResultVector6HelperEnum.Y: return Element2dDisplacementsA1.YInMillimeter();
        case ResultVector6HelperEnum.Z: return Element2dDisplacementsA1.ZInMillimeter();
        case ResultVector6HelperEnum.Xyz: return Element2dDisplacementsA1.XyzInMillimeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p1Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Element2dDisplacementsC2p1.XInMillimeter();
        case ResultVector6HelperEnum.Y: return Element2dDisplacementsC2p1.YInMillimeter();
        case ResultVector6HelperEnum.Z: return Element2dDisplacementsC2p1.ZInMillimeter();
        case ResultVector6HelperEnum.Xyz: return Element2dDisplacementsC2p1.XyzInMillimeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p2Values(ResultVector6HelperEnum component) {
      switch (component) {
        case ResultVector6HelperEnum.X: return Element2dDisplacementsC2p2.XInMillimeter();
        case ResultVector6HelperEnum.Y: return Element2dDisplacementsC2p2.YInMillimeter();
        case ResultVector6HelperEnum.Z: return Element2dDisplacementsC2p2.ZInMillimeter();
        case ResultVector6HelperEnum.Xyz: return Element2dDisplacementsC2p2.XyzInMillimeter();
      }

      throw new NotImplementedException();
    }
  }
}
