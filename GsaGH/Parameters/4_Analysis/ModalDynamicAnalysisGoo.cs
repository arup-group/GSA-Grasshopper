using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.Graphics;

using OasysGH;
using OasysGH.Parameters;
using OasysGH.Units;

using Rhino.Display;
using Rhino.Geometry;

using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaEffectiveLengthOptions" /> can be used in Grasshopper.
  /// </summary>
  public class GsaModalDynamicAnalysisGoo : GH_OasysGoo<GsaModalDynamicAnalysis> {
    public static string Description => "GSA modal dynamic analysis input";
    public static string Name => "Modal dynamic analyis input";
    public static string NickName => "Mdp";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaModalDynamicAnalysisGoo(GsaModalDynamicAnalysis item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaModalDynamicAnalysisGoo(Value);
    }
  }
}
