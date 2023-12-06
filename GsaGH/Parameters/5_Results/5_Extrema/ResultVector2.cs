namespace GsaGH.Parameters.Results {
  public class ResultVector2<T> : IResultVector2<T> {
    public T Qx { get; set; }
    public T Qy { get; set; }

    public ResultVector2() {
    }

    public ResultVector2(T initialValue) {
      Qx = initialValue;
      Qy = initialValue;
    }

  }
}
