namespace GsaGH.Parameters.Results {
  public class ResultVector2<T> : IResultVector2<T> {
    public T Vx { get; set; }
    public T Vy { get; set; }

    public ResultVector2() {
    }

    public ResultVector2(T initialValue) {
      Vx = initialValue;
      Vy = initialValue;
    }

  }
}
