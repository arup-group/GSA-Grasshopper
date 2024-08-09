using System.Collections.Generic;
using System.Drawing;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;
using GsaGH.Helpers.Graphics;

using OasysGH;
using OasysGH.Parameters;

using Rhino.Collections;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaElement2d" /> can be used in Grasshopper.
  /// </summary>
  public class GsaElement2dGoo : GH_OasysGeometricGoo<GsaElement2d>, IGH_PreviewData {
    public static string Description => "GSA 2D Element(s)";
    public static string Name => "Element 2D";
    public static string NickName => "E2D";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaElement2dGoo(GsaElement2d item) : base(item) { }

    public override bool CastTo<TQ>(ref TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Mesh))) {
        target = Value == null ? default : (TQ)(object)new GH_Mesh(Value.Mesh);
        return true;
      }

      target = default;
      return false;
    }

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) {
      if (Value != null && Value.Mesh != null) {
        args.Pipeline.DrawMeshShaded(Value?.Mesh,
        args.Material.Diffuse == Color.FromArgb(255, 150, 0, 0)
          ? Colours.Element2dFace : Colours.Element2dFaceSelected);
      }

      Value?.Section3dPreview?.DrawViewportMeshes(args);
    }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value == null || CentralSettings.PreviewMeshEdges == false || Value.Mesh == null) {
        return;
      }

      Value.Section3dPreview?.DrawViewportWires(args);

      if (args.Color == Color.FromArgb(255, 150, 0, 0)) {
        args.Pipeline.DrawMeshWires(Value.Mesh, Colours.Element2dEdge, 1);
      } else {
        args.Pipeline.DrawMeshWires(Value.Mesh, Colours.Element2dEdgeSelected, 2);
      }

      if (Value.Section3dPreview != null) {
        if (args.Color == Color.FromArgb(255, 150, 0, 0)) {
          args.Pipeline.DrawLines(Value.Section3dPreview.Outlines, Colours.Element2dEdge, 1);
        } else {
          args.Pipeline.DrawLines(Value.Section3dPreview.Outlines, Colours.Element2dEdgeSelected, 2);
        }
      }
    }

    public override IGH_GeometricGoo Duplicate() {
      return new GsaElement2dGoo(Value);
    }

    public override GeometryBase GetGeometry() {
      if (Value == null) {
        return null;
      }

      if (Value.Section3dPreview != null && Value.Section3dPreview.Mesh != null) {
        return Value.Section3dPreview.Mesh;
      }

      return Value.Mesh;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      var elem = new GsaElement2d(Value) {
        Ids = new List<int>(new int[Value.Mesh.Faces.Count]),
      };
      elem.Topology?.Morph(xmorph);
      Mesh m = Value.Mesh.DuplicateMesh();
      xmorph.Morph(m);
      elem.Mesh = m;
      elem.Section3dPreview?.Morph(xmorph);
      return new GsaElement2dGoo(elem);
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      var xpts = new Point3dList(Value.Topology);
      xpts.Transform(xform);
      var elem = new GsaElement2d(Value) {
        Ids = new List<int>(new int[Value.Mesh.Faces.Count]),
        Topology = xpts
      };
      Mesh m = Value.Mesh.DuplicateMesh();
      m.Transform(xform);
      elem.Mesh = m;
      elem.Section3dPreview?.Transform(xform);
      return new GsaElement2dGoo(elem);
    }
  }
}
