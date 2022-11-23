using System;
using System.Collections.Generic;
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
  /// Goo wrapper class, makes sure <see cref="GsaMember1d"/> can be used in Grasshopper.
  /// </summary>
  public class GsaMember1dGoo : GH_OasysGeometricGoo<GsaMember1d>
  {
    public static string Name => "Member1D";
    public static string NickName => "M1D";
    public static string Description => "GSA 1D Member";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    
    public GsaMember1dGoo(GsaMember1d item) : base(item)
    {
    }
    
    public override IGH_GeometricGoo Duplicate() => new GsaMember1dGoo(this.Value);
    public override GeometryBase GetGeometry() => this.Value.PolyCurve;

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

      //Cast from Curve
      Curve crv = null;
      if (GH_Convert.ToCurve(source, ref crv, GH_Conversion.Both))
      {
        GsaMember1d member = new GsaMember1d(crv);
        this.Value = member;
        return true;
      }

      return false;
    }
    #endregion

    #region transformation methods
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      return new GsaMember1dGoo(Value.Transform(xform));
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      return new GsaMember1dGoo(Value.Morph(xmorph));
    }

    #endregion

    #region drawing methods
    public override void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      // no meshes to be drawn
    }
    public override void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }

      //Draw lines
      if (Value.PolyCurve != null)
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
              System.Drawing.Color col = Helpers.Graphics.Colours.ElementType(Value.Type1D);
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

      //Draw points.
      if (Value.Topology != null)
      {
        if (!Value.IsDummy)
        {
          List<Point3d> pts = Value.Topology;
          for (int i = 0; i < pts.Count; i++)
          {
            if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
            {
              if (i == 0 | i == pts.Count - 1) // draw first point bigger
                args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundSimple, 2, Helpers.Graphics.Colours.Member1dNode);
              else
                args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundSimple, 1, Helpers.Graphics.Colours.Member1dNode);
            }
            else
            {
              if (i == 0 | i == pts.Count - 1) // draw first point bigger
                args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundControlPoint, 2, Helpers.Graphics.Colours.Member1dNodeSelected);
              else
                args.Pipeline.DrawPoint(pts[i], Rhino.Display.PointStyle.RoundControlPoint, 1, Helpers.Graphics.Colours.Member1dNodeSelected);
            }
          }
        }
      }

      //Draw releases
      if (!Value.IsDummy)
      {
        if (Value.previewGreenLines != null)
        {
          foreach (Line ln1 in Value.previewGreenLines)
            args.Pipeline.DrawLine(ln1, Helpers.Graphics.Colours.Support);
          foreach (Line ln2 in Value.previewRedLines)
            args.Pipeline.DrawLine(ln2, Helpers.Graphics.Colours.Release);
        }
      }
    }
    #endregion
  }
}
