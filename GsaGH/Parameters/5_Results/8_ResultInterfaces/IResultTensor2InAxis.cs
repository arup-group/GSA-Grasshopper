namespace GsaGH.Parameters.Results {
  public interface IResultTensor2InAxis<T> {
    T Nx { get; }
    T Ny { get; }
    T Nxy { get; }
  }
}
