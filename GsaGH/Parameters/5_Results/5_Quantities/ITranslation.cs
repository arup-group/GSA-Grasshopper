using OasysUnits;

namespace GsaGH.Parameters.Results {
  public interface ITranslation : IResultItem {
    public Length X { get; }
    public Length Y { get; }
    public Length Z { get; }
    public Length Xyz { get; }
  }
}
