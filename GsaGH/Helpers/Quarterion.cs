using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using Rhino.Geometry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GsaGH.Helpers {
  public static class Quarterion {
    internal static Entity1dDisplacements CoordinateTransformationTo(IDictionary<int, IList<IEntity1dDisplacement>> displacementSubset, Plane plane, Model model) {
      var values = new ConcurrentDictionary<int, IList<IEntity1dDisplacement>>();
      Parallel.ForEach(displacementSubset, kvp => {
        var localAxes = new LocalAxes(model.ElementDirectionCosine(kvp.Key));
        var local = new Plane(Point3d.Origin, localAxes.X, localAxes.Y);
        var q = Quaternion.Rotation(plane, local);
        q.GetRotation(out double angle, out Vector3d axis);
        if (angle > Math.PI) {
          angle -= 2 * Math.PI;
        }

        var permutationResults = new Collection<IEntity1dDisplacement>();
        foreach (IEntity1dDisplacement permutation in kvp.Value) {
          var results = new Collection<Double6>();
          foreach (IDisplacement result in permutation.Results.Values) {
            var d = new Point3d(result.X.Meters, result.Y.Meters, result.Z.Meters);
            d.Transform(Transform.Rotation(angle, axis, Point3d.Origin));
            var r = new Point3d(result.Xx.Radians, result.Yy.Radians, result.Zz.Radians);
            r.Transform(Transform.Rotation(angle, axis, Point3d.Origin));
            results.Add(new Double6(d.X, d.Y, d.Z, r.X, r.Y, r.Z));
          }
          permutationResults.Add(new Entity1dDisplacement(new ReadOnlyCollection<Double6>(results),
            new ReadOnlyCollection<double>(permutation.Results.Keys.ToList())));
        }
        values.TryAdd(kvp.Key, permutationResults);
      });
      return new Entity1dDisplacements(values);
    }
  }
}
