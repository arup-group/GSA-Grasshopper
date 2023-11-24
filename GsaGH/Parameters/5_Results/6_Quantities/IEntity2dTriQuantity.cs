namespace GsaGH.Parameters.Results {
  public interface IEntity2dTriQuantity<T> : IEntity2dQuantity<T> where T : IResultItem {
    T Node1 { get; }
    T Node2 { get; }
    T Node3 { get; }
  }
}
