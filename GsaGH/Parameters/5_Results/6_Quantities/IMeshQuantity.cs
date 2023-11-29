using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IMeshQuantity<T> where T : IResultItem {
    T Centre { get; }

    IList<T> Results();
  }
}
