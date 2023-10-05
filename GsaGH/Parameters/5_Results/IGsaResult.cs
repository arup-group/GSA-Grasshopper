using OasysUnits.Units;

namespace GsaGH.Parameters {
  public interface IGsaResult {
    CaseType Type { get; }

    GsaResultValues GetNodeDisplacementValues(string nodelist, LengthUnit lengthUnit);

    // etc.
  }
}
