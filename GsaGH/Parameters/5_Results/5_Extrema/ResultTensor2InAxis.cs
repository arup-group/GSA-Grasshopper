namespace GsaGH.Parameters.Results {
  public class ResultTensor2InAxis<T> : IResultTensor2InAxis<T> {
    public T Nx { get; set; }
    public T Ny { get; set; }
    public T Nxy { get; set; }

    public ResultTensor2InAxis() {
    }

    public ResultTensor2InAxis(T initialValue) {
      Nx = initialValue;
      Ny = initialValue;
      Nxy = initialValue;
    }

  }
}
