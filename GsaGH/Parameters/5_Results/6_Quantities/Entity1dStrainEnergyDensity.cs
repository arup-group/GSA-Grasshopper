using GsaAPI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public class Entity1dStrainEnergyDensity : Entity1dResult<double, IEnergyDensity> {
    internal Entity1dStrainEnergyDensity(
      ReadOnlyCollection<double> result, ReadOnlyCollection<double> positions)
      : base(result, positions, (x) => new StrainEnergyDensity(x)) { }

    private Entity1dStrainEnergyDensity(IDictionary<double, IEnergyDensity> results) : base(results) { }

    public override IEntity1dQuantity<IEnergyDensity> TakePositions(ICollection<double> positions) {
      return new Entity1dStrainEnergyDensity(TakePositions(this, positions));
    }
  }
}
