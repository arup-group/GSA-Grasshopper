using GsaAPI;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters.Results {
  public class Stress : IStress {
    public Pressure Xx { get; internal set; }
    public Pressure Yy { get; internal set; }
    public Pressure Zz { get; internal set; }
    public Pressure Xy { get; internal set; }
    public Pressure Yz { get; internal set; }
    public Pressure Zx { get; internal set; }

    internal Stress(Tensor3 result) {
      Xx = new Pressure(result.XX, PressureUnit.Pascal);
      Yy = new Pressure(result.YY, PressureUnit.Pascal);
      Zz = new Pressure(result.ZZ, PressureUnit.Pascal);
      Xy = new Pressure(result.XY, PressureUnit.Pascal);
      Yz = new Pressure(result.YZ, PressureUnit.Pascal);
      Zx = new Pressure(result.ZX, PressureUnit.Pascal);
    }
  }
}
