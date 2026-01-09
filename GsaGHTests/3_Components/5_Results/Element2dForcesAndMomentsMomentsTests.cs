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
  public class Element2dForcesAndMomentsMomentsTests {
    private static readonly string ElementList = "420 430 440 445";

    [Fact]
    public void Element2dMomentsElement2dIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      var comp = new Element2dForcesAndMoments();
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
    public void Element2dIMomentElement2dIdsFromCombinationCaseTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);

      // Act
      ReadOnlyCollection<int> elementIds = result.ElementIds(ElementList, 2);
      var comp = new Element2dForcesAndMoments();
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
    [InlineData(ResultTensor2AroundAxis.Mx)]
    [InlineData(ResultTensor2AroundAxis.My)]
    [InlineData(ResultTensor2AroundAxis.Mxy)]
    [InlineData(ResultTensor2AroundAxis.WoodArmerX)]
    [InlineData(ResultTensor2AroundAxis.WoodArmerY)]
    public void Element2dMomentsMaxFromAnalysisCaseTest(ResultTensor2AroundAxis component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      var comp = new Element2dForcesAndMoments();
      comp.SetSelected(0, 6 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 5 + (int)component);

      // Assert Max in set
      double max = output.Max().As(ForceUnit.Kilonewton);
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultTensor2AroundAxis.Mx)]
    [InlineData(ResultTensor2AroundAxis.My)]
    [InlineData(ResultTensor2AroundAxis.Mxy)]
    [InlineData(ResultTensor2AroundAxis.WoodArmerX)]
    [InlineData(ResultTensor2AroundAxis.WoodArmerY)]
    public void Element2dMomentMaxFromCombinationCaseTest(ResultTensor2AroundAxis component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Max(ExpectedCombinationCaseC2p1Values(component).Max(),
        ExpectedCombinationCaseC2p2Values(component).Max());

      // Act
      var comp = new Element2dForcesAndMoments();
      comp.SetSelected(0, 6 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 5 + (int)component);

      // Assert Max in set
      double max = output.Max().As(ForceUnit.Kilonewton);
      Assert.Equal(expected, max, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultTensor2AroundAxis.Mx)]
    [InlineData(ResultTensor2AroundAxis.My)]
    [InlineData(ResultTensor2AroundAxis.Mxy)]
    [InlineData(ResultTensor2AroundAxis.WoodArmerX)]
    [InlineData(ResultTensor2AroundAxis.WoodArmerY)]
    public void Element2dMomentsMinFromAnalysisCaseTest(ResultTensor2AroundAxis component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.Element2dSimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      var comp = new Element2dForcesAndMoments();
      comp.SetSelected(0, 16 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 5 + (int)component);

      // Assert Min in set
      double min = output.Min().As(ForceUnit.Kilonewton);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    [Theory]
    [InlineData(ResultTensor2AroundAxis.Mx)]
    [InlineData(ResultTensor2AroundAxis.My)]
    [InlineData(ResultTensor2AroundAxis.Mxy)]
    [InlineData(ResultTensor2AroundAxis.WoodArmerX)]
    [InlineData(ResultTensor2AroundAxis.WoodArmerY)]
    public void Element2dMomentsMinFromcombinationCaseTest(ResultTensor2AroundAxis component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.Element2dSimple, 2);
      double expected = Math.Min(ExpectedCombinationCaseC2p1Values(component).Min(),
        ExpectedCombinationCaseC2p2Values(component).Min());

      // Act
      var comp = new Element2dForcesAndMoments();
      comp.SetSelected(0, 16 + (int)component);
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(result));
      ComponentTestHelper.SetInput(comp, ElementList, 1);

      List<IQuantity> output = ComponentTestHelper.GetResultOutput(comp, 5 + (int)component);

      // Assert Min in set
      double min = output.Min().As(ForceUnit.Kilonewton);
      Assert.Equal(expected, min, DoubleComparer.Default);
    }

    private List<double> ExpectedAnalysisCaseValues(ResultTensor2AroundAxis component) {
      switch (component) {
        case ResultTensor2AroundAxis.Mx: return Element2dMomentsA1.MxInKiloNewton();
        case ResultTensor2AroundAxis.My: return Element2dMomentsA1.MyInKiloNewton();
        case ResultTensor2AroundAxis.Mxy: return Element2dMomentsA1.MxyInKiloNewton();
        case ResultTensor2AroundAxis.WoodArmerX: return Element2dMomentsA1.WoodArmerXInKiloNewton();
        case ResultTensor2AroundAxis.WoodArmerY: return Element2dMomentsA1.WoodArmerYInKiloNewton();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p1Values(ResultTensor2AroundAxis component) {
      switch (component) {
        case ResultTensor2AroundAxis.Mx: return Element2dMomentsC2p1.MxInKiloNewton();
        case ResultTensor2AroundAxis.My: return Element2dMomentsC2p1.MyInKiloNewton();
        case ResultTensor2AroundAxis.Mxy: return Element2dMomentsC2p1.MxyInKiloNewton();
        case ResultTensor2AroundAxis.WoodArmerX: return Element2dMomentsC2p1.WoodArmerXInKiloNewton();
        case ResultTensor2AroundAxis.WoodArmerY: return Element2dMomentsC2p1.WoodArmerYInKiloNewton();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC2p2Values(ResultTensor2AroundAxis component) {
      switch (component) {
        case ResultTensor2AroundAxis.Mx: return Element2dMomentsC2p2.MxInKiloNewton();
        case ResultTensor2AroundAxis.My: return Element2dMomentsC2p2.MyInKiloNewton();
        case ResultTensor2AroundAxis.Mxy: return Element2dMomentsC2p2.MxyInKiloNewton();
        case ResultTensor2AroundAxis.WoodArmerX: return Element2dMomentsC2p2.WoodArmerXInKiloNewton();
        case ResultTensor2AroundAxis.WoodArmerY: return Element2dMomentsC2p2.WoodArmerYInKiloNewton();
      }

      throw new NotImplementedException();
    }
  }
}
