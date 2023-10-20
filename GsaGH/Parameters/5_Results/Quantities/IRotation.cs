using OasysUnits;

namespace GsaGH.Parameters.Results {
  public interface IRotation : IResultQuantitySet {
    public Angle Xx { get; }
    public Angle Xxyyzz { get; }
    public Angle Yy { get; }
    public Angle Zz { get; }
  }
}
