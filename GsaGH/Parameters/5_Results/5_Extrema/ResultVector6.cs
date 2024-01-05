namespace GsaGH.Parameters.Results {
  public class ResultVector6<T> : IResultVector6<T, T> {
    public T X { get; set; }

    public T Y { get; set; }

    public T Z { get; set; }

    public T Xyz { get; set; }

    public T Xx { get; set; }

    public T Yy { get; set; }

    public T Zz { get; set; }

    public T Xxyyzz { get; set; }

    public ResultVector6() {
    }

    public ResultVector6(T initialValue) {
      X = initialValue;
      Y = initialValue;
      Z = initialValue;
      Xyz = initialValue;
      Xx = initialValue;
      Yy = initialValue;
      Zz = initialValue;
      Xxyyzz = initialValue;
    }
  }
}
