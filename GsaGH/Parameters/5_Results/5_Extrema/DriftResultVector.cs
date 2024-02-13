namespace GsaGH.Parameters.Results {
  public class DriftResultVector<T> : IDrift<T> {
    public T X { get; set; }
    public T Y { get; set; }
    //public T Z { get; set; }
    //public T Xyz { get; set; }
    public T Xy { get; set; }

    public DriftResultVector() {
    }

    public DriftResultVector(T initialValue) {
      X = initialValue;
      Y = initialValue;
      //Z = initialValue;
      //Xyz = initialValue;
      Xy = initialValue;
    }
  }
}
