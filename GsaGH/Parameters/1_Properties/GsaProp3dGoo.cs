using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaProp3d" /> can be used in Grasshopper.
  /// </summary>
  public class GsaProp3dGoo : GH_OasysGoo<GsaProp3d> {
    public static string Description => "GSA 3D Property (Volumetric)";
    public static string Name => "Prop3d";
    public static string NickName => "PV";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaProp3dGoo(GsaProp3d item) : base(item) { }

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
      return new GsaProp3dGoo(Value);
    }
  }
}
