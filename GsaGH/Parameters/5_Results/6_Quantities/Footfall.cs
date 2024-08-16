using GsaAPI;

using OasysUnits;
using OasysUnits.Units;

using AccelerationUnit = OasysUnits.Units.AccelerationUnit;

namespace GsaGH.Parameters.Results {
  public class Footfall : IFootfall {
    public Frequency CriticalFrequency { get; internal set; }
    public int CriticalNode { get; internal set; }
    public double MaximumResponseFactor { get; internal set; }
    public Acceleration PeakAcceleration { get; internal set; }
    public Speed PeakVelocity { get; internal set; }
    public Acceleration RmsAcceleration { get; internal set; }
    public Speed RmsVelocity { get; internal set; }

    internal Footfall(NodeFootfallResult result) {
      CriticalFrequency = new Frequency(result.CriticalFrequency, FrequencyUnit.Hertz);
      CriticalNode = result.CriticalNode;
      MaximumResponseFactor = result.MaximumResponseFactor;
      AccelerationUnit acceleration = AccelerationUnit.MeterPerSecondSquared;
      PeakAcceleration = new Acceleration(result.PeakAcceleration, acceleration);
      SpeedUnit speed = SpeedUnit.MeterPerSecond;
      PeakVelocity = new Speed(result.PeakVelocity, speed);
      RmsAcceleration = new Acceleration(result.RmsAcceleration, acceleration);
      RmsVelocity = new Speed(result.RmsVelocity, speed);
    }
  }
}
