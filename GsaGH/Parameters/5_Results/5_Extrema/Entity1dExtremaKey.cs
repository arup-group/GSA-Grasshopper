namespace GsaGH.Parameters.Results {
  public class Entity1dExtremaKey {
    public int Id { get; internal set; }
    public int Permutation { get; internal set; }
    public double Position { get; internal set; }
    public Entity1dExtremaKey(int id, double position, int permutation = 0) {
      Id = id;
      Position = position;
      Permutation = permutation;
    }
  }
}
