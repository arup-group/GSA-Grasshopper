using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaAnalysisTask" /> can be used in Grasshopper.
  /// </summary>
  public class GsaAnalysisTaskGoo : GH_OasysGoo<GsaAnalysisTask> {
    public static string Description => "GSA Analysis Task";
    public static string Name => "Analysis Task";
    public static string NickName => "ΣT";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaAnalysisTaskGoo(GsaAnalysisTask item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaAnalysisTaskGoo(Value);
    }
  }
}
