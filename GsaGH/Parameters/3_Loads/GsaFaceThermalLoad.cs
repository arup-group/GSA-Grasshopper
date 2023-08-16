using System;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaFaceThermalLoad : IGsaLoad {
    public FaceThermalLoad FaceThermalLoad { get; set; }
    public GsaLoadCase LoadCase { get; set; }
    public LoadType LoadType => LoadType.FaceThermal;
    public ReferenceType ReferenceType { get; set; } = ReferenceType.None;
    public GsaList ReferenceList { get; set; }
    public Guid RefObjectGuid { get; set; }
    public int CaseId {
      get => FaceThermalLoad.Case;
      set => FaceThermalLoad.Case = value;
    }
    public string Name {
      get => FaceThermalLoad.Name;
      set => FaceThermalLoad.Name = value;
    }
    public GsaFaceThermalLoad() {
      FaceThermalLoad = new FaceThermalLoad ();
    }

    public IGsaLoad Duplicate() {
      var dup = new GsaFaceThermalLoad {
        FaceThermalLoad = {
          Case = FaceThermalLoad.Case,
          EntityList = FaceThermalLoad.EntityList,
          EntityType = FaceThermalLoad.EntityType,
          Name = FaceThermalLoad.Name,
          UniformTemperature = FaceThermalLoad.UniformTemperature
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
