using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IEntity1dQuantity<T> where T : IResultItem {
    IDictionary<double, T> Results { get; }
  }
}
