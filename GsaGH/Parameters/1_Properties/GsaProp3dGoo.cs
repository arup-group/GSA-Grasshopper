using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaProp3d"/> can be used in Grasshopper.
  /// </summary>
  public class GsaProp3dGoo : GH_OasysGoo<GsaProp3d> {
    public static string Description => "GSA Volume Property";
    public static string Name => "Prop3D";
    public static string NickName => "PV";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaProp3dGoo(GsaProp3d item) : base(item) { }

    public override bool CastFrom(object source) {
      if (source == null)
        return false;

      if (base.CastFrom(source))
        return true;

      if (source.GetType().IsAssignableFrom(typeof(GsaMaterial))) {
        Value = new GsaProp3d((GsaMaterial)source);
        return true;
      }

      if (!source.GetType().IsAssignableFrom(typeof(GsaMaterialGoo))) {
        return false;
      }

      Value = new GsaProp3d(((GsaMaterialGoo)source).Value);
      return true;
    }

    public override bool CastTo<TQ>(ref TQ target) {
      if (base.CastTo(ref target))
        return true;

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        if (Value == null)
          target = default;
        else {
          var ghint = new GH_Integer();
          if (GH_Convert.ToGHInteger(Value.Id, GH_Conversion.Both, ref ghint))
            target = (TQ)(object)ghint;
          else
            target = default;
        }
        return true;
      }

      target = default;
      return false;
    }

    public override IGH_Goo Duplicate() => new GsaProp3dGoo(Value);
  }
}
