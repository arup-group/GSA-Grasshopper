using OasysUnits;

namespace GsaGH.Parameters.Results {
  public interface IDerivedStress1d : IResultItem {
    Pressure ElasticShearY { get; }
    Pressure ElasticShearZ { get; }
    Pressure Torsional { get; }
    Pressure VonMises { get; }
  }
}
