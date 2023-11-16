using OasysUnits;

namespace GsaGH.Parameters.Results {
  public interface IStress1d : IResultItem {
    Pressure Axial { get; }
    Pressure BendingYyNegativeZ { get; }
    Pressure BendingYyPositiveZ { get; }
    Pressure BendingZzNegativeY { get; }
    Pressure BendingZzPositiveY { get; }
    Pressure CombinedC1 { get; }
    Pressure CombinedC2 { get; }
    Pressure ShearY { get; }
    Pressure ShearZ { get; }
  }
}
