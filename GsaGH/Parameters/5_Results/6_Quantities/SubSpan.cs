using System.Collections.ObjectModel;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters.Results {
  public class SubSpan : ISubSpan {
    public Length? StartPosition { get; internal set; }
    public Length? EndPosition { get; internal set; }
    public Length? SpanLength { get; internal set; }
    public Length? EffectiveLength { get; internal set; }
    public Ratio? SlendernessRatio { get; internal set; }
    public ReadOnlyCollection<int> ElementIds { get; internal set; }

    internal SubSpan(GsaAPI.SubSpan values) {
      if (!double.IsNaN(values.StartPosition)) {
        StartPosition = new Length(values.StartPosition, LengthUnit.Meter);
      }

      if (!double.IsNaN(values.EndPosition)) {
        EndPosition = new Length(values.EndPosition, LengthUnit.Meter);
      }

      if (StartPosition.HasValue && EndPosition.HasValue) {
        SpanLength = EndPosition.Value - StartPosition.Value;
      }

      if (!double.IsNaN(values.EffectiveLength)) {
        EffectiveLength = new Length(values.EffectiveLength, LengthUnit.Meter);
      }

      if (!double.IsNaN(values.SlendernessRatio)) {
        SlendernessRatio = new Ratio(values.SlendernessRatio, RatioUnit.DecimalFraction);
      }
      ElementIds = values.ElementIds;
    }

    public Length? StartPositionToUnit(LengthUnit unit) {
      if (StartPosition.HasValue) {
        return StartPosition.Value.ToUnit(unit);
      }
      return null;
    }

    public Length? EndPositionToUnit(LengthUnit unit) {
      if (EndPosition.HasValue) {
        return EndPosition.Value.ToUnit(unit);
      }
      return null;
    }

    public Length? SpanLengthToUnit(LengthUnit unit) {
      if (StartPosition.HasValue && EndPosition.HasValue) {
        return (EndPosition.Value - StartPosition.Value).ToUnit(unit);
      }
      return null;
    }

    public Length? EffectiveLengthToUnit(LengthUnit unit) {
      if (EffectiveLength.HasValue) {
        return EffectiveLength.Value.ToUnit(unit);
      }
      return null;
    }

    public Ratio? SlendernessRatioToUnit(RatioUnit unit) {
      if (SlendernessRatio.HasValue) {
        return SlendernessRatio.Value.ToUnit(unit);
      }
      return null;
    }

    public double? StartPositionAs(LengthUnit unit) {
      if (StartPosition.HasValue) {
        return StartPosition.Value.As(unit);
      }
      return null;
    }

    public double? EndPositionAs(LengthUnit unit) {
      if (EndPosition.HasValue) {
        return EndPosition.Value.As(unit);
      }
      return null;
    }

    public double? SpanLengthAs(LengthUnit unit) {
      if (StartPosition.HasValue && EndPosition.HasValue) {
        return (EndPosition.Value - StartPosition.Value).As(unit);
      }
      return null;
    }

    public double? EffectiveLengthAs(LengthUnit unit) {
      if (EffectiveLength.HasValue) {
        return EffectiveLength.Value.As(unit);
      }
      return null;
    }

    public double? SlendernessRatioAs(RatioUnit unit) {
      if (SlendernessRatio.HasValue) {
        return SlendernessRatio.Value.As(unit);
      }
      return null;
    }
  }
}
