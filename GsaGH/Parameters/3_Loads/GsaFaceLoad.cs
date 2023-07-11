using GsaAPI;
using System;

namespace GsaGH.Parameters {
  public class GsaFaceLoad : IGsaLoad {
    public FaceLoad FaceLoad { get; set; }
    public LoadType LoadType => LoadType.Face;
    public ReferenceType ReferenceType { get; set; } = ReferenceType.None;
    public GsaList ReferenceList { get; set; }
    public Guid RefObjectGuid { get; set; }

    public GsaFaceLoad() {
      FaceLoad = new FaceLoad {
        Type = FaceLoadType.CONSTANT,
      };
    }

    public int CaseId() {
      return FaceLoad.Case;
    }

    public IGsaLoad Duplicate() {
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
      return string.Join(" ", LoadType.ToString().Trim(), FaceLoad.Name.Trim()).Trim().Replace("  ", " ");
    }
  }
}
