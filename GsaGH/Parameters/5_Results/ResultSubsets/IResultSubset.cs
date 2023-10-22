using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IResultSubset<T> where T : IResultQuantitySet {
    public T Max { get; }
    public T Min { get; }
    public List<int> Ids { get; }
    public ConcurrentDictionary<int, ICollection<T>> Results { get; }
  } 
}
