using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaSection" /> can be used in Grasshopper.
  /// </summary>
  public class GsaSectionGoo : GH_OasysGoo<GsaSection> {
    public static string Description => "GSA Beam Property";
    public static string Name => "Section";
    public static string NickName => "PB";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaSectionGoo(GsaSection item) : base(item) { }

    public override bool CastFrom(object source) {
      if (source == null) {
        return false;
      }

      if (base.CastFrom(source)) {
        return true;
      }

      if (GH_Convert.ToString(source, out string name, GH_Conversion.Both)) {
        if (GsaSection.ValidProfile(name)) {
          Value = new GsaSection(name);
          return true;
        }
      }

      if (!GH_Convert.ToInt32(source, out int idd, GH_Conversion.Both)) {
        return false;
      }

      Value.Id = idd;
      return true;
    }

    public override bool CastTo<TQ>(ref TQ target) {
      if (base.CastTo(ref target)) {
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        if (Value == null) {
          target = default;
        } else {
          var ghint = new GH_Integer();
          if (GH_Convert.ToGHInteger(Value.Id, GH_Conversion.Both, ref ghint)) {
            target = (TQ)(object)ghint;
          } else {
            target = default;
          }
        }

        return true;
      }

      target = default;
      return false;
    }

    public override IGH_Goo Duplicate() {
      return new GsaSectionGoo(Value);
    }
  }
}
