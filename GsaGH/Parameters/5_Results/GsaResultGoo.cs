using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaResult" /> can be used in Grasshopper.
  /// </summary>
  public class GsaResultGoo : GH_OasysGoo<GsaResult> {
    public static string Description => "GSA Result";
    public static string Name => "Result";
    public static string NickName => "Res";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaResultGoo(GsaResult item) : base(item) { }

    public override bool CastTo<TQ>(ref TQ target) {
      if (base.CastTo(ref target)) {
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        if (Value == null) {
          target = default;
        } else {
          var ghint = new GH_Integer();
          if (GH_Convert.ToGHInteger(Value.CaseId, GH_Conversion.Both, ref ghint)) {
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
      return this;
    }
  }
}
