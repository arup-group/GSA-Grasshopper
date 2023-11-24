using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IEntity2dQuantity<T> where T : IResultItem {
    T Centre { get; }

    IList<T> Results();
  }
}
