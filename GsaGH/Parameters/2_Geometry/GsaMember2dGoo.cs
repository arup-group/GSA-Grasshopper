using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;
using OasysGH.Units;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaMember2d"/> can be used in Grasshopper.
  /// </summary>
  public class GsaMember2dGoo : GH_OasysGeometricGoo<GsaMember2d>, IGH_PreviewData
  {
    public static string Name => "Member2D";
    public static string NickName => "M2D";
    public static string Description => "GSA 2D Member";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaMember2dGoo(GsaMember2d item) : base(item)
    {
    }

    public override IGH_GeometricGoo Duplicate() => new GsaMember2dGoo(this.Value);
    public override GeometryBase GetGeometry() => this.Value.Brep;

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of GsaMember into some other type Q.            
      if (base.CastTo<Q>(ref target))
        return true;

      //Cast to Curve
      if (typeof(Q).IsAssignableFrom(typeof(Curve)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.PolyCurve.DuplicatePolyCurve();
        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)new GH_Curve(Value.PolyCurve.DuplicatePolyCurve());
          if (Value.PolyCurve == null)
            return false;
        }

        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(PolyCurve)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)Value.PolyCurve.DuplicatePolyCurve();
          if (Value.PolyCurve == null)
            return false;
        }

        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(Polyline)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)Value.PolyCurve.DuplicatePolyCurve();
          if (Value.PolyCurve == null)
            return false;
        }

        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(Line)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)Value.PolyCurve.ToPolyline(DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry), 2, 0, 0);
          if (Value.PolyCurve == null)
            return false;
        }

        return true;
      }

      //Cast to Brep
      if (typeof(Q).IsAssignableFrom(typeof(Brep)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)Value.Brep.DuplicateBrep();
          if (Value.Brep == null)
            return false;
        }

        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(GH_Brep)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)new GH_Brep(Value.Brep.DuplicateBrep());
          if (Value.Brep == null)
            return false;
        }
        return true;
      }

      //Cast to Points
      if (typeof(Q).IsAssignableFrom(typeof(List<Point3d>)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Topology.ToList();
        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(List<GH_Point>)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Topology.ToList();
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
      // into GsaMember.
      if (source == null) { return false; }

      if (base.CastFrom(source))
        return true;

      //Cast from Brep
      Brep brep = new Brep();
      if (GH_Convert.ToBrep(source, ref brep, GH_Conversion.Both))
      {
        List<Point3d> pts = new List<Point3d>();
        List<Curve> crvs = new List<Curve>();
        GsaMember2d mem = new GsaMember2d(brep, crvs, pts);
        this.Value = mem;
        return true;
      }

      return false;
    }
    #endregion

    #region transformation methods
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      return new GsaMember2dGoo(Value.Transform(xform));
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      return new GsaMember2dGoo(Value.Morph(xmorph));
    }

    #endregion

    #region drawing methods
    public override void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      //Draw shape.
      if (Value.Brep != null)
      {
        if (Value.Type == MemberType.VOID_CUTTER_2D)
        {
          if (args.Material.Diffuse == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
            args.Pipeline.DrawBrepShaded(Value.Brep, Helpers.Graphics.Colours.Member2dVoidCutterFace); //UI.Colour.Member2dFace
        }
        else
        {
          if (args.Material.Diffuse == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
            args.Pipeline.DrawBrepShaded(Value.Brep, Helpers.Graphics.Colours.Member2dFace); //UI.Colour.Member2dFace
          else
            args.Pipeline.DrawBrepShaded(Value.Brep, Helpers.Graphics.Colours.Member2dFaceSelected);
        }
      }
    }
    public override void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }

      // Draw shape edge
      if (Value.Brep != null)
      {
        if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
        {
          if (Value.Type == MemberType.VOID_CUTTER_2D)
          {
            args.Pipeline.DrawBrepWires(Value.Brep, Helpers.Graphics.Colours.VoidCutter, -1);
          }
          else if (!Value.IsDummy)
            args.Pipeline.DrawBrepWires(Value.Brep, Helpers.Graphics.Colours.Member2dEdge, -1);
        }
        else
          args.Pipeline.DrawBrepWires(Value.Brep, Helpers.Graphics.Colours.Member2dEdgeSelected, -1);
      }

      //Draw lines
      if (Value.PolyCurve != null & Value.Brep == null)
      {
        if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
        {
          if (Value.IsDummy)
            args.Pipeline.DrawDottedPolyline(Value.Topology, Helpers.Graphics.Colours.Dummy1D, false);
          else
          {
            if ((System.Drawing.Color)Value.Colour != System.Drawing.Color.FromArgb(0, 0, 0))
              args.Pipeline.DrawCurve(Value.PolyCurve, Value.Colour, 2);
            else
            {
              System.Drawing.Color col = Helpers.Graphics.Colours.Member2dEdge;
              args.Pipeline.DrawCurve(Value.PolyCurve, col, 2);
            }
          }
        }
        else
        {
          if (Value.IsDummy)
            args.Pipeline.DrawDottedPolyline(Value.Topology, Helpers.Graphics.Colours.Member1dSelected, false);
          else
            args.Pipeline.DrawCurve(Value.PolyCurve, Helpers.Graphics.Colours.Member1dSelected, 2);
        }
      }

      if (Value.InclusionLines != null)
      {
        for (int i = 0; i < Value.InclusionLines.Count; i++)
        {
          if (Value.IsDummy)
            args.Pipeline.DrawDottedPolyline(Value.IncLinesTopology[i], Helpers.Graphics.Colours.Member1dSelected, false);
          else
            args.Pipeline.DrawCurve(Value.InclusionLines[i], Helpers.Graphics.Colours.Member2dInclLn, 2);
        }
      }

      //Draw points.
      if (Value.Topology != null)
      {
        List<Point3d> pts = Value.Topology;
        for (int i = 0; i < pts.Count; i++)
        {
          if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
          {
            if (Value.Brep == null & (i == 0 | i == pts.Count - 1)) // draw first point bigger
              args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundSimple, 3, (Value.IsDummy) ? Helpers.Graphics.Colours.Dummy1D : Helpers.Graphics.Colours.Member1dNode);
            else
              args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundSimple, 2, (Value.IsDummy) ? Helpers.Graphics.Colours.Dummy1D : Helpers.Graphics.Colours.Member1dNode);
          }
          else
          {
            if (Value.Brep == null & (i == 0 | i == pts.Count - 1)) // draw first point bigger
              args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundControlPoint, 3, Helpers.Graphics.Colours.Member1dNodeSelected);
            else
              args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundControlPoint, 2, Helpers.Graphics.Colours.Member1dNodeSelected);
          }
        }
      }
      if (Value.InclusionPoints != null)
      {
        for (int i = 0; i < Value.InclusionPoints.Count; i++)
          args.Pipeline.DrawPoint(Value.InclusionPoints[i], Rhino.Display.PointStyle.RoundSimple, 3, (Value.IsDummy) ? Helpers.Graphics.Colours.Dummy1D : Helpers.Graphics.Colours.Member2dInclPt);
      }
    }
    #endregion
  }
}
