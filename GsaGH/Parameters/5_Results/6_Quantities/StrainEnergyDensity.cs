using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters.Results {
  public class StrainEnergyDensity : IEnergyDensity {
    public Energy EnergyDensity { get; internal set; }

    internal StrainEnergyDensity(double result) {
      EnergyDensity = new Energy(result, EnergyUnit.Joule);
    }
  }
}
