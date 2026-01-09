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
  public class Element2dStressesTests {
    private static readonly string ElementList = "420 430 440 445";

    [Fact]
    public void InvalidInputErrorTests() {
      var comp = new Element2dStresses();
      ComponentTestHelper.SetInput(comp, "not a result");
      comp.Params.Output[0].CollectData();
      Assert.True((int)comp.RuntimeMessageLevel >= 10);
    }

    [Theory]
    [InlineData(Layer2d.Top)]
    [InlineData(Layer2d.Middle)]
    [InlineData(Layer2d.Bottom)]
    public void Element2dStressesElement2dIdsFromAnalysisCaseTest(Layer2d layer) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      var comp = new Element2dStresses();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, (int)layer, 2);

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

    [Theory]
    [InlineData(Layer2d.Top)]
    [InlineData(Layer2d.Middle)]
    [InlineData(Layer2d.Bottom)]
    public void Element2dStressesElement2dIdsFromCombinationCaseTest(Layer2d layer) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      var comp = new Element2dStresses();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, (int)layer, 2);

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
      var comp = new Element2dStresses();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, (int)layer, 2);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(PressureUnit.Megapascal);
      Assert.Equal(expected, max, DoubleComparer.Default);
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
      var comp = new Element2dStresses();
      comp.SetSelected(0, 1 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, (int)layer, 2);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Max in set
      double max = output.Max().As(PressureUnit.Megapascal);
      Assert.Equal(expected, max, DoubleComparer.Default);
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
      var comp = new Element2dStresses();
      comp.SetSelected(0, 7 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, (int)layer, 2);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(PressureUnit.Megapascal);
      Assert.Equal(expected, min, DoubleComparer.Default);
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
      var comp = new Element2dStresses();
      comp.SetSelected(0, 7 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);
      ComponentTestHelper.SetInput(comp, (int)layer, 2);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, (int)component);

      // Assert Min in set
      double min = output.Min().As(PressureUnit.Megapascal);
      Assert.Equal(expected, min, DoubleComparer.Default);
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
