using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;

using Double6 = GsaAPI.Double6;

using GsaGH.Components;
using GsaGH.Helpers;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters.Results;

using OasysUnits;
using OasysUnits.Units;

using Xunit;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class Element2dDisplacementsTests {
    private static readonly string ElementList = "420 430 440 445";

    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new Element2dDisplacements();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void EmptySelectedPermutationReportsUnavailable(int selection) {
      var result = MixedPermutationResult(new List<int> { 1, 2 });
      var comp = new Element2dDisplacements();
      comp.SetSelected(0, selection);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.ComputeOutput(comp);

      Assert.Contains("Result is not available for the selected component",
        comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
      Assert.All(comp.Params.Output, output => Assert.True(
        output.VolatileDataCount == 0 || output.VolatileData.AllData(false).All(item => item == null)));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EmptyAnalysisDisplacementsReportUnavailable(bool allPermutations) {
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      var ids = result.ElementIds(ElementList, 2);
      foreach (int id in ids) {
        result.Element2dDisplacements.Cache[id] = Entity2dResultsFactory.CreateDisplacements(
          new ReadOnlyCollection<Double6>(new List<Double6>()));
      }
      if (allPermutations) {
        result.SelectedPermutationIds = new List<int> { -1 };
      }
      var comp = new Element2dDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.ComputeOutput(comp);
      Assert.Contains("Result is not available for the selected component",
        comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void ValidPermutationRetainsItsNumberWhenAnotherIsEmpty(int selected, bool emptyFirst) {
      var result = MixedPermutationResult(new List<int> { selected }, emptyFirst);
      var comp = new Element2dDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, 0);
      Assert.NotEmpty(paths);
      Assert.All(paths, path => Assert.Equal(selected, path.Indices[1]));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    private static GsaResult MixedPermutationResult(List<int> selectedPermutations, bool emptyFirst = false) {
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      var ids = result.ElementIds(ElementList, 2);
      result.Element2dDisplacements.ResultSubset(ids);
      foreach (int id in ids) {
        int count = result.Element2dDisplacements.Cache[id][0].Results().Count;
        var valid = new ReadOnlyCollection<Double6>(
          Enumerable.Repeat(new Double6(1, 2, 3, 0, 0, 0), count).ToList());
        var empty = new ReadOnlyCollection<Double6>(new List<Double6>());
        result.Element2dDisplacements.Cache[id] = Entity2dResultsFactory.CreateDisplacements(
          new ReadOnlyCollection<ReadOnlyCollection<Double6>>(emptyFirst
            ? new List<ReadOnlyCollection<Double6>> { empty, valid }
            : new List<ReadOnlyCollection<Double6>> { valid, empty }));
      }

      result.SelectedPermutationIds = selectedPermutations;
      return result;
    }

    [Fact]
    public void Element2dDisplacementsElement2dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      var comp = new Element2dDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Equal(elementIds.Count, paths.Count);

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
          var expectedIds = result.Model.ApiModel.Elements(ElementList).Keys.OrderBy(x => x).ToList();
          Assert.Equal(expectedIds[j], ids[j]);
        }
      }
    }

    [Fact]
    public void Element2dDisplacementsElement2dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      var comp = new Element2dDisplacements();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      for (int i = 0; i < comp.Params.Output.Count; i++) { // loop through each output
        IList<GH_Path> paths = ComponentTestHelper.GetPathOutput(comp, i);
        Assert.Equal(elementIds.Count * 2, paths.Count); // elements * 2 permutations

        var cases = paths.Select(x => x.Indices[0]).ToList();
        foreach (int caseid in cases) {
          Assert.Equal(2, caseid);
        }

        var expectedPermutations = new List<int>();
        for (int j = 0; j < elementIds.Count; j++) {
          expectedPermutations.AddRange(Enumerable.Repeat(j + 1, elementIds.Count));
        }
        var permutations = paths.Select(x => x.Indices[1]).ToList();
        for (int j = 0; j < permutations.Count; j++) {
          Assert.Equal(expectedPermutations[j], permutations[j]);
        }

        var expectedIds = elementIds.ToList();
        expectedIds.AddRange(elementIds.ToList()); // add elementlist for each permutation

        var ids = paths.Select(x => x.Indices[2]).ToList();
        for (int j = 0; j < ids.Count; j++) {
          Assert.Equal(expectedIds[j], ids[j]);
        }
      }
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    public void Element2dDisplacementsMaxFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      var comp = new Element2dDisplacements();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(LengthUnit.Millimeter);
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    public void Element2dDisplacementsMaxFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Max(ExpectedCombinationCaseC2p1Values(component).Max(),
        ExpectedCombinationCaseC2p2Values(component).Max());

      // Act
      var comp = new Element2dDisplacements();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(LengthUnit.Millimeter);
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    public void Element2dDisplacementsMinFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      var comp = new Element2dDisplacements();
      comp.SetSelected(0, 9 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(LengthUnit.Millimeter);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    public void Element2dDisplacementsMinFromcombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Min(ExpectedCombinationCaseC2p1Values(component).Min(),
        ExpectedCombinationCaseC2p2Values(component).Min());

      // Act
      var comp = new Element2dDisplacements();
      comp.SetSelected(0, 9 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(LengthUnit.Millimeter);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return Element2dDisplacementsA1.XInMillimeter();
        case ResultVector6.Y: return Element2dDisplacementsA1.YInMillimeter();
        case ResultVector6.Z: return Element2dDisplacementsA1.ZInMillimeter();
        case ResultVector6.Xyz: return Element2dDisplacementsA1.XyzInMillimeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p1Values(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return Element2dDisplacementsC2p1.XInMillimeter();
        case ResultVector6.Y: return Element2dDisplacementsC2p1.YInMillimeter();
        case ResultVector6.Z: return Element2dDisplacementsC2p1.ZInMillimeter();
        case ResultVector6.Xyz: return Element2dDisplacementsC2p1.XyzInMillimeter();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p2Values(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return Element2dDisplacementsC2p2.XInMillimeter();
        case ResultVector6.Y: return Element2dDisplacementsC2p2.YInMillimeter();
        case ResultVector6.Z: return Element2dDisplacementsC2p2.ZInMillimeter();
        case ResultVector6.Xyz: return Element2dDisplacementsC2p2.XyzInMillimeter();
      }

      throw new NotImplementedException();
    }
  }
}
