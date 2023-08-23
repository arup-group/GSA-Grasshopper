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
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        if (Value != null) {
          target = (TQ)(object)new GH_Integer(Value.CaseId);
          return true;
        }
      }

      target = default;
      return false;
    }

    public override IGH_Goo Duplicate() {
      return this;
    }
  }
}
