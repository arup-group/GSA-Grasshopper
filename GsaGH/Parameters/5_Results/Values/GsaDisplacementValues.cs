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

namespace GsaGH.Parameters {
  // For now, to be refactored
  public class GsaDisplacementValues: GsaResultsValues, IResultValues<Length, Angle, IDisplacementQuantity>{
    new public Length DmaxX { get; set; }
    new public Angle DmaxXx { get; set; }
    new public Angle DmaxXxyyzz { get; set; }
    new public Length DmaxXyz { get; set; }
    new public Length DmaxY { get; set; }
    new public Angle DmaxYy { get; set; }
    new public Length DmaxZ { get; set; }
    new public Angle DmaxZz { get; set; }
    new public Length DminX { get; set; }
    new public Angle DminXx { get; set; }
    new public Angle DminXxyyzz { get; set; }
    new public Length DminXyz { get; set; }
    new public Length DminY { get; set; }
    new public Angle DminYy { get; set; }
    new public Length DminZ { get; set; }
    new public Angle DminZz { get; set; }
    new public ResultType Type { get; set; }
    new public List<int> Ids => Results.Keys.OrderBy(x => x).ToList();

    new public ConcurrentDictionary<int, ConcurrentDictionary<int, IDisplacementQuantity>>
      Results { get; set; }
      = new ConcurrentDictionary<int, ConcurrentDictionary<int, IDisplacementQuantity>>();

    internal GsaDisplacementValues() { }

    new internal void CoordinateTransformationTo(Plane plane, Model model) {
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

        foreach (IDisplacementQuantity results in Results[elementId].Values) {
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

    new internal void UpdateMinMax() {
      if (Results.Count > 0) {
        DmaxX = Results.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Max())
         .Max();
        DmaxY = Results.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Max())
         .Max();
        try {
          DmaxZ = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Z).Max()).Max();
        } catch (Exception) {
          // shear does not set this value
        }

        try {
          DmaxXyz = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xyz).Max()).Max();
        } catch (Exception) {
          // resultant may not always be computed
        }

        DminX = Results.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Min())
         .Min();
        DminY = Results.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Min())
         .Min();
        try {
          DminZ = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Z).Min()).Min();
        } catch (Exception) {
          // shear does not set this value
        }

        try {
          DminXyz = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xyz).Min()).Min();
        } catch (Exception) {
          // resultant may not always be computed
        }
      }

      if (Results.Count <= 0) {
        return;
      }

      {
        try {
          DmaxXx = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xx).Max()).Max();
          DmaxYy = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Yy).Max()).Max();
          DmaxZz = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Zz).Max()).Max();
        } catch (Exception) {
          // some cases doesnt compute xxyyzz results at all
        }

        try {
          DmaxXxyyzz = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xxyyzz).Max()).Max();
        } catch (Exception) {
          // resultant may not always be computed
        }

        try {
          DminXx = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xx).Min()).Min();
          DminYy = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Yy).Min()).Min();
          DminZz = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Zz).Min()).Min();
        } catch (Exception) {
          // some cases doesnt compute xxyyzz results at all
        }

        try {
          DminXxyyzz = Results.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xxyyzz).Min()).Min();
        } catch (Exception) {
          // resultant may not always be computed
        }
      }
    }
  }
}
