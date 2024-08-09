using GsaAPI;

using OasysUnits;

using PressureUnit = OasysUnits.Units.PressureUnit;

namespace GsaGH.Parameters.Results {
  public class Stress1d : IStress1d {
    public Pressure Axial { get; internal set; }
    public Pressure BendingYyNegativeZ { get; internal set; }
    public Pressure BendingYyPositiveZ { get; internal set; }
    public Pressure BendingZzNegativeY { get; internal set; }
    public Pressure BendingZzPositiveY { get; internal set; }
    public Pressure CombinedC1 { get; internal set; }
    public Pressure CombinedC2 { get; internal set; }
    public Pressure ShearY { get; internal set; }
    public Pressure ShearZ { get; internal set; }

    internal Stress1d(StressResult1d result) {
      Axial = new Pressure(result.AxialStressA, PressureUnit.Pascal);
      BendingYyNegativeZ = new Pressure(result.BendingStressByNegativeZ, PressureUnit.Pascal);
      BendingYyPositiveZ = new Pressure(result.BendingStressByPositiveZ, PressureUnit.Pascal);
      BendingZzNegativeY = new Pressure(result.BendingStressBzNegativeY, PressureUnit.Pascal);
      BendingZzPositiveY = new Pressure(result.BendingStressBzPositiveY, PressureUnit.Pascal);
      CombinedC1 = new Pressure(result.CombinedStressC1, PressureUnit.Pascal);
      CombinedC2 = new Pressure(result.CombinedStressC2, PressureUnit.Pascal);
      ShearY = new Pressure(result.ShearStressSy, PressureUnit.Pascal);
      ShearZ = new Pressure(result.ShearStressSz, PressureUnit.Pascal);
    }
  }
}
