using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaBucklingLengthFactors" /> can be used in Grasshopper.
  /// </summary>
  public class GsaBucklingLengthFactorsGoo : GH_OasysGoo<GsaBucklingLengthFactors> {
    public static string Description => 
      "Buckling Factors are part of a 1D Member and can be used to \n" +
      "override the automatically calculated factor to account for \n" +
      "the shape of the moment diagram in lateral torsional buckling \n" +
      "design equations. This override is applied for all bending \n" +
      "segments in the member.";
    public static string Name => "BucklingFactors";
    public static string NickName => "BFs";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaBucklingLengthFactorsGoo(GsaBucklingLengthFactors item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaBucklingLengthFactorsGoo(Value);
    }
  }
}
