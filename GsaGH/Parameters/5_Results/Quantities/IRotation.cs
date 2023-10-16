using OasysUnits;

namespace GsaGH.Parameters.Results {
  public interface IRotation : IResult {
    public Angle Xx { get; }
    public Angle Xxyyzz { get; }
    public Angle Yy { get; }
    public Angle Zz { get; }
  }
}
