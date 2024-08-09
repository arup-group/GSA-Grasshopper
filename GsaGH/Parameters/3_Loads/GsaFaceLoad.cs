using System;

using GsaAPI;

using GsaGH.Parameters.Enums;

namespace GsaGH.Parameters {
  public class GsaFaceLoad : IGsaLoad {
    public FaceLoad ApiLoad { get; set; }
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
    public GsaFaceLoad() {
      ApiLoad = new FaceLoad {
        Type = FaceLoadType.CONSTANT,
        Direction = Direction.Z,
      };
    }

    public IGsaLoad Duplicate() {
      var dup = new GsaFaceLoad {
        ApiLoad = {
          AxisProperty = ApiLoad.AxisProperty,
          Case = ApiLoad.Case,
          Direction = ApiLoad.Direction,
          EntityList = ApiLoad.EntityList,
          EntityType = ApiLoad.EntityType,
          Name = ApiLoad.Name,
          Type = ApiLoad.Type,
        },
      };
      switch (ApiLoad.Type) {
        case FaceLoadType.CONSTANT:
          dup.ApiLoad.IsProjected = ApiLoad.IsProjected;
          dup.ApiLoad.SetValue(0, ApiLoad.Value(0));
          break;

        case FaceLoadType.GENERAL:
          dup.ApiLoad.IsProjected = ApiLoad.IsProjected;
          dup.ApiLoad.SetValue(0, ApiLoad.Value(0));
          dup.ApiLoad.SetValue(1, ApiLoad.Value(1));
          dup.ApiLoad.SetValue(2, ApiLoad.Value(2));
          dup.ApiLoad.SetValue(3, ApiLoad.Value(3));
          break;

        case FaceLoadType.POINT:
          dup.ApiLoad.IsProjected = ApiLoad.IsProjected;
          dup.ApiLoad.SetValue(0, ApiLoad.Value(0));
          dup.ApiLoad.Position = new Vector2(ApiLoad.Position.X, ApiLoad.Position.Y);
          break;

        case FaceLoadType.EQUATION:
          dup.ApiLoad.IsProjected = ApiLoad.IsProjected;
          dup.ApiLoad.SetEquation(ApiLoad.Equation());
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
