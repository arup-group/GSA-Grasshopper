using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IQuantity1D<T> where T : IResultItem {
    IDictionary<double, T> Results { get; }
  }
}
