using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaCombinationCase" /> can be used in Grasshopper.
  /// </summary>
  public class GsaCombinationCaseGoo : GH_OasysGoo<GsaCombinationCase> {
    public static string Description =>
      "Combination cases are similar to analysis cases but differ \n" +
      "in two respects:\n" +
      " - Results for combination cases are inferred from analysis \n" +
      "case results and not calculated explicitly.\n" +
      " - Combination cases can be enveloping cases, as described \n" +
      "in Enveloping in GSA.";
    public static string Name => "Combination Case";
    public static string NickName => "ΣC";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaCombinationCaseGoo(GsaCombinationCase item) : base(item) { }
    public override bool CastTo<TQ>(ref TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        if (Value != null) {
          target = (TQ)(object)new GH_Integer(Value.Id);
          return true;
        }
      }

      target = default;
      return false;
    }
    public override IGH_Goo Duplicate() {
      return new GsaCombinationCaseGoo(Value);
    }
  }
}
