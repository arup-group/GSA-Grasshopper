using System.Collections.ObjectModel;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters.Results {
  public class SubSpan : ISubSpan {
    public Length? StartPosition { get; internal set; }
    public Length? EndPosition { get; internal set; }
    public Length? EffectiveLength { get; internal set; }
    public Ratio? SlendernessRatio { get; internal set; }
    public ReadOnlyCollection<int> ElementIds { get; internal set; }

    internal SubSpan(GsaAPI.SubSpan values) {
      if (!IsNotANumber(values.StartPosition)) {
          StartPosition = new Length(values.StartPosition, LengthUnit.Meter);
      }
      if (!IsNotANumber(values.EndPosition)) {
          EndPosition = new Length(values.EndPosition, LengthUnit.Meter);
      }
      if (!IsNotANumber(values.EffectiveLength)) {
          EffectiveLength = new Length(values.EffectiveLength, LengthUnit.Meter);
      }
      if (!IsNotANumber(values.SlendernessRatio)) {
          SlendernessRatio = new Ratio(values.SlendernessRatio, RatioUnit.DecimalFraction);
      }
        
      ElementIds = values.ElementIds;
    }

    private static bool IsNotANumber(double? value) {
        return value.HasValue && (double.IsNaN(value.Value) || double.IsInfinity(value.Value));
    }
  }
}
