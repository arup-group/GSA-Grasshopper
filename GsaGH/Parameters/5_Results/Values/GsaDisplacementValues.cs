using GsaAPI;
using GsaGH.Parameters._5_Results.Quantities;
using GsaGH.Parameters._5_Results.Values;
using OasysUnits;
using Rhino.Geometry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GsaGH.Parameters.GsaResultsValues;

namespace GsaGH.Parameters {
  // For now, to be refactored
  public class GsaDisplacementValues : IResultValues<IDisplacement> {
    public IDisplacement Max { get; set; }
    public IDisplacement Min { get; set; }
    
    public ResultType Type { get; set; }
    public List<int> Ids => Results.Keys.OrderBy(x => x).ToList();

    public ConcurrentDictionary<int, ConcurrentDictionary<int, IDisplacement>>
      Results { get; set; }
      = new ConcurrentDictionary<int, ConcurrentDictionary<int, IDisplacement>>();

    internal GsaDisplacementValues() { }

    internal void CoordinateTransformationTo(Plane plane, Model model) {
      // coordinate transformation
      Parallel.ForEach(Results.Keys, elementId => {
        var localAxes = new LocalAxes(model.ElementDirectionCosine(elementId));
        var local = new Plane(Point3d.Origin, localAxes.X, localAxes.Y);
        // create quaternion from two planes
        var q = Quaternion.Rotation(plane, local);

        double angle = new double();
        var axis = new Vector3d();
        q.GetRotation(out angle, out axis);

        if (angle > Math.PI) {
          angle -= 2 * Math.PI;
        }

        foreach (IDisplacement results in Results[elementId].Values) {
          var displacements = new Point3d(results.X.Value, results.Y.Value, results.Z.Value);
          displacements.Transform(Transform.Rotation(angle, axis, Point3d.Origin));
          var rotations = new Point3d(results.Xx.Value, results.Yy.Value, results.Zz.Value);
          rotations.Transform(Transform.Rotation(angle, axis, Point3d.Origin));

          var value = new Double6(displacements.X, displacements.Y, displacements.Z, rotations.X, rotations.Y, rotations.Z);
          results.SetLengthUnit(value, results.X.Unit);
          results.SetAngleUnit(value, results.Xx.Unit);
        }
      });
    }

    internal void UpdateMinMax() {
      if (Results.Count > 0) {
        Length xMaxVal = Results.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Max())
         .Max();
        Length yMaxVal = Results.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Max())
         .Max();
        Length zMaxVal= Length.Zero;
        try {
          zMaxVal = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Z).Max()).Max();
        } catch (Exception) {
          // shear does not set this value
        }
        Length xyzMaxVal= Length.Zero;
        try {
          xyzMaxVal = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xyz).Max()).Max();
        } catch (Exception) {
          // resultant may not always be computed
        }
        Max.SetLengthUnit(xMaxVal, yMaxVal, zMaxVal, xyzMaxVal);
        //////// minimum
        Length xMinVal = Results.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Min())
         .Min();
        Length yMinVal = Results.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Min())
         .Min();
        Length zMinVal = Length.Zero;
        try {
          zMinVal = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Z).Min()).Min();
        } catch (Exception) {
          // shear does not set this value
        }
        Length xyzMinVal = Length.Zero;
        try {
          xyzMinVal = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xyz).Min()).Min();
        } catch (Exception) {
          // resultant may not always be computed
        }

        Min.SetLengthUnit(xMinVal, yMinVal, zMinVal, xyzMinVal);
      }
      if (Results.Count <= 0) {
        return;
      }
      /// rotation
      {
        Angle maxXx = Angle.Zero, maxYy = Angle.Zero, maxZz = Angle.Zero, maxXxyyzz = Angle.Zero;
        try {
          maxXx = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xx).Max()).Max();
          maxYy = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Yy).Max()).Max();
          maxZz = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Zz).Max()).Max();
        } catch (Exception) {
          // some cases doesnt compute xxyyzz results at all
        }

        try {
          maxXxyyzz = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xxyyzz).Max()).Max();
        } catch (Exception) {
          // resultant may not always be computed
        }
        Max.SetAngleUnit(maxXx, maxYy, maxZz, maxXxyyzz);
        /////min
        Angle minXx = Angle.Zero, minYy = Angle.Zero, minZz = Angle.Zero, minXxyyzz = Angle.Zero;
        try {
          minXx = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xx).Min()).Min();
          minYy = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Yy).Min()).Min();
          minZz = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Zz).Min()).Min();
        } catch (Exception) {
          // some cases doesnt compute xxyyzz results at all
        }

        try {
          minXxyyzz = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xxyyzz).Min()).Min();
        } catch (Exception) {
          // resultant may not always be computed
        }
        Min.SetAngleUnit(minXx, minYy, minZz, minXxyyzz);
      }
    }
  }
}
