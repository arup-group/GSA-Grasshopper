﻿using System.Globalization;
using System;
using OasysUnits;
using Rhino.Collections;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  internal static class GridLoadHelper {
    private static readonly char ListSeparator = Convert.ToChar(CultureInfo.CurrentCulture.TextInfo.ListSeparator);

    internal static string CreateDefinition(Point3dList controlPoints, Plane plane) {
      string desc = string.Empty;
      for (int i = 0; i < controlPoints.Count; i++) {
        if (i > 0) {
          desc += " ";
        }

        plane.RemapToPlaneSpace(controlPoints[i], out Point3d temppt);
        // format accepted by GSA: (0,0) (0,1) (1,2) (3,4) (4,0)(m)
        desc += $"({temppt.X},{temppt.Y})";
      }

      return desc;
    }

    internal static string ClearDefinitionForUnit(string definition) {
      return ClearDefGetUnit(definition).def;
    }

    internal static Point3dList ConvertPoints(string definition, LengthUnit desiredUnit, Plane localPlane) {
      (LengthUnit lengthUnit, string def) = ClearDefGetUnit(definition);
      var points = new Point3dList();
      string[] pts = def.Split(')');
      var map = Transform.ChangeBasis(localPlane, Plane.WorldXY);
      foreach (string ptStr in pts) {
        if (ptStr != string.Empty) {
          string pt = ptStr.Replace("(", string.Empty).Trim();
          var x = new Length(double.Parse(pt.Split(ListSeparator)[0]), lengthUnit);
          var y = new Length(double.Parse(pt.Split(ListSeparator)[1]), lengthUnit);
          var point = new Point3d(x.As(desiredUnit), y.As(desiredUnit), 0);
          point.Transform(map);
          points.Add(point);
        }
      }
      return points;
    }

    private static (LengthUnit lengthUnit, string def) ClearDefGetUnit(string definition) {
      LengthUnit lengthUnit = LengthUnit.Meter;
      if (definition.EndsWith("(mm)")) {
        lengthUnit = LengthUnit.Millimeter;
        definition = definition.Replace("(mm)", string.Empty);
      }
      if (definition.EndsWith("(cm)")) {
        lengthUnit = LengthUnit.Centimeter;
        definition = definition.Replace("(cm)", string.Empty);
      }
      if (definition.EndsWith("(ft)")) {
        lengthUnit = LengthUnit.Foot;
        definition = definition.Replace("(ft)", string.Empty);
      }
      if (definition.EndsWith("(in)")) {
        lengthUnit = LengthUnit.Inch;
        definition = definition.Replace("(in)", string.Empty);
      }
      definition = definition.Replace("(m)", string.Empty);
      return (lengthUnit, definition);
    }
  }
}
