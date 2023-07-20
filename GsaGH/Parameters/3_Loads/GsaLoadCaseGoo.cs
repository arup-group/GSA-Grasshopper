using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaLoadCase" /> can be used in Grasshopper.
  /// </summary>
  public class GsaLoadCaseGoo : GH_OasysGoo<GsaLoadCase> {
    public static string Description => "GSA Load Case";
    public static string Name => "LoadCase";
    public static string NickName => "LC";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaLoadCaseGoo(GsaLoadCase item) : base(item) { }

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
      return new GsaLoadCaseGoo(Value);
    }
  }
}
