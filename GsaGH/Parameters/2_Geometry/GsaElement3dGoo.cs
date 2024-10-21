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
  ///   Goo wrapper class, makes sure <see cref="GsaElement3d" /> can be used in Grasshopper.
  /// </summary>
  public class GsaElement3dGoo : GH_OasysGeometricGoo<GsaElement3d>, IGH_PreviewData {
    public static string Description => "GSA 3D Element(s)";
    public static string Name => "Element 3D";
    public static string NickName => "E3D";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaElement3dGoo(GsaElement3d item) : base(item) { }

    public override bool CastTo<TQ>(ref TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Mesh))) {
        target = Value == null ? default : (TQ)(object)new GH_Mesh(Value.NgonMesh);
        return true;
      }

      target = default;
      return false;
    }

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) {
      if (Value == null || Value.DisplayMesh == null) {
        return;
      }
      args.Pipeline.DrawMeshShaded(Value.DisplayMesh,
        // this is a workaround to change colour between selected and not
        args.Material.Diffuse == Colours.EntityIsNotSelected
          ? Colours.Element3dFace : Colours.Element2dFaceSelected);
    }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value == null || CentralSettings.PreviewMeshEdges == false || Value.NgonMesh == null) {
        return;
      }

      if (args.Color
        == Color.FromArgb(255, 150, 0,
          0)) // this is a workaround to change colour between selected and not
      {
        args.Pipeline.DrawMeshWires(Value.DisplayMesh, Colours.Element2dEdge, 1);
      } else {
        args.Pipeline.DrawMeshWires(Value.DisplayMesh, Colours.Element2dEdgeSelected, 2);
      }
    }

    public override IGH_GeometricGoo Duplicate() {
      return new GsaElement3dGoo(Value);
    }

    public override GeometryBase GetGeometry() {
      return Value == null ? null : (GeometryBase)Value.DisplayMesh;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      var elem = new GsaElement3d(Value) {
        Ids = new List<int>(new int[Value.NgonMesh.Faces.Count]),
      };
      elem.Topology?.Morph(xmorph);
      Mesh m = Value.NgonMesh.DuplicateMesh();
      xmorph.Morph(m);
      elem.NgonMesh = m;
      return new GsaElement3dGoo(elem);
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      var xpts = new Point3dList(Value.Topology);
      xpts.Transform(xform);
      var elem = new GsaElement3d(Value) {
        Ids = new List<int>(new int[Value.NgonMesh.Faces.Count]),
        Topology = xpts
      };
      Mesh m = Value.NgonMesh.DuplicateMesh();
      m.Transform(xform);
      elem.NgonMesh = m;
      return new GsaElement3dGoo(elem);
    }
  }
}
