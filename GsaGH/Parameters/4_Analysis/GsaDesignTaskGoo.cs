using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="IGsaDesignTask" /> can be used in Grasshopper.
  /// </summary>
  public class GsaDesignTaskGoo : GH_OasysGoo<IGsaDesignTask> {
    public static string Description => "GSA Design Task";
    public static string Name => "Design Task";
    public static string NickName => "ΣD";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaDesignTaskGoo(IGsaDesignTask item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaDesignTaskGoo(Value);
    }
  }
}
