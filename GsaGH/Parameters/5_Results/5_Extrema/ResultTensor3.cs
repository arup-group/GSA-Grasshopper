namespace GsaGH.Parameters.Results {
  public class ResultTensor3<T> : IResultTensor3<T> {
    public T Xx { get; set; }
    public T Yy { get; set; }
    public T Zz { get; set; }
    public T Xy { get; set; }
    public T Yz { get; set; }
    public T Zx { get; set; }

    public ResultTensor3() {
    }

    public ResultTensor3(T initialValue) {
      Xx = initialValue;
      Yy = initialValue;
      Zz = initialValue;
      Xy = initialValue;
      Yz = initialValue;
      Zx = initialValue;
    }
  }
}
