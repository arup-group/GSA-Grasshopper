using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaProp2d" /> can be used in Grasshopper.
  /// </summary>
  public class GsaProp2dGoo : GH_OasysGoo<GsaProp2d> {
    public static string Description => "GSA 2D Property (Area)";
    public static string Name => "Prop2d";
    public static string NickName => "PA";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaProp2dGoo(GsaProp2d item) : base(item) { }

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
      return new GsaProp2dGoo(Value);
    }
  }
}
