using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.Graphics;

using Rhino;
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
      Plane plane = localAxis.Clone();
      if (!plane.IsValid) {
        plane = Plane.WorldXY;
      }
      plane.Origin = pt;

      if (restraint.X & restraint.Y & restraint.Z & !restraint.Xx & !restraint.Yy & !restraint.Zz) {
        var pin = new Cone(plane, -0.4, 0.4);
        SupportSymbol = pin.ToBrep(true);
      } else if (restraint.X & restraint.Y & restraint.Z & restraint.Xx & restraint.Yy
        & restraint.Zz) {
        var fix = new Box(plane, new Interval(-0.3, 0.3), new Interval(-0.3, 0.3),
          new Interval(-0.2, 0));
        SupportSymbol = fix.ToBrep();
      } else {
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

      if (isGlobalAxis) {
        return;
      }

      Xaxis = new Line(pt, plane.XAxis, 0.5);
      Yaxis = new Line(pt, plane.YAxis, 0.5);
      Zaxis = new Line(pt, plane.ZAxis, 0.5);
    }

    public void BakeGeometry(
      ref GH_BakeUtility gH_BakeUtility, ref List<Guid> objIds, RhinoDoc doc, ObjectAttributes att) {
      att ??= doc.CreateDefaultAttributes();
      att.ColorSource = ObjectColorSource.ColorFromObject;
      ObjectAttributes meshAtt = att.Duplicate();
      if (SupportSymbol != null) {
        gH_BakeUtility.BakeObject(new GH_Brep(SupportSymbol), meshAtt, doc);
      }

      if (Xaxis != null) {
        ObjectAttributes lnAtt = att.Duplicate();
        lnAtt.ObjectColor = Color.FromArgb(255, 244, 96, 96);
        gH_BakeUtility.BakeObject(new GH_Line(Xaxis), lnAtt, doc);
      }

      if (Yaxis != null) {
        ObjectAttributes lnAtt = att.Duplicate();
        lnAtt.ObjectColor = Color.FromArgb(255, 96, 244, 96);
        gH_BakeUtility.BakeObject(new GH_Line(Yaxis), lnAtt, doc);
      }

      if (Zaxis != null) {
        ObjectAttributes lnAtt = att.Duplicate();
        lnAtt.ObjectColor = Color.FromArgb(255, 96, 96, 234);
        gH_BakeUtility.BakeObject(new GH_Line(Zaxis), lnAtt, doc);
      }

      if (Text != null) {
        objIds.Add(doc.Objects.AddText(Text, att));
      }
    }

    public void DrawViewportWires(GH_PreviewWireArgs args) {
      if (args.Color == Colours.EntityIsNotSelected) {
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
