using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Entity2dTriDisplacement : Entity2dTriResult<IDisplacement> {
    internal Entity2dTriDisplacement(ReadOnlyCollection<Double6> result) {
      int i = 0;
      Centre = new Displacement(result[i++]);
      Node1 = new Displacement(result[i++]);
      Node2 = new Displacement(result[i++]);
      Node3 = new Displacement(result[i++]);
    }
  }
}
