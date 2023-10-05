using GsaAPI;

namespace GsaGH.Parameters {
  public interface IGsaResult {
    GsaResultValues GetNodeDisplacements(string nodelist, LengthUnit lengthUnit);
  }
}
