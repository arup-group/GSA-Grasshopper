using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {

  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaBucklingLengthFactors"/> can be used in Grasshopper.
  /// </summary>
  public class GsaBucklingLengthFactorsGoo : GH_OasysGoo<GsaBucklingLengthFactors> {

    #region Properties + Fields
    public static string Description => "GSA Buckling Length Factors";
    public static string Name => "BucklingLengthFactors";
    public static string NickName => "fLs";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    #endregion Properties + Fields

    #region Public Constructors
    public GsaBucklingLengthFactorsGoo(GsaBucklingLengthFactors item) : base(item) {
    }

    #endregion Public Constructors

    #region Public Methods
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

    public override IGH_Goo Duplicate() => new GsaBucklingLengthFactorsGoo(Value);

    #endregion Public Methods
  }
}
