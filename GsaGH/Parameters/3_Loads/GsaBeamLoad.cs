using System;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaBeamLoad : IGsaLoad {
    public BeamLoad BeamLoad { get; set; }
    public GsaLoadCase LoadCase { get; set; }
    public LoadType LoadType => LoadType.Beam;
    public ReferenceType ReferenceType { get; set; } = ReferenceType.None;
    public GsaList ReferenceList { get; set; }
    public Guid RefObjectGuid { get; set; }
    public int CaseId {
      get => BeamLoad.Case;
      set => BeamLoad.Case = value;
    }
    public string Name {
      get => BeamLoad.Name;
      set => BeamLoad.Name = value;
    }
    public GsaBeamLoad() {
      BeamLoad = new BeamLoad {
        Type = BeamLoadType.UNIFORM,
      };
    }

    public IGsaLoad Duplicate() {
      var dup = new GsaBeamLoad {
        BeamLoad = {
          AxisProperty = BeamLoad.AxisProperty,
          Case = BeamLoad.Case,
          Direction = BeamLoad.Direction,
          EntityList = BeamLoad.EntityList,
          EntityType = BeamLoad.EntityType,
          Name = BeamLoad.Name,
          IsProjected = BeamLoad.IsProjected,
          Type = BeamLoad.Type,
        },
      };
      switch (BeamLoad.Type) {
        case BeamLoadType.POINT:
          dup.BeamLoad.SetPosition(0, BeamLoad.Position(0));
          dup.BeamLoad.SetValue(0, BeamLoad.Value(0));
          break;

        case BeamLoadType.UNIFORM:
          dup.BeamLoad.SetValue(0, BeamLoad.Value(0));
          break;

        case BeamLoadType.LINEAR:
          dup.BeamLoad.SetValue(0, BeamLoad.Value(0));
          dup.BeamLoad.SetValue(1, BeamLoad.Value(1));
          break;

        case BeamLoadType.PATCH:
          dup.BeamLoad.SetPosition(0, BeamLoad.Position(0));
          dup.BeamLoad.SetPosition(1, BeamLoad.Position(1));
          dup.BeamLoad.SetValue(0, BeamLoad.Value(0));
          dup.BeamLoad.SetValue(1, BeamLoad.Value(1));
          break;

        case BeamLoadType.TRILINEAR:
          dup.BeamLoad.SetPosition(0, BeamLoad.Position(0));
          dup.BeamLoad.SetPosition(1, BeamLoad.Position(1));
          dup.BeamLoad.SetValue(0, BeamLoad.Value(0));
          dup.BeamLoad.SetValue(1, BeamLoad.Value(1));
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
