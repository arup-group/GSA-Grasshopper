using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Grasshopper.Kernel.Data;

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
  public class Element3dStressesTests {
    private static readonly string ElementList = "6444 6555 7000 7015";

    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new Element3dStresses();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Fact]
    public void Element3dStressesElement3dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element3dSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      var comp = new Element3dStresses();
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
    public void Element3dStressesElement3dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element3dSimple, 2);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 3);
      var comp = new Element3dStresses();
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
    [InlineData(ResultTensor3.Xx)]
    [InlineData(ResultTensor3.Yy)]
    [InlineData(ResultTensor3.Zz)]
    [InlineData(ResultTensor3.Xy)]
    [InlineData(ResultTensor3.Yz)]
    [InlineData(ResultTensor3.Zx)]
    public void Element3dStressesMaxFromAnalysisCaseTest(ResultTensor3 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element3dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      var comp = new Element3dStresses();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(PressureUnit.Megapascal);
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultTensor3.Xx)]
    [InlineData(ResultTensor3.Yy)]
    [InlineData(ResultTensor3.Zz)]
    [InlineData(ResultTensor3.Xy)]
    [InlineData(ResultTensor3.Yz)]
    [InlineData(ResultTensor3.Zx)]
    public void Element3dStressesMaxFromCombinationCaseTest(ResultTensor3 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element3dSimple, 2);
      double expected = Math.Max(
        ExpectedCombinationCaseC2p1Values(component).Max(),
        ExpectedCombinationCaseC2p2Values(component).Max());

      // Act
      var comp = new Element3dStresses();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(PressureUnit.Megapascal);
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultTensor3.Xx)]
    [InlineData(ResultTensor3.Yy)]
    [InlineData(ResultTensor3.Zz)]
    [InlineData(ResultTensor3.Xy)]
    [InlineData(ResultTensor3.Yz)]
    [InlineData(ResultTensor3.Zx)]
    public void Element3dStressesMinFromAnalysisCaseTest(ResultTensor3 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element3dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      var comp = new Element3dStresses();
      comp.SetSelected(0, 7 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(PressureUnit.Megapascal);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultTensor3.Xx)]
    [InlineData(ResultTensor3.Yy)]
    [InlineData(ResultTensor3.Zz)]
    [InlineData(ResultTensor3.Xy)]
    [InlineData(ResultTensor3.Yz)]
    [InlineData(ResultTensor3.Zx)]
    public void Element3dStressesMinFromcombinationCaseTest(ResultTensor3 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element3dSimple, 2);
      double expected = Math.Min(
        ExpectedCombinationCaseC2p1Values(component).Min(),
        ExpectedCombinationCaseC2p2Values(component).Min());

      // Act
      var comp = new Element3dStresses();
      comp.SetSelected(0, 7 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(PressureUnit.Megapascal);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    private List<double> ExpectedAnalysisCaseValues(ResultTensor3 component) {
      switch (component) {
        case ResultTensor3.Xx: return Element3dStressesA1.XxInMPa();
        case ResultTensor3.Yy: return Element3dStressesA1.YyInMPa();
        case ResultTensor3.Zz: return Element3dStressesA1.ZzInMPa();
        case ResultTensor3.Xy: return Element3dStressesA1.XyInMPa();
        case ResultTensor3.Yz: return Element3dStressesA1.YzInMPa();
        case ResultTensor3.Zx: return Element3dStressesA1.ZxInMPa();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p1Values(
      ResultTensor3 component) {
      switch (component) {
        case ResultTensor3.Xx: return Element3dStressesC2p1.XxInMPa();
        case ResultTensor3.Yy: return Element3dStressesC2p1.YyInMPa();
        case ResultTensor3.Zz: return Element3dStressesC2p1.ZzInMPa();
        case ResultTensor3.Xy: return Element3dStressesC2p1.XyInMPa();
        case ResultTensor3.Yz: return Element3dStressesC2p1.YzInMPa();
        case ResultTensor3.Zx: return Element3dStressesC2p1.ZxInMPa();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p2Values(
      ResultTensor3 component) {
      switch (component) {
        case ResultTensor3.Xx: return Element3dStressesC2p2.XxInMPa();
        case ResultTensor3.Yy: return Element3dStressesC2p2.YyInMPa();
        case ResultTensor3.Zz: return Element3dStressesC2p2.ZzInMPa();
        case ResultTensor3.Xy: return Element3dStressesC2p2.XyInMPa();
        case ResultTensor3.Yz: return Element3dStressesC2p2.YzInMPa();
        case ResultTensor3.Zx: return Element3dStressesC2p2.ZxInMPa();
      }

      throw new NotImplementedException();
    }
  }
}
