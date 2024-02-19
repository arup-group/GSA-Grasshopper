using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IAssemblyQuantity<T> where T : IResultItem {
    IDictionary<double, T> Results { get; }
  }
}
