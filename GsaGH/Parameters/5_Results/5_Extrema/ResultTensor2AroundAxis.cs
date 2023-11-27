namespace GsaGH.Parameters.Results {
  public class ResultTensor2AroundAxis<T> : IResultTensor2AroundAxis<T> {
    public T Mx { get; set; }
    public T My { get; set; }
    public T Mxy { get; set; }
    public T WoodArmerX { get; set; }
    public T WoodArmerY { get; set; }

    public ResultTensor2AroundAxis() {
    }

    public ResultTensor2AroundAxis(T initialValue) {
      Mx = initialValue;
      My = initialValue;
      Mxy = initialValue;
      WoodArmerX = initialValue;
      WoodArmerY = initialValue;
    }
  }
}
