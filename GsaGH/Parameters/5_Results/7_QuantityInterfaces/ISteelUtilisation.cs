using OasysUnits;

namespace GsaGH.Parameters.Results {
  public interface ISteelUtilisation : IResultItem {
    Ratio? Overall { get; }
    Ratio? LocalCombined { get; }
    Ratio? BucklingCombined { get; }
    Ratio? LocalAxial { get; }
    Ratio? LocalShearU { get; }
    Ratio? LocalShearV { get; }
    Ratio? LocalTorsion { get; }
    Ratio? LocalMajorMoment { get; }
    Ratio? LocalMinorMoment { get; }
    Ratio? MajorBuckling { get; }
    Ratio? MinorBuckling { get; }
    Ratio? LateralTorsionalBuckling { get; }
    Ratio? TorsionalBuckling { get; }
    Ratio? FlexuralBuckling { get; }
  }
}
