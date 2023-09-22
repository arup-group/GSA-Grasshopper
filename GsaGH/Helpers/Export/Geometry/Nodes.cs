using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.Export {
  internal partial class ModelAssembly {

    internal int AddNode(Point3d testPoint) {
      return Nodes.AddValue(NodeFromPoint(testPoint, Unit));
    }

    internal int AddNode(Node node) {
      return Nodes.AddValue(node);
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

    internal void ConvertNode(GsaNode node) {
      Node apiNode = node.GetApiNodeToUnit(Unit);

      if (!node.IsGlobalAxis) {
        var ax = new Axis();
        Plane pln = node.LocalAxis;
        ax.Origin.X = (Unit == LengthUnit.Meter) ? pln.OriginX :
          new Length(pln.OriginX, Unit).Meters;
        ax.Origin.Y = (Unit == LengthUnit.Meter) ? pln.OriginY :
          new Length(pln.OriginY, Unit).Meters;
        ax.Origin.Z = (Unit == LengthUnit.Meter) ? pln.OriginZ :
          new Length(pln.OriginZ, Unit).Meters;

        ax.XVector.X = pln.XAxis.X;
        ax.XVector.Y = pln.XAxis.Y;
        ax.XVector.Z = pln.XAxis.Z;
        ax.XYPlane.X = pln.YAxis.X;
        ax.XYPlane.Y = pln.YAxis.Y;
        ax.XYPlane.Z = pln.YAxis.Z;

        apiNode.AxisProperty = TryGetExistingAxisId(ax);
      }

      if (
        // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
        node.Id > 0) {
        Nodes.SetValue(node.Id, apiNode);
      } else {
        AddNode(apiNode);
      }
    }

    internal void ConvertNodes(List<GsaNode> nodes) {
      if (nodes == null || nodes.Count <= 0) {
        return;
      }

      nodes = nodes.OrderByDescending(n => n.Id).ToList();
      foreach (GsaNode node in nodes.Where(node => node != null)) {
        ConvertNode(node);
      }

      Nodes.UpdateFirstEmptyKeyToMaxKey();
    }
  }
}
