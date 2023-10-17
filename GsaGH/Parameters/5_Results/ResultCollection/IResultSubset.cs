using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public interface IResultSubset<IResult> {
    public IResult Max { get; }
    public IResult Min { get; }
    public List<int> Ids { get; }
    public ConcurrentDictionary<int, Collection<IResult>> Results { get; }
  } 
}
