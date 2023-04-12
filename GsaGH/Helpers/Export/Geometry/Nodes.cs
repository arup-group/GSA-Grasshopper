using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Helpers.Export {
  internal class Nodes {

    internal static int AddNode(ref GsaIntDictionary<Node> existNodes, Point3d testPoint, LengthUnit unit) => existNodes.AddValue(NodeFromPoint(testPoint, unit));

    internal static int AddNode(ref GsaIntDictionary<Node> existNodes, Node node) => existNodes.AddValue(node);

    /// <summary>
    /// Method to convert a GsaNode to GsaAPI.Node
    /// </summary>
    /// <param name="node">GsaNode to convert</param>
    /// <param name="existingNodes">Dictionary of existing GsaAPI nodes to add nodes to [in meters]</param>
    /// <param name="existingAxes">Dictionary of existing GsaAPI axes to add local axis to [in meters]</param>
    /// <param name="nodeidcounter">node id counter for </param>
    /// <param name="unit">UnitsNet LengthUnit of GsaNode node input</param>
    internal static void ConvertNode(GsaNode node, ref GsaIntDictionary<Node> existingNodes, ref Dictionary<int, Axis> existingAxes, LengthUnit unit) {
      Node apiNode = node.GetApiNodeToUnit(unit);

      if (!node.IsGlobalAxis()) {
        var ax = new Axis();
        Plane pln = node.LocalAxis;
        ax.Origin.X = (unit == LengthUnit.Meter) ? pln.OriginX : new Length(pln.OriginX, unit).Meters;
        ax.Origin.Y = (unit == LengthUnit.Meter) ? pln.OriginY : new Length(pln.OriginY, unit).Meters;
        ax.Origin.Z = (unit == LengthUnit.Meter) ? pln.OriginZ : new Length(pln.OriginZ, unit).Meters;

        ax.XVector.X = pln.XAxis.X;
        ax.XVector.Y = pln.XAxis.Y;
        ax.XVector.Z = pln.XAxis.Z;
        ax.XYPlane.X = pln.YAxis.X;
        ax.XYPlane.Y = pln.YAxis.Y;
        ax.XYPlane.Z = pln.YAxis.Z;

        apiNode.AxisProperty = Axes.AddAxis(ref existingAxes, ax);
      }

      if (node.Id > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
        existingNodes.SetValue(node.Id, apiNode);
      else
        AddNode(ref existingNodes, apiNode);
    }

    internal static void ConvertNodes(List<GsaNode> nodes, ref GsaIntDictionary<Node> existingNodes,
                    ref Dictionary<int, Axis> existingAxes, LengthUnit modelUnit) {
      if (nodes == null || nodes.Count <= 0) {
        return;
      }

      nodes = nodes.OrderByDescending(n => n.Id).ToList();
      foreach (GsaNode node in nodes.Where(node => node != null))
        ConvertNode(node, ref existingNodes, ref existingAxes, modelUnit);
    }

    internal static Node NodeFromPoint(Point3d point, LengthUnit unit) {
      if (unit == LengthUnit.Meter) {
        var pos = new Vector3() { X = point.X, Y = point.Y, Z = point.Z };
        return new Node() { Position = pos };
      }
      else {
        var pos = new Vector3() {
          X = new Length(point.X, unit).Meters,
          Y = new Length(point.Y, unit).Meters,
          Z = new Length(point.Z, unit).Meters
        };
        return new Node() { Position = pos };
      }
    }
  }
}
