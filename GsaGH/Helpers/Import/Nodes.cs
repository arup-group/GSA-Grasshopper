using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using GsaAPI;

using GsaGH.Parameters;

using OasysUnits;

using Rhino.Geometry;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.Import {
  /// <summary>
  ///   Class containing functions to import various object types from GSA
  /// </summary>
  internal static class Nodes {

    /// <summary>
    ///   Method to create a Rhino Plane from a GSA Axis
    /// </summary>
    /// <param name="axis">GSA Axis to create plane from</param>
    /// <param name="unit"></param>
    /// <returns></returns>
    internal static Plane AxisToPlane(Axis axis, LengthUnit unit) {
      if (axis == null) {
        return Plane.Unset;
      }

      Point3d origin = Point3dFromXyzUnit(axis.Origin.X, axis.Origin.Y, axis.Origin.Z, unit);
      Vector3d xAxis = Vector3dFromXyzUnit(axis.XVector.X, axis.XVector.Y, axis.XVector.Z, unit);
      if (xAxis.IsZero) {
        return Plane.Unset;
      }

      var xUnit = new Vector3d(xAxis);
      xUnit.Unitize();

      Vector3d yAxis = Vector3dFromXyzUnit(axis.XYPlane.X, axis.XYPlane.Y, axis.XYPlane.Z, unit);
      if (yAxis.IsZero) {
        return Plane.Unset;
      }

      var yUnit = new Vector3d(yAxis);
      yUnit.Unitize();

      if (xUnit.Equals(yUnit)) {
        return Plane.Unset;
      }

      // Create new plane with Rhino method
      var pln = new Plane(origin, xAxis, yAxis);

      return pln;
    }

    internal static GsaNode GetNode(
      Node node, LengthUnit modelUnit, int id, ReadOnlyDictionary<int, Axis> axDict = null) {
      var local = new Plane();
      // add local axis if node has Axis property
      if (axDict == null) {
        return new GsaNode(node, id, modelUnit, local);
      }

      if (node.AxisProperty > 0) {
        axDict.TryGetValue(node.AxisProperty, out Axis axis);
        local = AxisToPlane(axis, modelUnit);
      } else {
        switch (node.AxisProperty) {
          case 0:
            // local axis = Global
            // do nothing, XY-plan already set by default
            break;

          case -11:
            // local axis = X-elevation
            local = Plane.WorldYZ;
            break;

          case -12:
            // local axis = X-elevation
            local = Plane.WorldZX;
            break;

          case -13:
            // local axis = vertical
            // GSA naming is confusing, but this is a XY-plane
            local = Plane.WorldXY;
            break;

          case -14:
            // local axis = global cylindric
            // no method in Rhino/GH to handle cylindric coordinate system
            local = Plane.Unset;
            break;
        }
      }

      return new GsaNode(node, id, modelUnit, local);
    }

    internal static ConcurrentDictionary<int, GsaNodeGoo> GetNodeDictionary(
      ReadOnlyDictionary<int, Node> nDict, LengthUnit unit,
      ReadOnlyDictionary<int, Axis> axDict = null) {
      var outNodes = new ConcurrentDictionary<int, GsaNodeGoo>();
      Parallel.ForEach(nDict,
        node => outNodes.TryAdd(node.Key,
          new GsaNodeGoo(GetNode(node.Value, unit, node.Key, axDict))));
      return outNodes;
    }

    /// <summary>
    ///   Method to import Nodes from a GSA model.
    ///   Will output a list of GsaNodeGoos.
    ///   Input node dictionary pre-filtered for selected nodes to import;
    /// </summary>
    /// <param name="nDict">Dictionary of GSA Nodes pre-filtered for nodes to import</param>
    /// <param name="axDict"></param>
    /// <param name="springProps"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    internal static ConcurrentBag<GsaNodeGoo> GetNodes(
      ReadOnlyDictionary<int, Node> nDict, LengthUnit unit,
      ReadOnlyDictionary<int, Axis> axDict = null,
      ReadOnlyDictionary<int, GsaSpringPropertyGoo> springProps = null) {
      var outNodes = new ConcurrentBag<GsaNodeGoo>();
      Parallel.ForEach(nDict, node => {
        GsaNode n = GetNode(node.Value, unit, node.Key, axDict);
        if (springProps != null && node.Value.SpringProperty != 0
          && springProps.ContainsKey(node.Value.SpringProperty)) {
          n.SpringProperty = springProps[node.Value.SpringProperty].Value;
        }

        outNodes.Add(new GsaNodeGoo(n));
      });
      return outNodes;
    }

    internal static Point3d Point3dFromNode(Node node, LengthUnit unit) {
      return (unit == LengthUnit.Meter) ?
        // skip unitsnet conversion, gsa api node always in meters
        new Point3d(node.Position.X, node.Position.Y, node.Position.Z)
        : new Point3d(new Length(node.Position.X, LengthUnit.Meter).As(unit),
            new Length(node.Position.Y, LengthUnit.Meter).As(unit),
            new Length(node.Position.Z, LengthUnit.Meter).As(unit));
    }

    internal static Point3d Point3dFromXyzUnit(double x, double y, double z, LengthUnit modelUnit) {
      return (modelUnit == LengthUnit.Meter) ?
        new Point3d(x, y, z) : // skip unitsnet conversion, gsa api node always in meters
        new Point3d(new Length(x, LengthUnit.Meter).As(modelUnit),
          new Length(y, LengthUnit.Meter).As(modelUnit),
          new Length(z, LengthUnit.Meter).As(modelUnit));
    }

    internal static Vector3d Vector3dFromXyzUnit(
      double x, double y, double z, LengthUnit modelUnit) {
      return (modelUnit == LengthUnit.Meter) ?
        new Vector3d(x, y, z) : // skip unitsnet conversion, gsa api node always in meters
        new Vector3d(new Length(x, LengthUnit.Meter).As(modelUnit),
          new Length(y, LengthUnit.Meter).As(modelUnit),
          new Length(z, LengthUnit.Meter).As(modelUnit));
    }
  }
}
