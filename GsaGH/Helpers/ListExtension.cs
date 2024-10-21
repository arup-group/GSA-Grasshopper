using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using GsaAPI;

using GsaGH.Parameters;

using OasysUnits;

using Rhino.Collections;
using Rhino.Geometry;

namespace GsaGH.Helpers {
  public static class ListExtension {
    public static bool IsNullOrEmpty<T>(this ICollection<T> value) {
      if (value == null || value.Count == 0) {
        return true;
      }

      return false;
    }

    public static bool IsNullOrEmpty<T>(this ConcurrentBag<T> value) {
      if (value == null || value.Count == 0) {
        return true;
      }

      return false;
    }

    public static void Morph(this Point3dList value, SpaceMorph xmorph) {
      var morphed = new Point3dList();
      for (int i = 0; i < value.Count; i++) {
        morphed.Add(xmorph.MorphPoint(value[i]));
      }

      value = morphed;
    }

    public static List<Point3dList> Duplicate(this List<Point3dList> value) {
      var duplicates = new List<Point3dList>();
      for (int i = 0; i < value.Count; i++) {
        duplicates.Add(value[i].Duplicate());
      }

      return duplicates;
    }

    public static void SetMembers<T>(this List<GSAElement> value, IList<T> list) {
      if (value.IsNullOrEmpty()) {
        throw new ArgumentException(
          $"Unable to set new {list.GetType().Name} members in Element list as it is null or " +
          $"empty");
      }

      if (list.IsNullOrEmpty()) {
        throw new ArgumentException(
          $"Unable to set new {list.GetType().Name} members in Element list as the new input " +
          $"is null or empty");
      }

      for (int i = 0; i < value.Count; i++) {
        switch (list) {
          case List<int> groups:
            value[i].Group = groups.Count > i ? groups[i] : groups.Last();
            break;

          case List<bool> isDummies:
            value[i].IsDummy = isDummies.Count > i ? isDummies[i] : isDummies.Last();
            break;

          case List<string> names:
            value[i].Name = names.Count > i ? names[i] : names.Last();
            break;

          case List<Angle> angles:
            value[i].OrientationAngle = angles.Count > i ? angles[i].Degrees : angles.Last().Degrees;
            break;

          case List<GsaOffset> offsets:
            if (offsets.Count > i) {
              value[i].Offset.X1 = offsets[i].X1.Meters;
              value[i].Offset.X2 = offsets[i].X2.Meters;
              value[i].Offset.Y = offsets[i].Y.Meters;
              value[i].Offset.Z = offsets[i].Z.Meters;
            } else {
              value[i].Offset.X1 = offsets.Last().X1.Meters;
              value[i].Offset.X2 = offsets.Last().X2.Meters;
              value[i].Offset.Y = offsets.Last().Y.Meters;
              value[i].Offset.Z = offsets.Last().Z.Meters;
            }

            break;

          case List<ElementType> types:
            value[i].Type = types.Count > i ? types[i] : types.Last();
            break;

          case List<Color> colors:
            value[i].Colour = colors.Count > i ? colors[i] : (ValueType)colors.Last();
            break;
        }
      }
    }
  }
}
