using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaMaterial" /> can be used in Grasshopper.
  /// </summary>
  public class GsaMaterialGoo : GH_OasysGoo<GsaMaterial> {
    public static string Description => "GSA Material";
    public static string Name => "Material";
    public static string NickName => "Mat";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaMaterialGoo(GsaMaterial item) : base(item) { }

    public override bool CastTo<TQ>(ref TQ target) {
      if (base.CastTo(ref target)) {
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        if (Value == null) {
          target = default;
        } else {
          var ghint = new GH_Integer();
          int id = Value.GradeProperty;
          if (id == 0) {
            id = Value.AnalysisProperty;
          }

          if (GH_Convert.ToGHInteger(id, GH_Conversion.Both, ref ghint)) {
            target = (TQ)(object)ghint;
          } else {
            target = default;
          }
        }

        return true;
      }

      target = default;
      return false;
    }

    public override IGH_Goo Duplicate() {
      return new GsaMaterialGoo(Value);
    }
  }
}
