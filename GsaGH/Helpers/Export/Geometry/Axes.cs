using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using OasysGH.Units;

namespace GsaGH.Helpers.Export {
  internal class Axes {
    /// <summary>
    /// This method checks if the testAxis is within tolerance of an existing axis and returns the 
    /// axis ID if found. Will return 0 if no existing axis is found within the tolerance.
    /// </summary>
    /// <param name="existAxes">Dictionary of axis to check against [in meters]</param>
    /// <param name="testAxis">Axis to check for [in meters]</param>
    /// <returns></returns>
    internal static int GetExistingAxisId(IReadOnlyDictionary<int, Axis> existAxes, Axis testAxis) {
      double tolerance = DefaultUnits.Tolerance.Meters;
      foreach (int key in existAxes.Keys) {
        if (!existAxes.TryGetValue(key, out Axis gsaAxis)) {
          continue;
        }

        if (Math.Abs(testAxis.Origin.X - gsaAxis.Origin.X) <= tolerance &
            Math.Abs(testAxis.Origin.Y - gsaAxis.Origin.Y) <= tolerance &
            Math.Abs(testAxis.Origin.Z - gsaAxis.Origin.Z) <= tolerance &
            Math.Abs(testAxis.XVector.X - gsaAxis.XVector.X) <= tolerance &
            Math.Abs(testAxis.XVector.Y - gsaAxis.XVector.Y) <= tolerance &
            Math.Abs(testAxis.XVector.Z - gsaAxis.XVector.Z) <= tolerance &
            Math.Abs(testAxis.XYPlane.X - gsaAxis.XYPlane.X) <= tolerance &
            Math.Abs(testAxis.XYPlane.Y - gsaAxis.XYPlane.Y) <= tolerance &
            Math.Abs(testAxis.XYPlane.Z - gsaAxis.XYPlane.Z) <= tolerance
           )
          return key;
      }

      return 0;
    }

    internal static int AddAxis(ref Dictionary<int, Axis> existAxes, Axis axes) {
      int key = (existAxes.Count > 0) ? existAxes.Keys.Max() + 1 : 1;
      existAxes.Add(key, axes);
      return key;
    }
  }
}
