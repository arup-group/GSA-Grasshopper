using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaProperty3d" /> can be used in Grasshopper.
  /// </summary>
  public class GsaProperty3dGoo : GH_OasysGoo<GsaProperty3d> {
    public static string Description => "GSA 3D Property (Volumetric)";
    public static string Name => "Property 3D";
    public static string NickName => "PV";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaProperty3dGoo(GsaProperty3d item) : base(item) { }

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
      return new GsaProperty3dGoo(Value);
    }
  }
}
