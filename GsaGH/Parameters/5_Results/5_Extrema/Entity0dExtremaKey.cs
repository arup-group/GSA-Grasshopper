namespace GsaGH.Parameters.Results {
  public class Entity0dExtremaKey : IExtremaKey {
    public int Id { get; internal set; }
    public int Permutation { get; internal set; }
    public Entity0dExtremaKey(int id, int permutation = 0) {
      Id = id;
      Permutation = permutation;
    }
  }
}
