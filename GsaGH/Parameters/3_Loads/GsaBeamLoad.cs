using System;

using GsaAPI;

using GsaGH.Parameters.Enums;

namespace GsaGH.Parameters {
  public class GsaBeamLoad : IGsaLoad {
    public BeamLoad ApiLoad { get; set; }
    public GsaLoadCase LoadCase { get; set; }
    public ReferenceType ReferenceType { get; set; } = ReferenceType.None;
    public GsaList ReferenceList { get; set; }
    public Guid RefObjectGuid { get; set; }
    public int CaseId {
      get => ApiLoad.Case;
      set => ApiLoad.Case = value;
    }
    public string Name {
      get => ApiLoad.Name;
      set => ApiLoad.Name = value;
    }
    public GsaBeamLoad() {
      ApiLoad = new BeamLoad {
        Type = BeamLoadType.UNIFORM,
        Direction = Direction.Z,
      };
    }

    public IGsaLoad Duplicate() {
      var dup = new GsaBeamLoad {
        ApiLoad = {
          AxisProperty = ApiLoad.AxisProperty,
          Case = ApiLoad.Case,
          Direction = ApiLoad.Direction,
          EntityList = ApiLoad.EntityList,
          EntityType = ApiLoad.EntityType,
          Name = ApiLoad.Name,
          IsProjected = ApiLoad.IsProjected,
          Type = ApiLoad.Type,
        },
      };
      switch (ApiLoad.Type) {
        case BeamLoadType.POINT:
          dup.ApiLoad.SetPosition(0, ApiLoad.Position(0));
          dup.ApiLoad.SetValue(0, ApiLoad.Value(0));
          break;

        case BeamLoadType.UNIFORM:
          dup.ApiLoad.SetValue(0, ApiLoad.Value(0));
          break;

        case BeamLoadType.LINEAR:
          dup.ApiLoad.SetValue(0, ApiLoad.Value(0));
          dup.ApiLoad.SetValue(1, ApiLoad.Value(1));
          break;

        case BeamLoadType.PATCH:
          dup.ApiLoad.SetPosition(0, ApiLoad.Position(0));
          dup.ApiLoad.SetPosition(1, ApiLoad.Position(1));
          dup.ApiLoad.SetValue(0, ApiLoad.Value(0));
          dup.ApiLoad.SetValue(1, ApiLoad.Value(1));
          break;

        case BeamLoadType.TRILINEAR:
          dup.ApiLoad.SetPosition(0, ApiLoad.Position(0));
          dup.ApiLoad.SetPosition(1, ApiLoad.Position(1));
          dup.ApiLoad.SetValue(0, ApiLoad.Value(0));
          dup.ApiLoad.SetValue(1, ApiLoad.Value(1));
          break;
      }

      if (LoadCase != null) {
        dup.LoadCase = LoadCase;
      }

      if (ReferenceType == ReferenceType.None) {
        return dup;
      }

      if (ReferenceType == ReferenceType.List) {
        dup.ReferenceType = ReferenceType.List;
        dup.ReferenceList = ReferenceList.Duplicate();
      } else {
        dup.RefObjectGuid = new Guid(RefObjectGuid.ToString());
        dup.ReferenceType = ReferenceType;
      }

      return dup;
    }
  }
}
