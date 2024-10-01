using GsaAPI;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters.Results {
  public class SteelUtilisation : ISteelUtilisation {
    public Ratio? Overall { get; internal set; }
    public Ratio? LocalCombined { get; internal set; }
    public Ratio? BucklingCombined { get; internal set; }
    public Ratio? LocalAxial { get; internal set; }
    public Ratio? LocalShearU { get; internal set; }
    public Ratio? LocalShearV { get; internal set; }
    public Ratio? LocalTorsion { get; internal set; }
    public Ratio? LocalMajorMoment { get; internal set; }
    public Ratio? LocalMinorMoment { get; internal set; }
    public Ratio? MajorBuckling { get; internal set; }
    public Ratio? MinorBuckling { get; internal set; }
    public Ratio? LateralTorsionalBuckling { get; internal set; }
    public Ratio? TorsionalBuckling { get; internal set; }
    public Ratio? FlexuralBuckling { get; internal set; }

    internal SteelUtilisation(SteelUtilisationResult values) {
      if (!double.IsNaN(values.Overall)) {
        Overall = new Ratio(values.Overall, RatioUnit.DecimalFraction);
      }
      if (!double.IsNaN(values.LocalCombined)) {
        LocalCombined = new Ratio(values.LocalCombined, RatioUnit.DecimalFraction);
      }
      if (!double.IsNaN(values.BucklingCombined)) {
        BucklingCombined = new Ratio(values.BucklingCombined, RatioUnit.DecimalFraction);
      }
      if (!double.IsNaN(values.LocalAxial)) {
        LocalAxial = new Ratio(values.LocalAxial, RatioUnit.DecimalFraction);
      }
      if (!double.IsNaN(values.LocalSu)) {
        LocalShearU = new Ratio(values.LocalSu, RatioUnit.DecimalFraction);
      }
      if (!double.IsNaN(values.LocalSv)) {
        LocalShearV = new Ratio(values.LocalSv, RatioUnit.DecimalFraction);
      }
      if (!double.IsNaN(values.LocalTorsion)) {
        LocalTorsion = new Ratio(values.LocalTorsion, RatioUnit.DecimalFraction);
      }
      if (!double.IsNaN(values.LocalMuu)) {
        LocalMajorMoment = new Ratio(values.LocalMuu, RatioUnit.DecimalFraction);
      }
      if (!double.IsNaN(values.LocalMvv)) {
        LocalMinorMoment = new Ratio(values.LocalMvv, RatioUnit.DecimalFraction);
      }
      if (!(double.IsNaN(values.BucklingUu) || double.IsInfinity(values.BucklingUu))) {
        MajorBuckling = new Ratio(values.BucklingUu, RatioUnit.DecimalFraction);
      }
      if (!double.IsNaN(values.BucklingVv)) {
        MinorBuckling = new Ratio(values.BucklingVv, RatioUnit.DecimalFraction);
      }
      if (!double.IsNaN(values.BucklingLT)) {
        LateralTorsionalBuckling = new Ratio(values.BucklingLT, RatioUnit.DecimalFraction);
      }
      if (!double.IsNaN(values.BucklingTor)) {
        TorsionalBuckling = new Ratio(values.BucklingTor, RatioUnit.DecimalFraction);
      }
      if (!double.IsNaN(values.BucklingFT)) {
        FlexuralBuckling = new Ratio(values.BucklingFT, RatioUnit.DecimalFraction);
      }
    }

    internal SteelUtilisation(double initialValue) {
      Overall = new Ratio(initialValue, RatioUnit.DecimalFraction);
      LocalCombined = new Ratio(initialValue, RatioUnit.DecimalFraction);
      BucklingCombined = new Ratio(initialValue, RatioUnit.DecimalFraction);
      LocalAxial = new Ratio(initialValue, RatioUnit.DecimalFraction);
      LocalShearU = new Ratio(initialValue, RatioUnit.DecimalFraction);
      LocalShearV = new Ratio(initialValue, RatioUnit.DecimalFraction);
      LocalTorsion = new Ratio(initialValue, RatioUnit.DecimalFraction);
      LocalMajorMoment = new Ratio(initialValue, RatioUnit.DecimalFraction);
      LocalMinorMoment = new Ratio(initialValue, RatioUnit.DecimalFraction);
      MajorBuckling = new Ratio(initialValue, RatioUnit.DecimalFraction);
      MinorBuckling = new Ratio(initialValue, RatioUnit.DecimalFraction);
      LateralTorsionalBuckling = new Ratio(initialValue, RatioUnit.DecimalFraction);
      TorsionalBuckling = new Ratio(initialValue, RatioUnit.DecimalFraction);
      FlexuralBuckling = new Ratio(initialValue, RatioUnit.DecimalFraction);
    }

    public double? OverallAs(RatioUnit unit) {
      if (Overall.HasValue && !double.IsNaN(Overall.Value.Value)) {
        return Overall.Value.As(unit);
      }
      return null;
    }
  }
}
