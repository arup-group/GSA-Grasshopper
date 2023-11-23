using GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters.Results {
  public class Force2d : IForce2d {
    public ForcePerLength Nx { get; internal set; }
    public ForcePerLength Ny { get; internal set; }
    public ForcePerLength Nxy { get; internal set; }

    internal Force2d(Tensor2 result) {
      Nx = new ForcePerLength(result.XX, ForcePerLengthUnit.NewtonPerMeter);
      Ny = new ForcePerLength(result.YY, ForcePerLengthUnit.NewtonPerMeter);
      Nxy = new ForcePerLength(result.XY, ForcePerLengthUnit.NewtonPerMeter);
    }
  }
}
