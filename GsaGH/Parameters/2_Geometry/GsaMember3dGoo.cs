using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.Graphics;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaMember3d" /> can be used in Grasshopper.
  /// </summary>
  public class GsaMember3dGoo : GH_OasysGeometricGoo<GsaMember3d>,
    IGH_PreviewData {
    public GsaMember3dGoo(GsaMember3d item) : base(item) { }

    internal GsaMember3dGoo(GsaMember3d item, bool duplicate) : base(null)
      => Value = duplicate
        ? item.Duplicate()
        : item;

    public static string Name => "Member3D";
    public static string NickName => "M3D";
    public static string Description => "GSA 3D Member";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public override IGH_GeometricGoo Duplicate() => new GsaMember3dGoo(Value);
    public override GeometryBase GetGeometry() => Value.SolidMesh;

    #region casting methods

    public override bool CastTo<TQ>(ref TQ target) {
      // This function is called when Grasshopper needs to convert this 
      // instance of GsaMember into some other type Q.            
      if (base.CastTo(ref target))
        return true;

      if (typeof(TQ).IsAssignableFrom(typeof(Mesh))) {
        target = Value == null
          ? default
          : (TQ)(object)Value.SolidMesh;
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Mesh))) {
        if (Value == null)
          target = default;
        else {
          target = (TQ)(object)new GH_Mesh(Value.SolidMesh);
          if (Value.SolidMesh == null)
            return false;
        }

        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        if (Value == null)
          target = default;
        else {
          var ghint = new GH_Integer();
          target = GH_Convert.ToGHInteger(Value.Id, GH_Conversion.Both, ref ghint)
            ? (TQ)(object)ghint
            : default;
        }

        return true;
      }

      target = default;
      return false;
    }

    public override bool CastFrom(object source) {
      // This function is called when Grasshopper needs to convert other data 
      // into GsaMember.
      if (source == null)
        return false;

      if (base.CastFrom(source))
        return true;

      var brep = new Brep();
      if (GH_Convert.ToBrep(source, ref brep, GH_Conversion.Both)) {
        var member = new GsaMember3d(brep);
        Value = member;
        return true;
      }

      var mesh = new Mesh();

      if (!GH_Convert.ToMesh(source, ref mesh, GH_Conversion.Both))
        return false;
      else {
        var member = new GsaMember3d(mesh);
        Value = member;
        return true;
      }
    }

    #endregion

    #region transformation methods

    public override IGH_GeometricGoo Transform(Transform xform)
      => new GsaMember3dGoo(Value.Transform(xform));

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
      => new GsaMember3dGoo(Value.Morph(xmorph));

    #endregion

    #region drawing methods

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) {
      if (Value?.SolidMesh == null)
        return;
      if (!Value.IsDummy) {
        args.Pipeline.DrawMeshShaded(Value.SolidMesh,
          args.Material.Diffuse == Color.FromArgb(255, 150, 0, 0) // this is a workaround to change colour between selected and not
            ? Colours.Element2dFace
            : Colours.Element2dFaceSelected); //UI.Colour.Member2dFace
      }
      else
        args.Pipeline.DrawMeshShaded(Value.SolidMesh, Colours.Dummy2D);
    }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value?.SolidMesh == null)
        return;
      if (Value.IsDummy)
        foreach (Line line in Value._previewEdgeLines)
          args.Pipeline.DrawDottedLine(line,
            args.Color == Color.FromArgb(255, 150, 0, 0) // this is a workaround to change colour between selected and not
              ? Colours.Dummy1D
              : Colours.Member2dEdgeSelected);
      else {
        foreach (Line line in Value._previewEdgeLines)
          if (args.Color == Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
          {
            if (Value.Colour != Color.FromArgb(0, 0, 0))
              args.Pipeline.DrawLine(line, (Color)Value.Colour, 2);
            else {
              Color col = Colours.Member2dEdge;
              args.Pipeline.DrawLine(line, col, 2);
            }
          }
          else
            args.Pipeline.DrawLine(line, Colours.Element2dEdgeSelected, 2);

        foreach (Polyline line in Value._previewHiddenLines)
          args.Pipeline.DrawDottedPolyline(line, Colours.Dummy1D, false);
      }

      foreach (Point3d point3d in Value._previewPts)
        if (args.Color == Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
          args.Pipeline.DrawPoint(point3d, PointStyle.RoundSimple, 2, (Value.IsDummy)
              ? Colours.Dummy1D
              : Colours.Member1dNode);
        else
          args.Pipeline.DrawPoint(point3d,
            PointStyle.RoundControlPoint,
            3,
            Colours.Member1dNodeSelected);
    }

    #endregion
  }
}
