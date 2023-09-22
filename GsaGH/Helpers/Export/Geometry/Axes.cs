using System;
using GsaAPI;
using OasysGH.Units;

namespace GsaGH.Helpers.Export {
  internal partial class ModelAssembly {
    internal int TryGetExistingAxisId(Axis testAxis) {
      double tolerance = DefaultUnits.Tolerance.Meters;
      foreach (int key in Axes.ReadOnlyDictionary.Keys) {
        if (!Axes.ReadOnlyDictionary.TryGetValue(key, out Axis gsaAxis)) {
          continue;
        }

        if (testAxis.Name == string.Empty
          && gsaAxis.Type == testAxis.Type
          && Math.Abs(testAxis.Origin.X - gsaAxis.Origin.X) <= tolerance
          && Math.Abs(testAxis.Origin.Y - gsaAxis.Origin.Y) <= tolerance
          && Math.Abs(testAxis.Origin.Z - gsaAxis.Origin.Z) <= tolerance
          && Math.Abs(testAxis.XVector.X - gsaAxis.XVector.X) <= tolerance
          && Math.Abs(testAxis.XVector.Y - gsaAxis.XVector.Y) <= tolerance
          && Math.Abs(testAxis.XVector.Z - gsaAxis.XVector.Z) <= tolerance
          && Math.Abs(testAxis.XYPlane.X - gsaAxis.XYPlane.X) <= tolerance
          && Math.Abs(testAxis.XYPlane.Y - gsaAxis.XYPlane.Y) <= tolerance
          && Math.Abs(testAxis.XYPlane.Z - gsaAxis.XYPlane.Z) <= tolerance) {
          return key;
        }
      }

      return Axes.AddValue(testAxis);
    }
  }
}
