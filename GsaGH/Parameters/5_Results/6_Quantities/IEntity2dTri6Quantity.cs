namespace GsaGH.Parameters.Results {
  public interface IEntity2dTri6Quantity<T> : IEntity2dTriQuantity<T> where T : IResultItem {
    T Node4 { get; }
    T Node5 { get; }
    T Node6 { get; }
  }
}
