using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaProperty2d" /> can be used in Grasshopper.
  /// </summary>
  public class GsaProperty2dGoo : GH_OasysGoo<GsaProperty2d> {
    public static string Description => "GSA 2D Property (Area)";
    public static string Name => "Property 2D";
    public static string NickName => "PA";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaProperty2dGoo(GsaProperty2d item) : base(item) { }

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
      return new GsaProperty2dGoo(Value);
    }
  }
}
