using GsaAPI;
using System;

namespace GsaGH.Parameters {
  public class GsaGravityLoad {
    public GravityLoad GravityLoad { get; set; } = new GravityLoad();
    internal ReferenceType _referenceType = ReferenceType.None;
    internal GsaList _refList;
    internal Guid _refObjectGuid;

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

    public GsaGravityLoad Duplicate() {
      var dup = new GsaGravityLoad {
        GravityLoad = {
          Case = GravityLoad.Case,
          Elements = GravityLoad.Elements.ToString(),
          Nodes = GravityLoad.Nodes.ToString(),
          Name = GravityLoad.Name.ToString(),
          Factor = GravityLoad.Factor,
        },
      };
      if (_referenceType == ReferenceType.None) {
        return dup;
      }

      if (_referenceType == ReferenceType.List) {
        dup._referenceType = ReferenceType.List;
        dup._refList = _refList.Duplicate();
      } else {
        dup._refObjectGuid = new Guid(_refObjectGuid.ToString());
        dup._referenceType = _referenceType;
      }

      return dup;
    }
  }
}
