using OasysUnits;

namespace GsaGH.Parameters.Results {
  public interface IStress1dDerived : IResultItem {
    Pressure ElasticShearY { get; }
    Pressure ElasticShearZ { get; }
    Pressure Torsional { get; }
    Pressure VonMises { get; }
  }
}
