using System;
using GsaAPI;

namespace GsaGH.Parameters {
  public interface IGsaMaterial {
    string Name { get; set; }
    int Id { get; set; }
    Guid Guid { get; }
    AnalysisMaterial AnalysisMaterial { get; set; }
    MaterialType Type { get; }
    IGsaMaterial Duplicate();
  }
}
