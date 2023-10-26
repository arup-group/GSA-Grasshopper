using OasysUnits;

namespace GsaGH.Parameters {
  public class GsaResultQuantity {
    internal IQuantity X { get; set; }
    internal IQuantity Xyz { get; set; }
    internal IQuantity Y { get; set; }
    internal IQuantity Z { get; set; }

    internal GsaResultQuantity() { }
  }
}
