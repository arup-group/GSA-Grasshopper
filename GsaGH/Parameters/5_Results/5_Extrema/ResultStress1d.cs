namespace GsaGH.Parameters.Results {
  public class ResultStress1d<T> {
    public T Axial { get; set; }
    public T BendingYyNegativeZ { get; set; }
    public T BendingYyPositiveZ { get; set; }
    public T BendingZzNegativeY { get; set; }
    public T BendingZzPositiveY { get; set; }
    public T CombinedC1 { get; set; }
    public T CombinedC2 { get; set; }
    public T ShearY { get; set; }
    public T ShearZ { get; set; }


    public ResultStress1d() {
    }

    public ResultStress1d(T initialValue) {
      Axial = initialValue;
      BendingYyNegativeZ = initialValue;
      BendingYyPositiveZ = initialValue;
      BendingZzNegativeY = initialValue;
      BendingZzPositiveY = initialValue;
      CombinedC1 = initialValue;
      CombinedC2 = initialValue;
      ShearY = initialValue;
      ShearZ = initialValue;
    }
  }
}
