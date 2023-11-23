using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public abstract class Entity3dTetraResult<T> : IEntity3dTetraQuantity<T>
    where T : IResultItem {
    public T Node1 { get; internal set; }
    public T Node2 { get; internal set; }
    public T Node3 { get; internal set; }
    public T Node4 { get; internal set; }
    public T Centre { get; internal set; }

    public IList<T> Results() {
      return new List<T>() {
        Node1,
        Node2,
        Node3,
        Node4,
        Centre
      };
    }
  }
}
