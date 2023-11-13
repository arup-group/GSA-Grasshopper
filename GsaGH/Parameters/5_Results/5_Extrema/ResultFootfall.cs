namespace GsaGH.Parameters.Results {
  public class ResultFootfall<T> {
    public T CriticalFrequency { get; set; }

    public T MaximumResponseFactor { get; set; }

    public T PeakAcceleration { get; set; }

    public T PeakVelocity { get; set; }

    public T RmsAcceleration { get; set; }

    public T RmsVelocity { get; set; }

    
    public ResultFootfall() {
    }

    public ResultFootfall(T initialValue) {
      CriticalFrequency = initialValue;
      MaximumResponseFactor = initialValue;
      PeakAcceleration = initialValue;
      PeakVelocity = initialValue;
      RmsAcceleration = initialValue;
      RmsVelocity = initialValue;
    }
  }
}
