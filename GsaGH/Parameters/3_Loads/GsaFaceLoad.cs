using GsaAPI;
using System;

namespace GsaGH.Parameters {
  public class GsaFaceLoad {
    public FaceLoad FaceLoad { get; set; }
    internal ReferenceType _referenceType = ReferenceType.None;
    internal GsaList _refList;
    internal Guid _refObjectGuid;

    public GsaFaceLoad() {
      FaceLoad = new FaceLoad {
        Type = FaceLoadType.CONSTANT,
      };
    }

    public GsaFaceLoad Duplicate() {
      var dup = new GsaFaceLoad {
        FaceLoad = {
          AxisProperty = FaceLoad.AxisProperty,
          Case = FaceLoad.Case,
          Direction = FaceLoad.Direction,
          Elements = FaceLoad.Elements.ToString(),
          Name = FaceLoad.Name.ToString(),
          Type = FaceLoad.Type,
        },
      };
      switch (FaceLoad.Type) {
        case FaceLoadType.CONSTANT:
          dup.FaceLoad.IsProjected = FaceLoad.IsProjected;
          dup.FaceLoad.SetValue(0, FaceLoad.Value(0));
          break;

        case FaceLoadType.GENERAL:
          dup.FaceLoad.IsProjected = FaceLoad.IsProjected;
          dup.FaceLoad.SetValue(0, FaceLoad.Value(0));
          dup.FaceLoad.SetValue(1, FaceLoad.Value(1));
          dup.FaceLoad.SetValue(2, FaceLoad.Value(2));
          dup.FaceLoad.SetValue(3, FaceLoad.Value(3));
          break;

        case FaceLoadType.POINT:
          dup.FaceLoad.IsProjected = FaceLoad.IsProjected;
          dup.FaceLoad.SetValue(0, FaceLoad.Value(0));
          dup.FaceLoad.Position = FaceLoad.Position; // todo
          //note Vector2 currently only get in GsaAPI
          // duplicate Position.X and Position.Y when fixed
          break;
      }

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
