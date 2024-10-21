using System;

using GsaAPI;

using OasysUnits;

using ForceUnit = OasysUnits.Units.ForceUnit;

namespace GsaGH.Parameters.Results {
  public class Moment2d : IMoment2d {
    public Force Mx { get; internal set; }
    public Force My { get; internal set; }
    public Force Mxy { get; internal set; }
    public Force WoodArmerX { get; internal set; }
    public Force WoodArmerY { get; internal set; }

    internal Moment2d(Tensor2 result) {
      Mx = new Force(result.XX, ForceUnit.Newton);
      My = new Force(result.YY, ForceUnit.Newton);
      Mxy = new Force(result.XY, ForceUnit.Newton);

      //Mx + sgn(Mx)|Mxy|
      double wx = result.XX + (Math.Sign(result.XX) * Math.Abs(result.XY));
      WoodArmerX = new Force(wx, ForceUnit.Newton);

      //My + sgn(My)|Myx|
      double wy = result.YY + (Math.Sign(result.YY) * Math.Abs(result.XY));
      WoodArmerY = new Force(wy, ForceUnit.Newton);
    }
  }
}
