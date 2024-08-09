using System;

using GsaAPI;

using GsaGH.Parameters.Enums;

namespace GsaGH.Parameters {
  public class GsaGravityLoad : IGsaLoad {
    public GravityLoad ApiLoad { get; set; } = new GravityLoad();
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
    public GsaGravityLoad() {
      ApiLoad.Factor = new Vector3() {
        X = 0,
        Y = 0,
        Z = -1,
      };
      ApiLoad.Case = 1;
      ApiLoad.EntityList = "all";
      ApiLoad.Nodes = "all";
    }

    public IGsaLoad Duplicate() {
      var dup = new GsaGravityLoad {
        ApiLoad = {
          Case = ApiLoad.Case,
          EntityList = ApiLoad.EntityList.ToString(),
          EntityType = ApiLoad.EntityType,
          Nodes = ApiLoad.Nodes.ToString(),
          Name = ApiLoad.Name.ToString(),
          Factor = ApiLoad.Factor,
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
