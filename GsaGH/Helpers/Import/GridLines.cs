using System;
using System.Collections.Generic;
using GsaAPI;
using GsaGH.Parameters;
using Rhino.Geometry;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Helpers.Import {
  internal class GridLines {
    internal static List<GsaGridLine> GetGridLines(GsaModel model) {
      var gridLines = new List<GsaGridLine>();
      foreach (GridLine gridLine in model.Model.GridLines().Values) {
        var curve = new PolyCurve();
        if (gridLine.Shape is GridLineShape.Line) {
          Line line = ToLine(gridLine);
          curve.Append(line);
        } else if (gridLine.Shape is GridLineShape.Arc) {
          Arc arc = ToArc(gridLine);
          curve.Append(arc);
        }
        gridLines.Add(new GsaGridLine(gridLine, curve));
      }
      return gridLines;
    }

    internal static Arc ToArc(GridLine gridLine) {
      var center = new Point3d(gridLine.X, gridLine.Y, 0);
      double angleRadians = (gridLine.Theta2 - gridLine.Theta1) * Math.PI / 180.0;
      double radius = gridLine.Length;
      var arc = new Arc(center, radius, angleRadians) { 
        StartAngle = gridLine.Theta1 * Math.PI / 180.0,
        EndAngle = gridLine.Theta2 * Math.PI / 180.0
      };
      return arc;
    }

    internal static Line ToLine(GridLine gridLine) {
      var start = new Point3d(gridLine.X, gridLine.Y, 0);
      var end = new Point3d(gridLine.X + gridLine.Length, gridLine.Y, 0);
      var line = new Line(start, end);
      Transform.Rotation(gridLine.Theta1 * Math.PI / 180.0, start);

      return line;
    }
  }
}
