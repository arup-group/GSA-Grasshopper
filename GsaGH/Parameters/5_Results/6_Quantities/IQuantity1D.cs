using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IQuantity1d<T> where T : IResultItem {
    IDictionary<double, T> Results { get; }
  }
}
