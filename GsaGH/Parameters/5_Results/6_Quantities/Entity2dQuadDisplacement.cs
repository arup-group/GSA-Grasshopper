using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Entity2dQuadDisplacement : Entity2dQuadResult<IDisplacement> {
    internal Entity2dQuadDisplacement(ReadOnlyCollection<Double6> result) {
      int i = 0;
      Centre = new Displacement(result[i++]);
      Node1 = new Displacement(result[i++]);
      Node2 = new Displacement(result[i++]);
      Node3 = new Displacement(result[i++]);
      Node4 = new Displacement(result[i++]);
    }
  }
}
