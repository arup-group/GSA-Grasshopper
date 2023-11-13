namespace GsaGH.Parameters.Results {
  public interface IResultTensor2AroundAxis<T> {
    T Mx { get; }
    T My { get; }
    T Mxy { get; }
    T WoodArmerX { get; }
    T WoodArmerY { get; }
  }
}
