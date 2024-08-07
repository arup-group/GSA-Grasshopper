using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helper;
using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Components.Display {
  [Collection("GrasshopperFixture collection")]
  public class LoadDiagramsTests {
    public static LoadDiagrams LoadDiagramNodeAndElement1dMother() {
      var open = new OpenModel();
      open.CreateAttributes();
      string file = GsaFile.SteelDesignComplex;
      ComponentTestHelper.SetInput(open, file);
      var model = (GsaModelGoo)ComponentTestHelper.GetOutput(open);
      var comp = new LoadDiagrams();
      ComponentTestHelper.SetInput(comp, model);
      return comp;
    }

    [Fact]
    public void DefaultDropSelectionsTest() {
      var comp = new LoadDiagrams();
      Assert.Equal("All", comp._selectedItems[1]);
      Assert.Equal(2, comp._selectedItems.Count);

      comp.SetSelected(1, 1);
      Assert.Equal("Nodal", comp._selectedItems[1]);
      Assert.Equal("All", comp._selectedItems[2]);

      comp.SetSelected(1, 2);
      Assert.Equal("Beam", comp._selectedItems[1]);
      Assert.Equal("All", comp._selectedItems[2]);

      comp.SetSelected(1, 3);
      Assert.Equal("2D", comp._selectedItems[1]);
      Assert.Equal("All", comp._selectedItems[2]);

      comp.SetSelected(1, 4);
      Assert.Equal("3D", comp._selectedItems[1]);
      Assert.Equal("All", comp._selectedItems[2]);

      comp.SetSelected(1, 5);
      Assert.Equal("Grid", comp._selectedItems[1]);
      Assert.Equal("All", comp._selectedItems[2]);

      comp.SetSelected(1, 0);
      Assert.Equal("All", comp._selectedItems[1]);
      Assert.Equal(2, comp._selectedItems.Count);
    }

    [Fact]
    public void CaseIdDropdownPopulationTest() {
      LoadDiagrams comp = LoadDiagramNodeAndElement1dMother();
      var diagramGoo = (GsaDiagramGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("L1", comp._selectedItems[0]);
      Assert.Equal(19, comp._dropDownItems[0].Count);
    }

    [Fact]
    public void SetSelectedDrawViewportMeshesAndWiresTests() {
      LoadDiagrams comp = LoadDiagramNodeAndElement1dMother();
      var diagramGoo = (GsaDiagramGoo)ComponentTestHelper.GetOutput(comp);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 1);
      SetSelectedDrawViewportMeshesAndWiresTest(comp, 0, 2);
    }

    private void SetSelectedDrawViewportMeshesAndWiresTest(LoadDiagrams comp, int i, int j) {
      comp.SetSelected(i, j);
      var diagramGoo = (GsaDiagramGoo)ComponentTestHelper.GetOutput(comp);
      Assert.NotNull(diagramGoo);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void UpdateForceTest() {
      var comp = new LoadDiagrams();
      comp.UpdateForce("kN");
    }

    [Fact]
    public void UpdateModelTest() {
      var comp = new LoadDiagrams();
      comp.UpdateModel("mm");
    }
  }
}
