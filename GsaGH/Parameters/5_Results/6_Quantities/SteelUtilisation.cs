using GsaAPI;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters.Results {
  public class SteelUtilisation : ISteelUtilisation {
    public Ratio Overall { get; internal set; }
    public Ratio LocalCombined { get; internal set; }
    public Ratio BucklingCombined { get; internal set; }
    public Ratio LocalAxial { get; internal set; }
    public Ratio LocalShearU { get; internal set; }
    public Ratio LocalShearV { get; internal set; }
    public Ratio LocalTorsion { get; internal set; }
    public Ratio LocalMajorMoment { get; internal set; }
    public Ratio LocalMinorMoment { get; internal set; }
    public Ratio MajorBuckling { get; internal set; }
    public Ratio MinorBuckling { get; internal set; }
    public Ratio LateralTorsionalBuckling { get; internal set; }
    public Ratio TorsionalBuckling { get; internal set; }
    public Ratio FlexuralBuckling { get; internal set; }

    internal SteelUtilisation(SteelUtilisationResult result) {
      Overall = new Ratio(result.Overall, RatioUnit.DecimalFraction);
      LocalCombined = new Ratio(result.LocalCombined, RatioUnit.DecimalFraction);
      BucklingCombined = new Ratio(result.BucklingCombined, RatioUnit.DecimalFraction);
      LocalAxial = new Ratio(result.LocalAxial, RatioUnit.DecimalFraction);
      LocalShearU = new Ratio(result.LocalSu, RatioUnit.DecimalFraction);
      LocalShearV = new Ratio(result.LocalSv, RatioUnit.DecimalFraction);
      LocalTorsion = new Ratio(result.LocalTorsion, RatioUnit.DecimalFraction);
      LocalMajorMoment = new Ratio(result.LocalMuu, RatioUnit.DecimalFraction);
      LocalMinorMoment = new Ratio(result.LocalMvv, RatioUnit.DecimalFraction);
      MajorBuckling = new Ratio(result.BucklingUu, RatioUnit.DecimalFraction);
      MinorBuckling = new Ratio(result.BucklingVv, RatioUnit.DecimalFraction);
      LateralTorsionalBuckling = new Ratio(result.BucklingLT, RatioUnit.DecimalFraction);
      TorsionalBuckling = new Ratio(result.BucklingTor, RatioUnit.DecimalFraction);
      FlexuralBuckling = new Ratio(result.BucklingFT, RatioUnit.DecimalFraction);
    }
  }
}
