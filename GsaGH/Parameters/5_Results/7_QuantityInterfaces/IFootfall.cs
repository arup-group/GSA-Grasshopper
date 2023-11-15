using OasysUnits;

namespace GsaGH.Parameters.Results {
  public interface IFootfall : IResultItem {
    Frequency CriticalFrequency { get; }
    int CriticalNode { get; }
    double MaximumResponseFactor { get; }
    Acceleration PeakAcceleration { get; }
    Speed PeakVelocity { get; }
    Acceleration RmsAcceleration { get; }
    Speed RmsVelocity { get; }
  }
}
