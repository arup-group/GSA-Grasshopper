namespace GsaGH.Parameters.Results {
  public interface IResultTensor3<T> {
    T Xx { get; }
    T Yy { get; }
    T Zz { get; }
    T Xy { get; }
    T Yz { get; }
    T Zx { get; }
  }
}
