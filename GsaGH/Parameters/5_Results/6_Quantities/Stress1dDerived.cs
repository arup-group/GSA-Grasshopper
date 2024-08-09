using GsaAPI;

using OasysUnits;

using PressureUnit = OasysUnits.Units.PressureUnit;

namespace GsaGH.Parameters.Results {
  public class Stress1dDerived : IStress1dDerived {
    public Pressure ElasticShearY { get; internal set; }
    public Pressure ElasticShearZ { get; internal set; }
    public Pressure Torsional { get; internal set; }
    public Pressure VonMises { get; internal set; }

    internal Stress1dDerived(DerivedStressResult1d result) {
      ElasticShearY = new Pressure(result.ElasticShearStressSEy, PressureUnit.Pascal);
      ElasticShearZ = new Pressure(result.ElasticShearStressSEz, PressureUnit.Pascal);
      Torsional = new Pressure(result.TorsionalStressSt, PressureUnit.Pascal);
      VonMises = new Pressure(result.VonMisesStress, PressureUnit.Pascal);
    }
  }
}
