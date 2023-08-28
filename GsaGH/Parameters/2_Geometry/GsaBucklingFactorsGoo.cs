using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaBucklingFactors" /> can be used in Grasshopper.
  /// </summary>
  public class GsaBucklingFactorsGoo : GH_OasysGoo<GsaBucklingFactors> {
    public static string Description => "GSA Equivalent uniform moment factor for LTB for 1D Member";
    public static string Name => "Buckling Factors";
    public static string NickName => "BFs";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaBucklingFactorsGoo(GsaBucklingFactors item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaBucklingFactorsGoo(Value);
    }
  }
}
