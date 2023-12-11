namespace GsaGH.Parameters.Results {
  public class Entity2dExtremaKey : IEntity2dExtremaKey {
    public int Id { get; internal set; }
    public int Permutation { get; internal set; }
    public int VertexId { get; internal set; }
    public Entity2dExtremaKey(int id, int vertex, int permutation = 0) {
      Id = id;
      VertexId = vertex;
      Permutation = permutation;
    }
  }
}
