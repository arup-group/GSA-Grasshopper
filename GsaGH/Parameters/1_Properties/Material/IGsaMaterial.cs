using System;
using GsaAPI;

namespace GsaGH.Parameters {
  public interface IGsaMaterial {
    string Name { get; set; }
    int Id { get; set; }
    Guid Guid { get; }
    AnalysisMaterial AnalysisMaterial { get; set; }
    MatType Type { get; }
    IGsaMaterial Duplicate();
  }
}
