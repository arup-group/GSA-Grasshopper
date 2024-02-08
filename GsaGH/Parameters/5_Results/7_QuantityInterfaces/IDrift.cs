using OasysUnits;

namespace GsaGH.Parameters.Results {
  public interface IDrift<T> : IResultItem {
    T X { get; }
    T Y { get; }
    T Xy { get; }
  }
}
