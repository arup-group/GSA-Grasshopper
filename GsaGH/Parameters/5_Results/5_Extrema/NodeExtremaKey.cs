namespace GsaGH.Parameters.Results {
  public class NodeExtremaKey : IExtremaKey {
    public int Id { get; internal set; }
    public int Permutation { get; internal set; }
    public NodeExtremaKey(int id, int permutation = 0) {
      Id = id;
      Permutation = permutation;
    }
  }
}
