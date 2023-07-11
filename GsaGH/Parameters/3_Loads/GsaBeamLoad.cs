using GsaAPI;
using System;

namespace GsaGH.Parameters {
  public class GsaBeamLoad {
    public BeamLoad BeamLoad { get; set; }
    internal ReferenceType _referenceType = ReferenceType.None;
    internal GsaList _refList;
    internal Guid _refObjectGuid;

    public GsaBeamLoad() {
      BeamLoad = new BeamLoad {
        Type = BeamLoadType.UNIFORM,
      };
    }

    public GsaBeamLoad Duplicate() {
      var dup = new GsaBeamLoad {
        BeamLoad = {
          AxisProperty = BeamLoad.AxisProperty,
          Case = BeamLoad.Case,
          Direction = BeamLoad.Direction,
          Elements = BeamLoad.Elements.ToString(),
          Name = BeamLoad.Name.ToString(),
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
