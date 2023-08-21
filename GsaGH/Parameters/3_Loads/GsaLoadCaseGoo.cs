using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaLoadCase" /> can be used in Grasshopper.
  /// </summary>
  public class GsaLoadCaseGoo : GH_OasysGoo<GsaLoadCase> {
    public static string Description =>
      "When loads are applied to the model they are assigned to a load case. \n" +
      "Load cases are a convenient way of grouping together a collection of \n" +
      "loads that are to be considered together. Thus a load case may be set \n" +
      "up to contain all the loads associated with dead loads, another to \n" +
      "contain all the loads associated with live loads etc.\n" +
      "In GSA the load cases are only used to group loading and are not used \n" +
      "directly for analysis.";
    public static string Name => "LoadCase";
    public static string NickName => "LC";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaLoadCaseGoo() { }
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

    public override bool CastFrom(object source) {
      if (source == null) {
        return false;
      }

      if (GH_Convert.ToInt32(source, out int id, GH_Conversion.Both)) {
        Value = new GsaLoadCase(id);
        return true;
      }

      return false;
    }

    public override IGH_Goo Duplicate() {
      return new GsaLoadCaseGoo(Value);
    }
  }
}
