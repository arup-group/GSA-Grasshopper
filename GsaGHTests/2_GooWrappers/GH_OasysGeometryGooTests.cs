using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.Graphics;

using OasysGH.Parameters;

using Rhino.Display;
using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GH_OasysGeometryGooTests {
    public static void DrawViewportMeshesAndWiresTest<T>(GH_OasysGeometricGoo<T> geometryGoo) {
      var rhinoViewPort = new RhinoViewport();
      var rhinoDocument = Rhino.RhinoDoc.CreateHeadless(null);
      rhinoDocument.Views.DefaultViewLayout();
      DisplayPipeline displayPipeline = rhinoDocument.Views.ActiveView.DisplayPipeline;
      var grasshopperDocument = new GH_Document();
      Grasshopper.CentralSettings.PreviewMeshEdges = true;
      MeshingParameters mp = grasshopperDocument.PreviewCurrentMeshParameters();
      var notSelectedMaterial = new DisplayMaterial {
        Diffuse = Colours.EntityIsNotSelected,
        Emission = Color.FromArgb(50, 190, 190, 190),
        Transparency = 0.1,
      };
      var selectedMaterial = new DisplayMaterial {
        Diffuse = Color.FromArgb(255, 150, 0, 1),
        Emission = Color.FromArgb(50, 190, 190, 190),
        Transparency = 0.1,
      };
      var notSelectedMeshArgs = new GH_PreviewMeshArgs(
        rhinoViewPort, displayPipeline, notSelectedMaterial, mp);
      var selectedMeshArgs = new GH_PreviewMeshArgs(rhinoViewPort, displayPipeline, selectedMaterial, mp);

      var notSelectedWireArgs = new GH_PreviewWireArgs(
        rhinoViewPort, displayPipeline, Colours.EntityIsNotSelected, 3);
      var selectedWireArgs = new GH_PreviewWireArgs(
        rhinoViewPort, displayPipeline, Color.FromArgb(255, 150, 0, 1), 3);

      geometryGoo.DrawViewportMeshes(notSelectedMeshArgs);
      geometryGoo.DrawViewportMeshes(selectedMeshArgs);
      geometryGoo.DrawViewportWires(notSelectedWireArgs);
      geometryGoo.DrawViewportWires(selectedWireArgs);

      Assert.True(true);

      rhinoDocument.Dispose();
      grasshopperDocument.Dispose();
    }
  }
}
