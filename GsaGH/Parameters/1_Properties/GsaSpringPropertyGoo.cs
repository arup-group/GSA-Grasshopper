using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaSpringProperty" /> can be used in Grasshopper.
  /// </summary>
  public class GsaSpringPropertyGoo : GH_OasysGoo<GsaSpringProperty> {
    public static string Description => "GSA Spring Property";
    public static string Name => "Spring Property";
    public static string NickName => "PS";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaSpringPropertyGoo(GsaSpringProperty item) : base(item) { }

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
      return new GsaSpringPropertyGoo(Value);
    }
  }
}
