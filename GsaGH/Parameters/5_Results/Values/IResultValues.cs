using System.Collections.Concurrent;
using System.Collections.Generic;
using static GsaGH.Parameters.GsaResultsValues;

namespace GsaGH.Parameters._5_Results.Values {
  public interface IResultValues<T1, T2, T3> {
    public T1 DmaxX { get; set; }
    public T2 DmaxXx { get; set; }
    public T2 DmaxXxyyzz { get; set; }
    public T1 DmaxXyz { get; set; }
    public T1 DmaxY { get; set; }
    public T2 DmaxYy { get; set; }
    public T1 DmaxZ { get; set; }
    public T2 DmaxZz { get; set; }
    public T1 DminX { get; set; }
    public T2 DminXx { get; set; }
    public T2 DminXxyyzz { get; set; }
    public T1 DminXyz { get; set; }
    public T1 DminY { get; set; }
    public T2 DminYy { get; set; }
    public T1 DminZ { get; set; }
    public T2 DminZz { get; set; }
    public ResultType Type { get; set; }
    public List<int> Ids { get; }

    public ConcurrentDictionary<int, ConcurrentDictionary<int, T3>>
      Results { get; set; }
  } 
}
