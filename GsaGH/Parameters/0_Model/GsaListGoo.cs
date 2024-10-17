using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaList" /> can be used in Grasshopper.
  /// </summary>
  public class GsaListGoo : GH_OasysGoo<GsaList> {
    public static string Description => "GSA Entity List";
    public static string Name => "List";
    public static string NickName => "L";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public GsaListGoo() { }
    public GsaListGoo(GsaList item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaListGoo(Value);
    }

    public override bool CastFrom(object source) {
      if (source == null) {
        return false;
      }

      if (GH_Convert.ToString(source, out string text, GH_Conversion.Both)) {
        Value = new GsaList() {
          EntityType = EntityType.Undefined,
          Definition = text
        };
        return true;
      }

      return false;
    }
  }
}
