using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.Export {
  internal class Nodes {

    internal static int AddNode(
      ref GsaIntDictionary<Node> existNodes, Point3d testPoint, LengthUnit unit) {
      return existNodes.AddValue(NodeFromPoint(testPoint, unit));
    }

    internal static int AddNode(ref GsaIntDictionary<Node> existNodes, Node node) {
      return existNodes.AddValue(node);
    }

    internal static Node NodeFromPoint(Point3d point, LengthUnit unit) {
      if (unit == LengthUnit.Meter) {
        var pos = new Vector3() {
          X = point.X,
          Y = point.Y,
          Z = point.Z,
        };
        return new Node() {
          Position = pos,
        };
      } else {
        var pos = new Vector3() {
          X = new Length(point.X, unit).Meters,
          Y = new Length(point.Y, unit).Meters,
          Z = new Length(point.Z, unit).Meters,
        };
        return new Node() {
          Position = pos,
        };
      }
    }

    internal static void ConvertNode(
      GsaNode node, ref GsaIntDictionary<Node> apiNodes,
      ref GsaIntDictionary<Axis> apiAxes, LengthUnit unit) {
      Node apiNode = node.GetApiNodeToUnit(unit);

      if (!node.IsGlobalAxis) {
        var ax = new Axis();
        Plane pln = node.LocalAxis;
        ax.Origin.X = (unit == LengthUnit.Meter) ? pln.OriginX :
          new Length(pln.OriginX, unit).Meters;
        ax.Origin.Y = (unit == LengthUnit.Meter) ? pln.OriginY :
          new Length(pln.OriginY, unit).Meters;
        ax.Origin.Z = (unit == LengthUnit.Meter) ? pln.OriginZ :
          new Length(pln.OriginZ, unit).Meters;

        ax.XVector.X = pln.XAxis.X;
        ax.XVector.Y = pln.XAxis.Y;
        ax.XVector.Z = pln.XAxis.Z;
        ax.XYPlane.X = pln.YAxis.X;
        ax.XYPlane.Y = pln.YAxis.Y;
        ax.XYPlane.Z = pln.YAxis.Z;

        apiNode.AxisProperty = Axes.TryGetExistingAxisId(ref apiAxes, ax);
      }

      if (
        // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
        node.Id > 0) 
      {
        apiNodes.SetValue(node.Id, apiNode);
      } else {
        AddNode(ref apiNodes, apiNode);
      }
    }

    internal static void ConvertNodes(
      List<GsaNode> nodes, ref GsaIntDictionary<Node> existingNodes,
      ref GsaIntDictionary<Axis> apiAxes, LengthUnit modelUnit) {
      if (nodes == null || nodes.Count <= 0) {
        return;
      }

      nodes = nodes.OrderByDescending(n => n.Id).ToList();
      foreach (GsaNode node in nodes.Where(node => node != null)) {
        ConvertNode(node, ref existingNodes, ref apiAxes, modelUnit);
      }
    }
  }
}
