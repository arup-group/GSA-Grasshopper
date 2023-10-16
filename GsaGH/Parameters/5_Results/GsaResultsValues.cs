using GsaAPI;
using OasysUnits;
using Rhino.Geometry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {


  public class GsaResultsValues{
    public enum ResultType {
      Displacement,
      Force,
      Stress,
      Shear,
      StrainEnergy,
      Footfall,
    }
    public IQuantity DmaxX { get; set; }
    public IQuantity DmaxXx { get; set; }
    public IQuantity DmaxXxyyzz { get; set; }
    public IQuantity DmaxXyz { get; set; }
    public IQuantity DmaxY { get; set; }
    public IQuantity DmaxYy { get; set; }
    public IQuantity DmaxZ { get; set; }
    public IQuantity DmaxZz { get; set; }
    public IQuantity DminX { get; set; }
    public IQuantity DminXx { get; set; }
    public IQuantity DminXxyyzz { get; set; }
    public IQuantity DminXyz { get; set; }
    public IQuantity DminY { get; set; }
    public IQuantity DminYy { get; set; }
    public IQuantity DminZ { get; set; }
    public IQuantity DminZz { get; set; }
    public ResultType Type { get; set; }
    public List<int> Ids => XyzResults.Keys.OrderBy(x => x).ToList();

    internal ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>>
      XxyyzzResults { get; set; }
      = new ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>>();
    /// <summary>
    ///   Translation, forces, etc results
    ///   dictionary (key = node/ elementID, value= dictionary( key = position on element, value= value))
    /// </summary>
    internal ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>>
      XyzResults { get; set; }
      = new ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>>();
    internal ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>>
      XyyyzzResults { get; set; }
      = new ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>>();

    internal GsaResultsValues() { }

    internal void CoordinateTransformationTo(Plane plane, Model model) {
      // coordinate transformation
      Parallel.ForEach(XyzResults.Keys, elementId => {
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

        foreach (GsaResultQuantity results in XyzResults[elementId].Values) {
          var displacements = new Point3d(results.X.Value, results.Y.Value, results.Z.Value);
          displacements.Transform(Transform.Rotation(angle, axis, Point3d.Origin));

          results.X = new Length(displacements.X, (LengthUnit)results.X.Unit);
          results.Y = new Length(displacements.Y, (LengthUnit)results.Y.Unit);
          results.Z = new Length(displacements.Z, (LengthUnit)results.Z.Unit);
        }

        foreach (GsaResultQuantity results in XxyyzzResults[elementId].Values) {
          var rotations = new Point3d(results.X.Value, results.Y.Value, results.Z.Value);
          rotations.Transform(Transform.Rotation(angle, axis, Point3d.Origin));

          results.X = new Angle(rotations.X, (AngleUnit)results.X.Unit);
          results.Y = new Angle(rotations.Y, (AngleUnit)results.Y.Unit);
          results.Z = new Angle(rotations.Z, (AngleUnit)results.Z.Unit);
        }
      });
    }

    internal void UpdateMinMax() {
      if (XyzResults.Count > 0) {
        DmaxX = XyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Max())
         .Max();
        DmaxY = XyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Max())
         .Max();
        try {
          DmaxZ = XyzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Z).Max()).Max();
        } catch (Exception) {
          // shear does not set this value
        }

        try {
          DmaxXyz = XyzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xyz).Max()).Max();
        } catch (Exception) {
          // resultant may not always be computed
        }

        DminX = XyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Min())
         .Min();
        DminY = XyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Min())
         .Min();
        try {
          DminZ = XyzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Z).Min()).Min();
        } catch (Exception) {
          // shear does not set this value
        }

        try {
          DminXyz = XyzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xyz).Min()).Min();
        } catch (Exception) {
          // resultant may not always be computed
        }
      }

      if (XxyyzzResults.Count <= 0) {
        return;
      }

      {
        try {
          DmaxXx = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.X).Max()).Max();
          DmaxYy = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Y).Max()).Max();
          DmaxZz = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Z).Max()).Max();
        } catch (Exception) {
          // some cases doesnt compute xxyyzz results at all
        }

        try {
          DmaxXxyyzz = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xyz).Max()).Max();
        } catch (Exception) {
          // resultant may not always be computed
        }

        try {
          DminXx = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.X).Min()).Min();
          DminYy = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Y).Min()).Min();
          DminZz = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Z).Min()).Min();
        } catch (Exception) {
          // some cases doesnt compute xxyyzz results at all
        }

        try {
          DminXxyyzz = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xyz).Min()).Min();
        } catch (Exception) {
          // resultant may not always be computed
        }
      }
    }
  }
}