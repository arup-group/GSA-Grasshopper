namespace GsaGH.Parameters.Results {
  public interface IEntity3dPyramid5Quantity<T> : IEntity3dQuantity<T> where T : IResultItem {
    T Node1 { get; }
    T Node2 { get; }
    T Node3 { get; }
    T Node4 { get; }
    T Node5 { get; }
    T Face1Centre { get; }
    T Face2Centre { get; }
    T Face3Centre { get; }
    T Face4Centre { get; }
    T Face5Centre { get; }
  }
}
