using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public abstract class Entity1dResult<ApiResultType, QuantityResult>
    : IEntity1dQuantity<QuantityResult> where QuantityResult : IResultItem {
    public IDictionary<double, QuantityResult> Results { get; private set; }
    public ICollection<string> Warnings { get; internal set; } = new List<string>();
    public ICollection<string> Errors { get; internal set; } = new List<string>();

    internal Entity1dResult(IDictionary<double, QuantityResult> results) {
      Results = results;
    }

    internal Entity1dResult(
      ReadOnlyCollection<ApiResultType> result, ReadOnlyCollection<double> positions,
      Func<ApiResultType, QuantityResult> constructor) {
      Results = new SortedDictionary<double, QuantityResult>();
      for (int i = 0; i < result.Count; i++) {
        Results.Add(positions[i], constructor(result[i]));
      }
    }

    public IDictionary<double, QuantityResult> TakePositions(
      IEntity1dQuantity<QuantityResult> existing, ICollection<double> positions) {
      var results = new SortedDictionary<double, QuantityResult>();
      foreach (double position in positions) {
        results.Add(position, existing.Results[position]);
      }

      return results;
    }

    public abstract IEntity1dQuantity<QuantityResult> TakePositions(ICollection<double> positions);
  }
}
