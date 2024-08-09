using System;

using GsaAPI;

using GsaGH.Parameters.Enums;

namespace GsaGH.Parameters {
  public class GsaFaceThermalLoad : IGsaLoad {
    public FaceThermalLoad ApiLoad { get; set; }
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
    public GsaFaceThermalLoad() {
      ApiLoad = new FaceThermalLoad();
    }

    public IGsaLoad Duplicate() {
      var dup = new GsaFaceThermalLoad {
        ApiLoad = {
          Case = ApiLoad.Case,
          EntityList = ApiLoad.EntityList,
          EntityType = ApiLoad.EntityType,
          Name = ApiLoad.Name,
          UniformTemperature = ApiLoad.UniformTemperature
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
