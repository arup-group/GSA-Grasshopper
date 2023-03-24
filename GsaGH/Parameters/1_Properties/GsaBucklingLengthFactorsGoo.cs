using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaBucklingLengthFactors"/> can be used in Grasshopper.
  /// </summary>
  public class GsaBucklingLengthFactorsGoo : GH_OasysGoo<GsaBucklingLengthFactors> {
    public static string Name => "BucklingLengthFactors";
    public static string NickName => "fLs";
    public static string Description => "GSA Buckling Length Factors";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaBucklingLengthFactorsGoo(GsaBucklingLengthFactors item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaBucklingLengthFactorsGoo(Value);

    public override bool CastFrom(object source) {
      if (source == null)
        return false;

      if (base.CastFrom(source))
        return true;

      if (!GH_Convert.ToDouble(source, out double val, GH_Conversion.Both)) {
        return false;
      }

      Value.LateralTorsionalBucklingFactor = val;
      Value.MomentAmplificationFactorStrongAxis = val;
      Value.MomentAmplificationFactorWeakAxis = val;
      return true;

    }
  }
}
