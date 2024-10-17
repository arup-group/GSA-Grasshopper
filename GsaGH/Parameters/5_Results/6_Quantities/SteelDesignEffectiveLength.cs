using System.Collections.Generic;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters.Results {
  public class SteelDesignEffectiveLength : ISteelDesignEffectiveLength {
    public Length MemberLength { get; internal set; }
    public List<SubSpan> MajorAxisSubSpans { get; internal set; } = new List<SubSpan>();
    public List<SubSpan> MinorAxisSubSpans { get; internal set; } = new List<SubSpan>();
    public List<SubSpan> LateralTorsionalSubSpans { get; internal set; } = new List<SubSpan>();

    internal SteelDesignEffectiveLength(GsaAPI.SteelDesignEffectiveLength values) {
      MemberLength = new Length(values.MemberLength, LengthUnit.Meter);
      foreach (GsaAPI.SubSpan subSpan in values.MajorAxisSubSpans) {
        MajorAxisSubSpans.Add(new SubSpan(subSpan));
      }
      foreach (GsaAPI.SubSpan subSpan in values.MinorAxisSubSpans) {
        MinorAxisSubSpans.Add(new SubSpan(subSpan));
      }
      foreach (GsaAPI.SubSpan subSpan in values.LateralTorsionalSubSpans) {
        LateralTorsionalSubSpans.Add(new SubSpan(subSpan));
      }
    }
  }
}
