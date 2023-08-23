using System;
using System.Drawing;
using System.Security.Cryptography;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaGridLine" /> can be used in Grasshopper.
  /// </summary>
  public class GsaGridLineGoo : GH_OasysGeometricGoo<GsaGridLine> {
    public static string Description => "GSA Grid Line";
    public static string Name => "GridLine";
    public static string NickName => "GL";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaGridLineGoo(GsaGridLine item) : base(item) { }

    public override IGH_GeometricGoo Duplicate() {
      return new GsaGridLineGoo(Value);
    }

    public override bool CastTo<Q>(ref Q target) {
      if (Value != null) {
        if (typeof(Q).IsAssignableFrom(typeof(GH_Line))) {
          if (Value._gridLine.Shape == GsaAPI.GridLineShape.Line) {
            var line = Helpers.Import.GridLines.ToLine(Value._gridLine);
            var ghLine = new GH_Line();
            GH_Convert.ToGHLine(line, GH_Conversion.Both, ref ghLine);
            target = (Q)(object)ghLine;
            return true;
          }
        }
        if (typeof(Q).IsAssignableFrom(typeof(GH_Arc))) {
          if (Value._gridLine.Shape == GsaAPI.GridLineShape.Arc) {
            var arc = Helpers.Import.GridLines.ToArc(Value._gridLine);
            var ghArc = new GH_Arc();
            GH_Convert.ToGHArc(arc, GH_Conversion.Both, ref ghArc);
            target = (Q)(object)ghArc;
            return true;
          } else {
            return false;
          }
        }
      }

      return base.CastTo<Q>(ref target);
    }

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) {
      //if (Value == null) {
      //  return;
      //}

      Color color = Color.Black;
      int pattern = 999999;
      if(Value._pattern != 0) {
        pattern = Value._pattern;
      }
      int thickness = 1;
      Point3d[] points;
      if (Value._curve.IsLinear()) {
        points = new Point3d[2] { Value._curve.SegmentCurve(0).PointAtStart, Value._curve.SegmentCurve(0).PointAtEnd };
      } else {

        points = Value._curve.SegmentCurve(0).DivideEquidistant(Value._curve.SegmentCurve(0).GetLength() / 360);
      }
      if (points != null) {
        args.Pipeline.DrawPatternedPolyline(points, color, pattern, thickness, false);
      }
    }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value == null) {
        return;
      }


    }

    public override GeometryBase GetGeometry() {
      return Value == null ? null : (GeometryBase)Value._curve;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      //  return new GsaMember1dGoo(Value.Morph(xmorph));
      throw new NotImplementedException();
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      //return new GsaGridLineGoo(Value.Transform(xform));
      throw new NotImplementedException();
    }
  }
}
