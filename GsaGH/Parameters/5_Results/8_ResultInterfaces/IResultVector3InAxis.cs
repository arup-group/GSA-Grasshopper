namespace GsaGH.Parameters.Results {
  public interface IResultVector3InAxis<T> : IResultVector3<T> {
    T Xyz { get; }
  }
}
