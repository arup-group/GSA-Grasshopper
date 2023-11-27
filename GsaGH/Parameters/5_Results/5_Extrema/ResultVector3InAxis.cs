namespace GsaGH.Parameters.Results {
  public class ResultVector3InAxis<T> : IResultVector3InAxis<T> {
    public T X { get; set; }

    public T Y { get; set; }

    public T Z { get; set; }

    public T Xyz { get; set; }

    public ResultVector3InAxis() {
    }

    public ResultVector3InAxis(T initialValue) {
      X = initialValue;
      Y = initialValue;
      Z = initialValue;
      Xyz = initialValue;
    }
  }
}
