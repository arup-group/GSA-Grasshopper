using GsaAPI;
using System;

namespace GsaGH.Parameters {
  public class GsaGravityLoad : IGsaLoad {
    public GravityLoad GravityLoad { get; set; } = new GravityLoad();
    public LoadType LoadType => LoadType.Gravity;
    public ReferenceType ReferenceType { get; set; } = ReferenceType.None;
    public GsaList ReferenceList { get; set; }
    public Guid RefObjectGuid { get; set; }

    public GsaGravityLoad() {
      GravityLoad.Factor = new Vector3() {
        X = 0,
        Y = 0,
        Z = -1,
      };
      GravityLoad.Case = 1;
      GravityLoad.Elements = "all";
      GravityLoad.Nodes = "all";
    }

    public int CaseId() {
      return GravityLoad.Case;
    }

    public IGsaLoad Duplicate() {
      var dup = new GsaGravityLoad {
        GravityLoad = {
          Case = GravityLoad.Case,
          Elements = GravityLoad.Elements.ToString(),
          Nodes = GravityLoad.Nodes.ToString(),
          Name = GravityLoad.Name.ToString(),
          Factor = GravityLoad.Factor,
        },
      };
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

    public override string ToString() {
      if (LoadType == LoadType.Gravity && GravityLoad == null) {
        return "Null";
      }

      return string.Join(" ", LoadType.ToString().Trim(), GravityLoad.Name.Trim()).Trim().Replace("  ", " ");
    }
  }
}
