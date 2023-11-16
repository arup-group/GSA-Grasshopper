namespace GsaGH.Parameters.Results {
  public class ResultDerivedStress1d<T> {
    public T ElasticShearY { get; set; }
    public T ElasticShearZ { get; set; }
    public T Torsional { get; set; }
    public T VonMises { get; set; }

    public ResultDerivedStress1d() {
    }

    public ResultDerivedStress1d(T initialValue) {
      ElasticShearY = initialValue;
      ElasticShearZ = initialValue;
      Torsional = initialValue;
      VonMises = initialValue;
    }
  }
}
