using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="IGsaMaterial" /> can be used in Grasshopper.
  /// </summary>
  public class GsaMaterialGoo : GH_OasysGoo<IGsaMaterial> {
    public static string Description => "GSA Material";
    public static string Name => "Material";
    public static string NickName => "Mat";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaMaterialGoo(IGsaMaterial item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaMaterialGoo(Value);
    }
  }
}
