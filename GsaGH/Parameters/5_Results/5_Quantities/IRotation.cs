using OasysUnits;

namespace GsaGH.Parameters.Results {
  public interface IRotation : IResultItem {
    public Angle Xx { get; }
    public Angle Yy { get; }
    public Angle Zz { get; }
    public Angle Xxyyzz { get; }
  }
}
