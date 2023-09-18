using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;
using Rhino.Display;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GH_OasysGeometryGooTests {
    public static GH_PreviewMeshArgs CreateNotSelectedPreviewMeshArgs() {
      var rvp = new RhinoViewport();
      var rhDoc = Rhino.RhinoDoc.CreateHeadless(null);
      rhDoc.Views.DefaultViewLayout();
      DisplayPipeline dpl = rhDoc.Views.ActiveView.DisplayPipeline;
      var doc = new GH_Document();
      MeshingParameters mp = doc.PreviewCurrentMeshParameters();
      var notSelectedMaterial = new DisplayMaterial {
        Diffuse = Color.FromArgb(255, 150, 0, 0),
        Emission = Color.FromArgb(50, 190, 190, 190),
        Transparency = 0.1,
      };

      return new GH_PreviewMeshArgs(rvp, dpl, notSelectedMaterial, mp);
    }


    [Fact]
    public void GsaElement1dGooPreviewTest() {
      var comp = (Section3dPreviewComponent)CreateElement1dTests.ComponentMother();
      comp.Preview3dSection = true;
      var output = (GsaElement1dGoo)ComponentTestHelper.GetOutput(comp);
      GH_PreviewMeshArgs args = CreateNotSelectedPreviewMeshArgs();
      output.DrawViewportMeshes(args);
      Assert.True(true);
    }
  }
}
