using System.Threading.Tasks;
using System.Collections.Concurrent;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits.Units;
using Rhino.Geometry;
using OasysUnits;
using System.Collections.ObjectModel;

namespace GsaGH.Helpers.Import
{
  /// <summary>
  /// Class containing functions to import various object types from GSA
  /// </summary>
  internal class Nodes
  {
    internal static GsaNode GetNode(Node node, LengthUnit modelUnit, int ID, ReadOnlyDictionary<int, Axis> axDict = null)
    {
      Plane local = new Plane();
      // add local axis if node has Axis property
      if (axDict != null)
      {
        if (node.AxisProperty > 0)
        {
          axDict.TryGetValue(node.AxisProperty, out Axis axis);
          local = AxisToPlane(axis, modelUnit);
        }
        else
        {
          switch (node.AxisProperty)
          {
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
      }

      // create new node with basic Position and ID values
      return new GsaNode(node, ID, modelUnit, local);
    }


    /// <summary>
    /// Method to import Nodes from a GSA model.
    /// Will output a list of GsaNodeGoos.
    /// Input node dictionary pre-filtered for selected nodes to import;
    /// </summary>
    /// <param name="nDict">Dictionary of GSA Nodes pre-filtered for nodes to import</param>
    /// <param name="model">GSA Model, only used in case node refers to a local axis</param>
    /// <returns></returns>
    internal static ConcurrentBag<GsaNodeGoo> GetNodes(ReadOnlyDictionary<int, Node> nDict, LengthUnit unit, ReadOnlyDictionary<int, Axis> axDict = null, bool duplicateApiObjects = false)
    {
      ConcurrentBag<GsaNodeGoo> outNodes = new ConcurrentBag<GsaNodeGoo>();
      Parallel.ForEach(nDict, node =>
      {
        outNodes.Add(new GsaNodeGoo(GetNode(node.Value, unit, node.Key, axDict), duplicateApiObjects));
      });
      return outNodes;

      // use linq parallel
      //return nDict.AsParallel().Select(node => new GsaNodeGoo(GetNode(node.Value, unit, node.Key, axDict)));
    }
    internal static ConcurrentDictionary<int, GsaNodeGoo> GetNodeDictionary(ReadOnlyDictionary<int, Node> nDict, LengthUnit unit, ReadOnlyDictionary<int, Axis> axDict = null)
    {
      ConcurrentDictionary<int, GsaNodeGoo> outNodes = new ConcurrentDictionary<int, GsaNodeGoo>();
      Parallel.ForEach(nDict, node =>
      {
        outNodes.TryAdd(node.Key, new GsaNodeGoo(GetNode(node.Value, unit, node.Key, axDict)));
      });
      return outNodes;

      // use linq parallel
      //return nDict.AsParallel().Select(node => new GsaNodeGoo(GetNode(node.Value, unit, node.Key, axDict)));
    }

    /// <summary>
    /// Method to create a Rhino Plane from a GSA Axis
    /// </summary>
    /// <param name="axis">GSA Axis to create plane from</param>
    /// <returns></returns>
    internal static Plane AxisToPlane(Axis axis, LengthUnit unit)
    {
      if (axis == null) { return Plane.Unset; }

      // origin point from GSA Axis
      Point3d origin = Point3dFromXYZUnit(axis.Origin.X, axis.Origin.Y, axis.Origin.Z, unit);

      // X-axis from GSA Axis
      Vector3d xAxis = Vector3dFromXYZUnit(axis.XVector.X, axis.XVector.Y, axis.XVector.Z, unit);

      // check if vector is zero-length
      if (xAxis.IsZero) { return Plane.Unset; }
      // create unitised vector
      Vector3d xUnit = new Vector3d(xAxis);
      xUnit.Unitize();

      // Y-axis from GSA Axis
      Vector3d yAxis = Vector3dFromXYZUnit(axis.XYPlane.X, axis.XYPlane.Y, axis.XYPlane.Z, unit);

      // check if vector is zero-length
      if (yAxis.IsZero) { return Plane.Unset; }
      // create unitised vector
      Vector3d yUnit = new Vector3d(yAxis);
      yUnit.Unitize();

      // check if x and y unitised are not the same
      if (xUnit.Equals(yUnit)) { return Plane.Unset; }

      // Create new plane with Rhino method
      Plane pln = new Plane(origin, xAxis, yAxis);

      return pln;
    }

    internal static Point3d Point3dFromNode(Node node, LengthUnit unit)
    {
      return (unit == LengthUnit.Meter) ?
          new Point3d(node.Position.X, node.Position.Y, node.Position.Z) : // skip unitsnet conversion, gsa api node always in meters
          new Point3d(new Length(node.Position.X, LengthUnit.Meter).As(unit),
                      new Length(node.Position.Y, LengthUnit.Meter).As(unit),
                      new Length(node.Position.Z, LengthUnit.Meter).As(unit));
    }
    internal static Point3d Point3dFromXYZUnit(double x, double y, double z, LengthUnit modelUnit)
    {
      return (modelUnit == LengthUnit.Meter) ?
          new Point3d(x, y, z) : // skip unitsnet conversion, gsa api node always in meters
          new Point3d(new Length(x, LengthUnit.Meter).As(modelUnit),
                      new Length(y, LengthUnit.Meter).As(modelUnit),
                      new Length(z, LengthUnit.Meter).As(modelUnit));
    }
    internal static Vector3d Vector3dFromXYZUnit(double x, double y, double z, LengthUnit modelUnit)
    {
      return (modelUnit == LengthUnit.Meter) ?
          new Vector3d(x, y, z) : // skip unitsnet conversion, gsa api node always in meters
          new Vector3d(new Length(x, LengthUnit.Meter).As(modelUnit),
                      new Length(y, LengthUnit.Meter).As(modelUnit),
                      new Length(z, LengthUnit.Meter).As(modelUnit));
    }
    internal static Node UpdateNodePositionUnit(Node node, LengthUnit unit)
    {
      if (unit != LengthUnit.Meter) // convert from meter to input unit if not meter
      {
        Vector3 pos = new Vector3();
        pos.X = new Length(node.Position.X, LengthUnit.Meter).As(unit);
        pos.Y = new Length(node.Position.Y, LengthUnit.Meter).As(unit);
        pos.Z = new Length(node.Position.Z, LengthUnit.Meter).As(unit);
        node.Position = pos;
      }
      return node;
    }
  }
}
