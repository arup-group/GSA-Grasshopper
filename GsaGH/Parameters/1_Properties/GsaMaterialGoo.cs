using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaMaterial"/> can be used in Grasshopper.
  /// </summary>
  public class GsaMaterialGoo : GH_OasysGoo<GsaMaterial> {
    public static string Name => "Material";
    public static string NickName => "Mat";
    public static string Description => "GSA Material";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaMaterialGoo(GsaMaterial item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaMaterialGoo(Value);

    public override bool CastTo<Q>(ref Q target) {
      if (base.CastTo<Q>(ref target))
        return true;

      if (typeof(Q).IsAssignableFrom(typeof(GsaProp3d))) {
        if (Value == null)
          target = default;
        else {
          target = (Q)(object)new GsaProp3d(Value);
        }
        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(GsaProp3dGoo))) {
        if (Value == null)
          target = default;
        else {
          target = (Q)(object)new GsaProp3dGoo(new GsaProp3d(Value));
        }
        return true;
      }

      return false;
    }

    public override bool CastFrom(object source) {
      if (source == null)
        return false;

      if (base.CastFrom(source))
        return true;

      // Cast from string
      if (GH_Convert.ToString(source, out string mat, GH_Conversion.Both)) {
        if (mat.ToUpper() == "STEEL") {
          Value.MaterialType = GsaMaterial.MatType.Steel;
          return true;
        }
        else if (mat.ToUpper() == "CONCRETE") {
          Value.MaterialType = GsaMaterial.MatType.Concrete;
          return true;
        }
        else if (mat.ToUpper() == "FRP") {
          Value.MaterialType = GsaMaterial.MatType.Frp;
          return true;
        }
        else if (mat.ToUpper() == "ALUMINIUM") {
          Value.MaterialType = GsaMaterial.MatType.Aluminium;
          return true;
        }
        else if (mat.ToUpper() == "TIMBER") {
          Value.MaterialType = GsaMaterial.MatType.Timber;
          return true;
        }
        else if (mat.ToUpper() == "GLASS") {
          Value.MaterialType = GsaMaterial.MatType.Glass;
          return true;
        }
        else if (mat.ToUpper() == "FABRIC") {
          Value.MaterialType = GsaMaterial.MatType.Fabric;
          return true;
        }
        else if (mat.ToUpper() == "GENERIC") {
          Value.MaterialType = GsaMaterial.MatType.Generic;
          return true;
        }
        return false;
      }

      // Cast from integer
      else if (GH_Convert.ToInt32(source, out int idd, GH_Conversion.Both)) {
        Value.GradeProperty = idd;
      }

      return false;
    }
  }
}
