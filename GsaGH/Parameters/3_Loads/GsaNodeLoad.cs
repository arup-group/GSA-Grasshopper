using System;

using GsaAPI;

using GsaGH.Parameters.Enums;

using Rhino.Geometry;

namespace GsaGH.Parameters {
  public class GsaNodeLoad : IGsaLoad {
    public NodeLoad ApiLoad { get; set; }
    public NodeLoadType Type { get; set; }
    public GsaLoadCase LoadCase { get; set; }
    public ReferenceType ReferenceType { get; set; } = ReferenceType.None;
    public GsaList ReferenceList { get; set; }

    public Guid RefObjectGuid => throw new NotImplementedException();

    public int CaseId {
      get => ApiLoad.Case;
      set => ApiLoad.Case = value;
    }
    public string Name {
      get => ApiLoad.Name;
      set => ApiLoad.Name = value;
    }

    internal Point3d _refPoint = Point3d.Unset;

    public GsaNodeLoad() {
      ApiLoad = new NodeLoad() {
        Direction = Direction.Z,
      };
      Type = NodeLoadType.NodeLoad;

    }

    public IGsaLoad Duplicate() {
      var dup = new GsaNodeLoad {
        ApiLoad = {
          AxisProperty = ApiLoad.AxisProperty,
          Case = ApiLoad.Case,
          Direction = ApiLoad.Direction,
          Nodes = ApiLoad.Nodes.ToString(),
          Name = ApiLoad.Name.ToString(),
          Value = ApiLoad.Value,
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
  }
}
