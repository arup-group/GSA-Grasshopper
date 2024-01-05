using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public interface IEntity1dResultCache<T1, T2>
    where T1 : IResultItem {
    IApiResult ApiResult { get; }
    IDictionary<int, IList<IEntity1dQuantity<T1>>> Cache { get; }
    IEntity1dResultSubset<T1, T2> ResultSubset(ICollection<int> elementIds, int positionCount);
    IEntity1dResultSubset<T1, T2> ResultSubset(ICollection<int> elementIds, ReadOnlyCollection<double> positions);
  }
}
