namespace GsaGH.Parameters.Results {
  public interface IEntity3dWedgeQuantity<T> : IEntity3dQuantity<T> where T : IResultItem {
    T Node1 { get; }
    T Node2 { get; }
    T Node3 { get; }
    T Node4 { get; }
    T Node5 { get; }
    T Node6 { get; }
  }
}
