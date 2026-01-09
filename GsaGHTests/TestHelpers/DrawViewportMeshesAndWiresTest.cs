using System;
using System.Drawing;
using System.Reflection;

using Grasshopper.Kernel;

using GsaGH.Helpers.Graphics;

using Rhino.Display;
using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Helpers {
  public partial class ComponentTestHelper {
    internal static void DrawViewportMeshesAndWiresTest(GH_Component component) {
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

      object[] parameters = {
        grasshopperDocument,
        displayPipeline,
        rhinoViewPort,
        3,
        Colours.EntityIsNotSelected,
        Color.FromArgb(255, 150, 0, 1),
        notSelectedMaterial,
        selectedMaterial,
        mp
      };

      // create GH_PreviewArgs with reflection as constructor is internal in GH
      BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
      var args = (GH_PreviewArgs)Activator.CreateInstance(
        typeof(GH_PreviewArgs), flags, null, parameters, null);

      component.Attributes.Selected = false;
      component.DrawViewportMeshes(args);
      component.DrawViewportWires(args);
      component.Attributes.Selected = true;
      component.DrawViewportMeshes(args);
      component.DrawViewportWires(args);

      Assert.True(true);

      rhinoDocument.Dispose();
      grasshopperDocument.Dispose();
    }
  }
}
