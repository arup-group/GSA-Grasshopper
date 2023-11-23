using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public abstract class Entity3dWedgeResult<T> : IEntity3dWedgeQuantity<T>
    where T : IResultItem {
    public T Node1 { get; internal set; }
    public T Node2 { get; internal set; }
    public T Node3 { get; internal set; }
    public T Node4 { get; internal set; }
    public T Node5 { get; internal set; }
    public T Node6 { get; internal set; }
    public T Centre { get; internal set; }

    public IList<T> Results() {
      return new List<T>() {
        Node1,
        Node2,
        Node3,
        Node4,
        Node5,
        Node6,
        Centre
      };
    }
  }
}
