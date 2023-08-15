using System;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaBeamThermalLoad : IGsaLoad {
    public BeamThermalLoad BeamThermalLoad { get; set; }
    public GsaLoadCase LoadCase { get; set; }
    public LoadType LoadType => LoadType.Thermal;
    public ReferenceType ReferenceType { get; set; } = ReferenceType.None;
    public GsaList ReferenceList { get; set; }
    public Guid RefObjectGuid { get; set; }
    public int CaseId {
      get => BeamThermalLoad.Case;
      set => BeamThermalLoad.Case = value;
    }
    public string Name {
      get => BeamThermalLoad.Name;
      set => BeamThermalLoad.Name = value;
    }
    public GsaBeamThermalLoad() {
      BeamThermalLoad = new BeamThermalLoad();
    }

    public IGsaLoad Duplicate() {
      var dup = new GsaBeamThermalLoad {
        BeamThermalLoad = {
          Case = BeamThermalLoad.Case,
          EntityList = BeamThermalLoad.EntityList,
          EntityType = BeamThermalLoad.EntityType,
          Name = BeamThermalLoad.Name,
          UniformTemperature = BeamThermalLoad.UniformTemperature
        },
      };

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
