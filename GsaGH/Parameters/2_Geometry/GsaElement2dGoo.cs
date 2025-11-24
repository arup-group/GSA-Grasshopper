using System.Collections.Generic;

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
  ///   Goo wrapper class, makes sure <see cref="GsaElement2D" /> can be used in Grasshopper.
  /// </summary>
  public class GsaElement2dGoo : GH_OasysGeometricGoo<GsaElement2D>, IGH_PreviewData {
    public static string Description => "GSA 2D Element(s)";
    public static string Name => "Element 2D";
    public static string NickName => "E2D";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaElement2dGoo(GsaElement2D item) : base(item) { }

    public override bool CastTo<TQ>(ref TQ target) {
      if (Value != null) {
        if (Value.IsLoadPanel) {
          if (typeof(TQ).IsAssignableFrom(typeof(GH_Curve))) {
            target = (TQ)(object)new GH_Curve(Value.Curve);
            return true;
          }
        } else {
          if (typeof(TQ).IsAssignableFrom(typeof(GH_Mesh))) {
            target = (TQ)(object)new GH_Mesh(Value.Mesh);
            return true;
          }
        }
      }
      target = default;
      return false;
    }

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) {
      if (Value != null) {
        if (Value.IsLoadPanel) {
          if (Value.Curve != null) {
            Brep[] PlanerBrep = Brep.CreatePlanarBreps(Value.Curve, Rhino.RhinoDoc.ActiveDoc != null ? Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance : 0.001);
            foreach (Brep brep in PlanerBrep) {
              args.Pipeline.DrawBrepShaded(brep, args.Material.Diffuse == Colours.EntityIsNotSelected
              ? Colours.Element2dFaceLoadPanel : Colours.Element2dFaceSelectedLoadPanel);
            }
          }
        } else {
          if (Value.Mesh != null) {
            args.Pipeline.DrawMeshShaded(Value?.Mesh,
            args.Material.Diffuse == Colours.EntityIsNotSelected
              ? Colours.Element2dFace : Colours.Element2dFaceSelected);
          }
        }
        Value?.Section3dPreview?.DrawViewportMeshes(args);
      }
    }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value != null) {
        Value.Section3dPreview?.DrawViewportWires(args);
        if (Value.Section3dPreview != null) {
          args.Pipeline.DrawLines(Value.Section3dPreview.Outlines, args.Color == Colours.EntityIsNotSelected ? Colours.Element2dEdge : Colours.Element2dEdgeSelected, 1);
        }
        if (Value.IsLoadPanel) {
          if (Value.Curve != null) {
            args.Pipeline.DrawCurve(Value.Curve, args.Color == Colours.EntityIsNotSelected ? Colours.Element2dEdge : Colours.Element2dEdgeSelected, -1);
          }
        } else {
          if (CentralSettings.PreviewMeshEdges == false || Value.Mesh == null) {
            return;
          }
          args.Pipeline.DrawMeshWires(Value.Mesh, args.Color == Colours.EntityIsNotSelected ? Colours.Element2dEdge : Colours.Element2dEdgeSelected, 1);
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
      if (Value.IsLoadPanel) {
        return Value.Curve;
      } else {
        return Value.Mesh;
      }
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      var elem = new GsaElement2D(Value) {
        Ids = new List<int>(new int[Value.IsLoadPanel ? 1 : Value.Mesh.Faces.Count]),
      };
      elem.Topology?.Morph(xmorph);
      if (Value.IsLoadPanel) {
        Curve m = Value.Curve.DuplicateCurve();
        xmorph.Morph(m);
        elem.Curve = m;
      } else {
        Mesh m = Value.Mesh.DuplicateMesh();
        xmorph.Morph(m);
        elem.Mesh = m;
      }

      elem.Section3dPreview?.Morph(xmorph);
      return new GsaElement2dGoo(elem);
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      var xpts = new Point3dList(Value.Topology);
      xpts.Transform(xform);
      var elem = new GsaElement2D(Value) {
        Ids = new List<int>(new int[Value.IsLoadPanel ? 1 : Value.Mesh.Faces.Count]),
        Topology = xpts
      };

      if (Value.IsLoadPanel) {
        Curve m = Value.Curve.DuplicateCurve();
        m.Transform(xform);
        elem.Curve = m;
      } else {
        Mesh m = Value.Mesh.DuplicateMesh();
        m.Transform(xform);
        elem.Mesh = m;
      }
      elem.Section3dPreview?.Transform(xform);
      return new GsaElement2dGoo(elem);
    }
  }
}
