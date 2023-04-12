using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaMaterial"/> can be used in Grasshopper.
  /// </summary>
  public class GsaMaterialGoo : GH_OasysGoo<GsaMaterial> {
    public static string Description => "GSA Material";
    public static string Name => "Material";
    public static string NickName => "Mat";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaMaterialGoo(GsaMaterial item) : base(item) { }

    public override bool CastFrom(object source) {
      if (source == null) {
        return false;
      }

      if (base.CastFrom(source)) {
        return true;
      }

      if (GH_Convert.ToString(source, out string mat, GH_Conversion.Both)) {
        switch (mat.ToUpper()) {
          case "STEEL":
            Value.MaterialType = GsaMaterial.MatType.Steel;
            return true;

          case "CONCRETE":
            Value.MaterialType = GsaMaterial.MatType.Concrete;
            return true;

          case "FRP":
            Value.MaterialType = GsaMaterial.MatType.Frp;
            return true;

          case "ALUMINIUM":
            Value.MaterialType = GsaMaterial.MatType.Aluminium;
            return true;

          case "TIMBER":
            Value.MaterialType = GsaMaterial.MatType.Timber;
            return true;

          case "GLASS":
            Value.MaterialType = GsaMaterial.MatType.Glass;
            return true;

          case "FABRIC":
            Value.MaterialType = GsaMaterial.MatType.Fabric;
            return true;

          case "GENERIC":
            Value.MaterialType = GsaMaterial.MatType.Generic;
            return true;

          default:
            return false;
        }
      }

      if (GH_Convert.ToInt32(source, out int idd, GH_Conversion.Both)) {
        Value.GradeProperty = idd;
      }

      return false;
    }

    public override bool CastTo<TQ>(ref TQ target) {
      if (base.CastTo(ref target)) {
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GsaProp3d))) {
        if (Value == null) {
          target = default;
        }
        else {
          target = (TQ)(object)new GsaProp3d(Value);
        }
        return true;
      }

      if (!typeof(TQ).IsAssignableFrom(typeof(GsaProp3dGoo))) {
        return false;
      }

      if (Value == null) {
        target = default;
      }
      else {
        target = (TQ)(object)new GsaProp3dGoo(new GsaProp3d(Value));
      }
      return true;
    }

    public override IGH_Goo Duplicate() {
      return new GsaMaterialGoo(Value);
    }
  }
}
