using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Geometry;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaGridLine" /> can be used in Grasshopper.
  /// </summary>
  public class GsaGridLineGoo : GH_OasysGoo<GsaGridLine> {
    public static string Description => "GSA Grid Line";
    public static string Name => "GridLine";
    public static string NickName => "GL";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaGridLineGoo(GsaGridLine item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaGridLineGoo(Value);
    }

    public override bool CastTo<Q>(ref Q target) {
      if (Value != null) {
        if (typeof(Q).IsAssignableFrom(typeof(GH_Line))) {
          if (Value._gridLine.Shape == GsaAPI.GridLineShape.Line) {
            var start = new Point3d(Value._gridLine.X, Value._gridLine.Y, 0);
            var direction = new Vector3d(1, 0, 0);
            direction.Rotate(Value._gridLine.Theta1 * Math.PI / 180.0, Vector3d.ZAxis);
            var line = new Line(start, direction, Value._gridLine.Length);

            var ghLine = new GH_Line();
            GH_Convert.ToGHLine(line, GH_Conversion.Both, ref ghLine);
            target = (Q)(object)ghLine;
            return true;
          }
        }
        if (typeof(Q).IsAssignableFrom(typeof(GH_Arc))) {
          if (Value._gridLine.Shape == GsaAPI.GridLineShape.Arc) {
            var center = new Point3d(Value._gridLine.X, Value._gridLine.Y, 0);
            double angleRadians = (Value._gridLine.Theta2 - Value._gridLine.Theta1) * Math.PI / 180.0;
            double radius = Value._gridLine.Length / angleRadians;
            var arc = new Arc(center, radius, angleRadians);
            arc.Transform(Transform.Rotation(Value._gridLine.Theta1 * Math.PI / 180.0, center));

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
  }
}
