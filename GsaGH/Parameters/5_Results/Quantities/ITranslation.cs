using OasysUnits;

namespace GsaGH.Parameters.Results {
  public interface ITranslation : IResultQuantitySet {
    public Length X { get; }
    public Length Xyz { get; }
    public Length Y { get; }
    public Length Z { get; }
  }
}
