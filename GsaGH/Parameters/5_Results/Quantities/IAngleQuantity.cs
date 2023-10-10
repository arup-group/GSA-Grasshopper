using GsaAPI;
using OasysUnits;
using AngleUnit = OasysUnits.Units.AngleUnit;

namespace GsaGH.Parameters._5_Results.Quantities {
  public interface IAngleQuantity : IResultQuantity {
    public Angle Xx { get; }
    public Angle Xxyyzz { get; }
    public Angle Yy { get; }
    public Angle Zz { get; }
  
    public void SetAngleUnit(Double6 result, AngleUnit unit);
  }
}
