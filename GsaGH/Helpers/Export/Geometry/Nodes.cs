using GsaAPI;
using OasysUnits;
using Rhino.Geometry;
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
  }
}
