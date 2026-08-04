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
      Assert.Equal("All", comp.SelectedItems[1]);
      Assert.Equal(2, comp.SelectedItems.Count);

      comp.SetSelected(1, 1);
      Assert.Equal("Nodal", comp.SelectedItems[1]);
      Assert.Equal("All", comp.SelectedItems[2]);

      comp.SetSelected(1, 2);
      Assert.Equal("Beam", comp.SelectedItems[1]);
      Assert.Equal("All", comp.SelectedItems[2]);

      comp.SetSelected(1, 3);
      Assert.Equal("2D", comp.SelectedItems[1]);
      Assert.Equal("All", comp.SelectedItems[2]);

      comp.SetSelected(1, 4);
      Assert.Equal("3D", comp.SelectedItems[1]);
      Assert.Equal("All", comp.SelectedItems[2]);

      comp.SetSelected(1, 5);
      Assert.Equal("Grid", comp.SelectedItems[1]);
      Assert.Equal("All", comp.SelectedItems[2]);

      comp.SetSelected(1, 0);
      Assert.Equal("All", comp.SelectedItems[1]);
      Assert.Equal(2, comp.SelectedItems.Count);
    }

    [Fact]
    public void CaseIdDropdownPopulationTest() {
      LoadDiagrams comp = LoadDiagramNodeAndElement1dMother();
      var diagramGoo = (GsaDiagramGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("L1", comp.SelectedItems[0]);
      Assert.Equal(19, comp.DropDownItems[0].Count);
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
