using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaNode"/> can be used in Grasshopper.
  /// </summary>
  public class GsaNodeGoo : GH_OasysGeometricGoo<GsaNode>, IGH_PreviewData
  {
    public static string Name => "Node";
    public static string NickName => "No";
    public static string Description => "GSA Node";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaNodeGoo(GsaNode item) : base(item)
    {
    }

    public override IGH_GeometricGoo Duplicate() => new GsaNodeGoo(this.Value);
    public override GeometryBase GetGeometry()
    {
      if (Value == null) { return null; }
      if (Value.Point == null) { return null; }
      Point3d pt1 = Value.Point;
      pt1.Z += OasysGH.Units.DefaultUnits.Tolerance.As(OasysGH.Units.DefaultUnits.LengthUnitGeometry) / 2;
      Point3d pt2 = Value.Point;
      pt2.Z += OasysGH.Units.DefaultUnits.Tolerance.As(OasysGH.Units.DefaultUnits.LengthUnitGeometry) / -2;
      Line ln = new Line(pt1, pt2);
      return new LineCurve(ln);
    }

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of GsaNode into some other type Q.            
      if (base.CastTo<Q>(ref target))
        return true;

      //Cast to Point3d
      if (typeof(Q).IsAssignableFrom(typeof(Point3d)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)new Point3d(Value.Point);
        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(GH_Point)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)new GH_Point(Value.Point);
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(Point)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)new Point(Value.Point);
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
      {
        if (Value == null)
          target = default;
        else
        {
          GH_Integer ghint = new GH_Integer();
          if (GH_Convert.ToGHInteger(Value.Id, GH_Conversion.Both, ref ghint))
            target = (Q)(object)ghint;
          else
            target = default;
        }
        return true;
      }

      target = default;
      return false;
    }
    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data 
      // into GsaNode.
      if (source == null) { return false; }

      if (base.CastFrom(source))
        return true;

      if (typeof(Node).IsAssignableFrom(source.GetType()))
      {
        Value = new GsaNode();
        Value.ApiNode = (Node)source;
        return true;
      }

      Point3d pt = new Point3d();
      if (GH_Convert.ToPoint3d(source, ref pt, GH_Conversion.Both))
      {
        Value = new GsaNode(pt);
        return true;
      }

      return false;
    }
    #endregion

    #region transformation methods
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      return new GsaNodeGoo(Value.Transform(xform));
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      return new GsaNodeGoo(Value.Morph(xmorph));
    }
    #endregion

    #region drawing methods
    public override void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      //No meshes are drawn.   
    }
    public override void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }

      if (Value.Point.IsValid)
      {
        // draw the point
        if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
        {
          if ((System.Drawing.Color)Value.Colour != System.Drawing.Color.FromArgb(0, 0, 0))
          {
            args.Pipeline.DrawPoint(Value.Point, Rhino.Display.PointStyle.RoundSimple, 3, (System.Drawing.Color)Value.Colour);
          }
          else
          {
            System.Drawing.Color col = Helpers.Graphics.Colours.Node;
            args.Pipeline.DrawPoint(Value.Point, Rhino.Display.PointStyle.RoundSimple, 3, col);
          }
          if (Value.previewSupportSymbol != null)
            args.Pipeline.DrawBrepShaded(Value.previewSupportSymbol, Helpers.Graphics.Colours.SupportSymbol);
          if (Value.previewText != null)
            args.Pipeline.Draw3dText(Value.previewText, Helpers.Graphics.Colours.Support);
        }
        else
        {
          args.Pipeline.DrawPoint(Value.Point, Rhino.Display.PointStyle.RoundControlPoint, 3, Helpers.Graphics.Colours.NodeSelected);
          if (Value.previewSupportSymbol != null)
            args.Pipeline.DrawBrepShaded(Value.previewSupportSymbol, Helpers.Graphics.Colours.SupportSymbolSelected);
          if (Value.previewText != null)
            args.Pipeline.Draw3dText(Value.previewText, Helpers.Graphics.Colours.NodeSelected);
        }

        // local axis
        if (Value.LocalAxis != Plane.WorldXY & Value.LocalAxis != new Plane() & Value.LocalAxis != Plane.Unset)
        {
          args.Pipeline.DrawLine(Value.previewXaxis, System.Drawing.Color.FromArgb(255, 244, 96, 96), 1);
          args.Pipeline.DrawLine(Value.previewYaxis, System.Drawing.Color.FromArgb(255, 96, 244, 96), 1);
          args.Pipeline.DrawLine(Value.previewZaxis, System.Drawing.Color.FromArgb(255, 96, 96, 234), 1);
        }
      }
    }
    #endregion
  }
}
