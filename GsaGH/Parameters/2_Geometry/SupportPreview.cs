using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.Graphics;
using Rhino.Display;
using Rhino.DocObjects;
using Rhino.Geometry;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  public class SupportPreview {
    public Brep SupportSymbol { get; set; }
    public Text3d Text { get; set; }
    public Line Xaxis { get; set; }
    public Line Yaxis { get; set; }
    public Line Zaxis { get; set; }

    public SupportPreview(GsaBool6 restraint, Plane localAxis, Point3d pt, bool isGlobalAxis) {
      if (restraint.X & restraint.Y & restraint.Z & !restraint.Xx & !restraint.Yy & !restraint.Zz) {
        Plane plane = localAxis.Clone();
        if (!plane.IsValid) {
          plane = Plane.WorldXY;
        }

        plane.Origin = pt;
        var pin = new Cone(plane, -0.4, 0.4);
        SupportSymbol = pin.ToBrep(true);
      } else if (restraint.X & restraint.Y & restraint.Z & restraint.Xx & restraint.Yy
        & restraint.Zz) {
        Plane plane = localAxis.Clone();
        if (!plane.IsValid) {
          plane = Plane.WorldXY;
        }

        plane.Origin = pt;
        var fix = new Box(plane, new Interval(-0.3, 0.3), new Interval(-0.3, 0.3),
          new Interval(-0.2, 0));
        SupportSymbol = fix.ToBrep();
      } else {
        Plane plane = localAxis.Clone();
        if (!plane.IsValid) {
          plane = Plane.WorldXY;
        }

        plane.Origin = pt;
        string rest = string.Empty;
        if (restraint.X) {
          rest += "X";
        }

        if (restraint.Y) {
          rest += "Y";
        }

        if (restraint.Z) {
          rest += "Z";
        }

        if (restraint.Xx) {
          rest += "XX";
        }

        if (restraint.Yy) {
          rest += "YY";
        }

        if (restraint.Zz) {
          rest += "ZZ";
        }

        Text = new Text3d(rest, plane, 0.3) {
          HorizontalAlignment = TextHorizontalAlignment.Left,
          VerticalAlignment = TextVerticalAlignment.Top,
        };
      }

      if (!isGlobalAxis) {
        return;
      }

      Plane local = localAxis.Clone();
      local.Origin = pt;

      Xaxis = new Line(pt, local.XAxis, 0.5);
      Yaxis = new Line(pt, local.YAxis, 0.5);
      Zaxis = new Line(pt, local.ZAxis, 0.5);
    }

    public void DrawViewportWires(GH_PreviewWireArgs args) {
      if (args.Color == Color.FromArgb(255, 150, 0, 0)) {
        // this is a workaround to change colour between selected and not

        if (SupportSymbol != null) {
          args.Pipeline.DrawBrepShaded(SupportSymbol, Colours.SupportSymbol);
        }

        if (Text != null) {
          args.Pipeline.Draw3dText(Text, Colours.Support);
        }
      } else {
        if (SupportSymbol != null) {
          args.Pipeline.DrawBrepShaded(SupportSymbol, Colours.SupportSymbolSelected);
        }

        if (Text != null) {
          args.Pipeline.Draw3dText(Text, Colours.NodeSelected);
        }
      }

      if (Xaxis != null) {
        args.Pipeline.DrawLine(Xaxis, Color.FromArgb(255, 244, 96, 96), 1);
        args.Pipeline.DrawLine(Yaxis, Color.FromArgb(255, 96, 244, 96), 1);
        args.Pipeline.DrawLine(Zaxis, Color.FromArgb(255, 96, 96, 234), 1);
      }
    }
  }
}
