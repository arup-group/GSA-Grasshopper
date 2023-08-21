using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaMaterial" /> can be used in Grasshopper.
  /// </summary>
  public class GsaMaterialGoo : GH_OasysGoo<GsaMaterial> {
    public static string Description =>
      "A Material is used by Sections, 2D properties or 3D properties \n" +
      "in order to analyse the Model. In Grasshopper it is only \n" +
      "possible to work with elastic isotropic material types. \n" +
      "A Material can either be created as a Standard Material \n" +
      "from design code and grade, or as a custom material.";
    public static string Name => "Material";
    public static string NickName => "Mat";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaMaterialGoo(GsaMaterial item) : base(item) { }

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
      return new GsaMaterialGoo(Value);
    }
  }
}
