using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaAnalysisTask" /> can be used in Grasshopper.
  /// </summary>
  public class GsaAnalysisTaskGoo : GH_OasysGoo<GsaAnalysisTask> {
    public static string Description =>
      "An analysis task is a package of work for the solver. \n" +
      "Thus we can have a static analysis task, a modal analysis \n" +
      "task, etc. Each analysis task has one or more analysis \n" +
      "case(s). The distinction is that the cases corresponds to \n" +
      "result sets and define items such as loading (in the static \n" +
      "case) while the task describes what the solver has to do. \n" +
      "In Grasshopper, it is only possible to create linear static \n" +
      "analysis tasks";
    public static string Name => "Analysis Task";
    public static string NickName => "ΣT";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaAnalysisTaskGoo(GsaAnalysisTask item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaAnalysisTaskGoo(Value);
    }
  }
}
