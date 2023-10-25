namespace GsaGH.Parameters.Results {
  public class ExtremaVector3InAxis : IResultExtrema {
    public (int Id, int Permutation) X { get; internal set; }
    public (int Id, int Permutation) Y { get; internal set; }
    public (int Id, int Permutation) Z { get; internal set; }
    public (int Id, int Permutation) Xyz { get; internal set; }
  }
}
