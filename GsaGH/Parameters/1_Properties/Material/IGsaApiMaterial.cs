using GsaAPI;

namespace GsaGH.Parameters {
  internal interface IGsaApiMaterial : IGsaMaterial {
    AnalysisMaterial AnalysisMaterial { get; set; }
  }
}
