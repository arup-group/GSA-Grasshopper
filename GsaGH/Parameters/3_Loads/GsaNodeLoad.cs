using System;
using GsaAPI;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  public class GsaNodeLoad : IGsaLoad {
    public enum NodeLoadType // direct copy from GSA API enums
    {
      NodeLoad = 0,
      AppliedDisp = 1,
      Settlement = 2,
      Gravity = 3,
      NumTypes = 4,
    }

    public NodeLoad NodeLoad { get; set; } = new NodeLoad();
    public NodeLoadType Type;
    public GsaLoadCase LoadCase { get; set; }
    public LoadType LoadType => LoadType.Node;
    public ReferenceType ReferenceType { get; set; } = ReferenceType.None;
    public GsaList ReferenceList { get; set; }

    public Guid RefObjectGuid => throw new NotImplementedException();

    internal Point3d _refPoint = Point3d.Unset;

    public GsaNodeLoad() {
      Type = NodeLoadType.NodeLoad;
    }

    public int CaseId() {
      return NodeLoad.Case;
    }

    public IGsaLoad Duplicate() {
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

      if (LoadCase != null) {
        dup.LoadCase = LoadCase;
      }

      if (_refPoint != Point3d.Unset) {
        dup._refPoint = new Point3d(_refPoint);
      }

      if (ReferenceList != null) {
        dup.ReferenceList = ReferenceList.Duplicate();
      }

      return dup;
    }

    public override string ToString() {
      return string.Join(" ", LoadType.ToString().Trim(), NodeLoad.Name.Trim()).Trim().Replace("  ", " ");
    }
  }
}
