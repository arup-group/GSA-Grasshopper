using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IEntity1dQuantity<T> where T : IResultItem {
    IDictionary<double, T> Results { get; }
    IEntity1dQuantity<T> TakePositions(ICollection<double> positions);
    ICollection<string> Warnings { get; }
    ICollection<string> Errors { get; }
  }
}
