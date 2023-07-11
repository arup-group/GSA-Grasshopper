using GsaAPI;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  public class GsaNodeLoad {
    public enum NodeLoadTypes // direct copy from GSA API enums
    {
      NodeLoad = 0,
      AppliedDisp = 1,
      Settlement = 2,
      Gravity = 3,
      NumTypes = 4,
    }

    public NodeLoad NodeLoad { get; set; } = new NodeLoad();
    public NodeLoadTypes Type;
    internal GsaList _refList;
    internal Point3d _refPoint = Point3d.Unset;

    public GsaNodeLoad() {
      Type = NodeLoadTypes.NodeLoad;
    }

    public GsaNodeLoad Duplicate() {
      var dup = new GsaNodeLoad {
        NodeLoad = {
          AxisProperty = NodeLoad.AxisProperty,
          Case = NodeLoad.Case,
          Direction = NodeLoad.Direction,
          Nodes = NodeLoad.Nodes.ToString(),
          Name = NodeLoad.Name.ToString(),
          Value = NodeLoad.Value,
        },
        Type = Type,
      };
      if (_refPoint != Point3d.Unset) {
        dup._refPoint = new Point3d(_refPoint);
      }

      if (_refList != null) {
        dup._refList = _refList.Duplicate();
      }

      return dup;
    }
  }
}
