using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters;

using OasysGH.UI;

using Xunit;

namespace GsaGHTests.Components.Display {
  [Collection("GrasshopperFixture collection")]
  public class Contour2dResultsTests {
    [Fact]
    public void CombinationCaseWithMultiplePermutationsMessageTests() {
      var caseResult = (GsaResult)GsaResultTests.CombinationCaseResult(GsaFile.SteelDesignComplex, 2, new List<int>() { 1, 2, 3, });

      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, new GsaResultGoo(caseResult));
      comp.Params.Output[0].CollectData();
      IList<string> messages = comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark);

      Assert.Single(messages);
      Assert.Equal("Combination Case 2 contains 3 permutations which have been enveloped using Absolute method.\r\nChange the enveloping method by right-clicking the component.", messages[0]);
    }

    [Fact]
    public void EmptyDisplacementsClearPreviousContourAndReportUnavailable() {
      var resultGoo = GsaResultTests.NodeAndElement2dAnalysisCaseResultsMother();
      var result = (GsaResult)resultGoo.Value;
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, resultGoo);
      Assert.NotNull(ComponentTestHelper.GetOutput(comp));

      foreach (int id in result.ElementIds("All", 2)) {
        result.Element2dDisplacements.Cache[id] = Entity2dResultsFactory.CreateDisplacements(
          new ReadOnlyCollection<Double6>(new List<Double6>()));
      }

      ComponentTestHelper.ComputeOutput(comp);
      Assert.Contains("Result is not available for the selected component",
        comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
      Assert.Null(ComponentTestHelper.GetOutput(comp));
      Assert.Equal(0, comp.Params.Output[1].VolatileDataCount);
      Assert.Equal(0, comp.Params.Output[2].VolatileDataCount);
    }

    [Fact]
    public void OneElementWithoutDisplacementsRejectsPartialContour() {
      var resultGoo = GsaResultTests.NodeAndElement2dAnalysisCaseResultsMother();
      var result = (GsaResult)resultGoo.Value;
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, resultGoo);
      Assert.NotNull(ComponentTestHelper.GetOutput(comp));
      var ids = result.ElementIds("All", 2);
      Assert.True(ids.Count > 1);
      result.Element2dDisplacements.Cache[ids[0]] = Entity2dResultsFactory.CreateDisplacements(
        new ReadOnlyCollection<Double6>(new List<Double6>()));

      ComponentTestHelper.ComputeOutput(comp);
      Assert.Contains("Result is not available for the selected component",
        comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
      Assert.Null(ComponentTestHelper.GetOutput(comp));
    }

    [Fact]
    public void UnanalysedModelClearsContourAndLegend() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dAnalysisCaseResultsMother());
      Assert.NotNull(ComponentTestHelper.GetOutput(comp));
      var field = typeof(Contour2dResults).GetField("_gradients", BindingFlags.Instance | BindingFlags.NonPublic);
      var gradients = (List<(int startY, int endY, Color gradientColor)>)field.GetValue(comp);
      Assert.NotEmpty(gradients);

      var apiModel = new GsaAPI.Model();
      int n1 = apiModel.AddNode(new Node());
      var node2 = new Node();
      node2.Position.X = 1;
      int n2 = apiModel.AddNode(node2);
      var node3 = new Node();
      node3.Position.Y = 1;
      int n3 = apiModel.AddNode(node3);
      apiModel.AddElement(new Element {
        Type = ElementType.TRI3,
        Topology = new ReadOnlyCollection<int>(new[] { n1, n2, n3 }),
      });
      var getResult = new GetResult();
      ComponentTestHelper.SetInput(getResult, new GsaModelGoo(new GsaModel(apiModel)));
      comp.Params.Input[0].RemoveAllSources();
      comp.Params.Input[0].AddSource(getResult.Params.Output[0]);
      ComponentTestHelper.ComputeOutput(comp);

      Assert.Contains("No Analysis Case Results exist in Model", getResult.RuntimeMessages(GH_RuntimeMessageLevel.Error));
      Assert.All(comp.Params.Output, output => Assert.Equal(0, output.VolatileDataCount));
      Assert.Empty((List<(int startY, int endY, Color gradientColor)>)field.GetValue(comp));
    }

    [Fact]
    public void SelectionWithout2dElementsReportsNo2dElements() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement1dAnalysisCaseResultsMother());
      ComponentTestHelper.ComputeOutput(comp);

      Assert.Contains("No 2D elements found in list 'All'",
        comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
      Assert.Null(ComponentTestHelper.GetOutput(comp));
    }

    [Fact]
    public void EmptyApiDisplacementsProduceEmptyResult() {
      var empty = new ReadOnlyCollection<Double6>(new List<Double6>());
      Assert.Empty(Entity2dResultsFactory.CreateDisplacements(empty));
      var zero = new Double6(0, 0, 0, 0, 0, 0);
      var valid = new ReadOnlyCollection<Double6>(Enumerable.Repeat(zero, 5).ToList());
      var permutations = new ReadOnlyCollection<ReadOnlyCollection<Double6>>(
        new List<ReadOnlyCollection<Double6>> { empty, valid });
      var converted = Entity2dResultsFactory.CreateDisplacements(permutations);
      Assert.Null(converted[0]);
      Assert.NotNull(converted[1]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EmptySelectedPermutationReportsUnavailable(bool envelope) {
      var resultGoo = GsaResultTests.NodeAndElement2dCombinationResultsMother();
      var result = (GsaResult)resultGoo.Value;
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, resultGoo);
      Assert.NotNull(ComponentTestHelper.GetOutput(comp));
      result.SelectedPermutationIds = envelope ? new List<int> { 1, 2 } : new List<int> { 2 };
      foreach (int id in result.ElementIds("All", 2)) {
        int count = result.Element2dDisplacements.Cache[id][0].Results().Count;
        var zero = new Double6(0, 0, 0, 0, 0, 0);
        var valid = new ReadOnlyCollection<Double6>(Enumerable.Repeat(zero, count).ToList());
        var empty = new ReadOnlyCollection<Double6>(new List<Double6>());
        result.Element2dDisplacements.Cache[id] = Entity2dResultsFactory.CreateDisplacements(
          new ReadOnlyCollection<ReadOnlyCollection<Double6>>(new List<ReadOnlyCollection<Double6>> { valid, empty }));
      }

      ComponentTestHelper.ComputeOutput(comp);
      Assert.Contains("Result is not available for the selected component",
        comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Null(ComponentTestHelper.GetOutput(comp));
    }

    [Fact]
    public void ZeroDisplacementsProduceContour() {
      var resultGoo = GsaResultTests.NodeAndElement2dAnalysisCaseResultsMother();
      var result = (GsaResult)resultGoo.Value;
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, resultGoo);
      Assert.NotNull(ComponentTestHelper.GetOutput(comp));
      foreach (int id in result.ElementIds("All", 2)) {
        int count = result.Element2dDisplacements.Cache[id][0].Results().Count;
        var zeros = new ReadOnlyCollection<Double6>(
          Enumerable.Range(0, count).Select(_ => new Double6(0, 0, 0, 0, 0, 0)).ToList());
        result.Element2dDisplacements.Cache[id] = Entity2dResultsFactory.CreateDisplacements(zeros);
      }

      ComponentTestHelper.ComputeOutput(comp);
      Assert.NotNull(ComponentTestHelper.GetOutput(comp));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void FootfallForCombinationCaseReportsUnavailable() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      Assert.NotNull(ComponentTestHelper.GetOutput(comp));
      comp.SetSelected(0, 3);
      ComponentTestHelper.ComputeOutput(comp);
      Assert.Contains("Result is not available for the selected component",
        comp.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
      Assert.Null(ComponentTestHelper.GetOutput(comp));
    }

    [Fact]
    public void DefaultDropSelectionsTest() {
      var comp = new Contour2dResults();
      Assert.Equal("Displacement", comp.SelectedItems[0]);
      Assert.Equal("Resolved |U|", comp.SelectedItems[1]);

      comp.SetSelected(0, 1);
      Assert.Equal("Force", comp.SelectedItems[0]);
      Assert.Equal("Force Nx", comp.SelectedItems[1]);

      comp.SetSelected(0, 2);
      Assert.Equal("Stress", comp.SelectedItems[0]);
      Assert.Equal("Stress xx", comp.SelectedItems[1]);
      Assert.Equal("Middle", comp.SelectedItems[2]);

      comp.SetSelected(0, 3);
      Assert.Equal("Footfall", comp.SelectedItems[0]);
      Assert.Equal("Resonant", comp.SelectedItems[1]);

      comp.SetSelected(0, 0);
      Assert.Equal("Displacement", comp.SelectedItems[0]);
      Assert.Equal("Resolved |U|", comp.SelectedItems[1]);
    }

    [Fact]
    public void SetMaxMinTest() {
      var comp = new Contour2dResults();
      comp.SetMaxMin(100, 10);
      Assert.NotNull((DropDownSliderComponentAttributes)comp.Attributes);
    }

    [Fact]
    public void GetGradientTest() {
      var comp = new Contour2dResults();
      GH_GradientControl gradient = comp.CreateGradient(new GH_Document());
      Assert.NotNull(gradient);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 0);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 3);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 4);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 5);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 6);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 7);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 8);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 9);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 2, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 4);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 5);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 2, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 4);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 5);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 2, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 2);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 4);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 5);

      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 0);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresFootfallTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dFootfallResultsMother());
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 3);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 0);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 1, 1);
    }

    [Fact]
    public void ShowLegendTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      comp.ShowLegend(null, null);
    }

    [Fact]
    public void UpdateForceTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      comp.UpdateForce("kN/m");
    }

    [Fact]
    public void UpdateLengthTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      comp.UpdateLength("mm");
    }

    [Fact]
    public void UpdateModelTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      comp.UpdateModel("mm");
    }

    [Fact]
    public void UpdateMomentTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      comp.UpdateMoment("kN");
    }

    [Fact]
    public void UpdateStressTest() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.NodeAndElement2dCombinationResultsMother());
      comp.UpdateStress("MPa");
    }

    private void SetSelectedDrawViewportMeshesAndWiresTest(Contour2dResults comp, int i, int j) {
      comp.SetSelected(i, j);
      var resultsGoo = (MeshResultGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(resultsGoo);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void ContourResultsComponentShouldNotThrowException() {
      var comp = new Contour2dResults();
      ComponentTestHelper.SetInput(comp, GsaResultTests.FabricModelResultsMother());

      //displacement
      ShouldNotThrowException(comp, 0, 0);

      ShouldNotThrowException(comp, 1, 0);
      ShouldNotThrowException(comp, 1, 1);
      ShouldNotThrowException(comp, 1, 2);
      ShouldNotThrowException(comp, 1, 3);

      //forces
      ShouldNotThrowException(comp, 0, 1);

      ShouldNotThrowException(comp, 1, 0);
      ShouldNotThrowException(comp, 1, 1);
      ShouldNotThrowException(comp, 1, 2);
      ShouldNotThrowException(comp, 1, 3);
      ShouldNotThrowException(comp, 1, 4);
      ShouldNotThrowException(comp, 1, 5);
      ShouldNotThrowException(comp, 1, 6);
      ShouldNotThrowException(comp, 1, 7);
      ShouldNotThrowException(comp, 1, 8);
      ShouldNotThrowException(comp, 1, 9);

      //strress
      ShouldNotThrowException(comp, 0, 2);
      ShouldNotThrowException(comp, 2, 0);

      ShouldNotThrowException(comp, 1, 0);
      ShouldNotThrowException(comp, 1, 1);
      ShouldNotThrowException(comp, 1, 2);
      ShouldNotThrowException(comp, 1, 3);
      ShouldNotThrowException(comp, 1, 4);
      ShouldNotThrowException(comp, 1, 5);

      ShouldNotThrowException(comp, 2, 1);

      ShouldNotThrowException(comp, 1, 0);
      ShouldNotThrowException(comp, 1, 1);
      ShouldNotThrowException(comp, 1, 2);
      ShouldNotThrowException(comp, 1, 3);
      ShouldNotThrowException(comp, 1, 4);
      ShouldNotThrowException(comp, 1, 5);

      ShouldNotThrowException(comp, 2, 2);

      ShouldNotThrowException(comp, 1, 0);
      ShouldNotThrowException(comp, 1, 1);
      ShouldNotThrowException(comp, 1, 2);
      ShouldNotThrowException(comp, 1, 3);
      ShouldNotThrowException(comp, 1, 4);
      ShouldNotThrowException(comp, 1, 5);

      //footfall
      ShouldNotThrowException(comp, 0, 3);
      ShouldNotThrowException(comp, 1, 0);
      ShouldNotThrowException(comp, 1, 1);
    }

    private void ShouldNotThrowException(Contour2dResults component, int i, int j) {
      component.SetSelected(i, j);
      var resultsGoo = (MeshResultGoo)ComponentTestHelper.GetOutput(component);
      if(resultsGoo == null) {
        Assert.Single(component.RuntimeMessages(GH_RuntimeMessageLevel.Warning));
        Assert.Contains("Result is not available for the selected component", component.RuntimeMessages(GH_RuntimeMessageLevel.Warning)[0]);
      }
    }

  }
}
