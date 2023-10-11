using System.Collections.Concurrent;
using System.Collections.Generic;
using static GsaGH.Parameters.GsaResultsValues;

namespace GsaGH.Parameters._5_Results.Values {
  public interface IResultValues<T> {
    public T Max { get; set; }
    public T Min { get; set; }
    public ResultType Type { get; set; }
    public List<int> Ids { get; }

    public ConcurrentDictionary<int, ConcurrentDictionary<int, T>>
      Results { get; set; }
  } 
}
