using System.Collections.Generic;

namespace GsaGH.Parameters.Results {
  public interface IBeamDisplacement : IResultItem {
    ICollection<IDisplacement> Displacements { get; }
  }
}
