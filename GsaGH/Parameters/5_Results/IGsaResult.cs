using OasysUnits.Units;

namespace GsaGH.Parameters {
  public interface IGsaResult {
    CaseType Type { get; }

    GsaResultsValues GetNodeDisplacementValues(string nodelist, LengthUnit lengthUnit);

    // etc.
  }
}
