namespace GsaGH.Parameters.Results {
  public interface IEntity2dQuad8Quantity<T> : IEntity2dQuadQuantity<T> where T : IResultItem {
    T Node5 { get; }
    T Node6 { get; }
    T Node7 { get; }
    T Node8 { get; }
  }
}
