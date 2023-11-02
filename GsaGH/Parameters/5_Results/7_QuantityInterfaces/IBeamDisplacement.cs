using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IBeamDisplacement : IResultItem {
    IDictionary<double, IDisplacement> Displacements { get; }
  }
}
