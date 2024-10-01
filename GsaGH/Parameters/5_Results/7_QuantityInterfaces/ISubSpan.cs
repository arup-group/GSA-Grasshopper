using System.Collections.ObjectModel;

using OasysUnits;

namespace GsaGH.Parameters.Results {
  public interface ISubSpan : IResultItem {
    Length? StartPosition { get; }
    Length? EndPosition { get; }
    Length? SpanLength { get; }
    Length? EffectiveLength { get; }
    Ratio? SlendernessRatio { get; }
    ReadOnlyCollection<int> ElementIds { get; }
  }
}
