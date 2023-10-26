namespace GsaGH.Parameters.Results {
  public interface IResultTensor3InAxis<T> {
    T Xy { get; }
    T Yz { get; }
    T Zx { get; }
  }
}
