using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class Entity2dQuad8Displacement : Entity2dQuad8Result<IDisplacement> {
    internal Entity2dQuad8Displacement(ReadOnlyCollection<Double6> result) {
      int i = 0;
      Centre = new Displacement(result[i++]);
      Node1 = new Displacement(result[i++]);
      Node2 = new Displacement(result[i++]);
      Node3 = new Displacement(result[i++]);
      Node4 = new Displacement(result[i++]);
      Node5 = new Displacement(result[i++]);
      Node6 = new Displacement(result[i++]);
      Node7 = new Displacement(result[i++]);
      Node8 = new Displacement(result[i++]);
    }
  }
}
