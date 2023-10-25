namespace GsaGH.Parameters.Results {
  public class ExtremaSextet : IResultExtrema {
    public (int Id, int Permutation) X { get; internal set; }
    public (int Id, int Permutation) Y { get; internal set; }
    public (int Id, int Permutation) Z { get; internal set; }
    public (int Id, int Permutation) Xyz { get; internal set; }
    public (int Id, int Permutation) Xx { get; internal set; }
    public (int Id, int Permutation) Yy { get; internal set; }
    public (int Id, int Permutation) Zz { get; internal set; }
    public (int Id, int Permutation) Xxyyzz { get; internal set; }
  }
}
