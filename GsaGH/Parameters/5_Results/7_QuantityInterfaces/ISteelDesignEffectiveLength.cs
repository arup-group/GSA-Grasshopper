using System.Collections.Generic;

using OasysUnits;

namespace GsaGH.Parameters.Results {
  public interface ISteelDesignEffectiveLength : IResultItem {
    Length MemberLength { get; }
    List<SubSpan> MajorAxisSubSpans { get; }
    List<SubSpan> MinorAxisSubSpans { get; }
    List<SubSpan> LateralTorsionalSubSpans { get; }
  }
}
