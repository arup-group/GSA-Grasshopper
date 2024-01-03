namespace GsaGH.Parameters.Results {
  public enum EnvelopeMethod {
    Maximum,
    Minimum,
    Absolute,
    SignedAbsolute
  }

  internal static partial class ResultsUtility {
    internal static string EnvelopeMethodAbbreviated(EnvelopeMethod method) {
      return method switch {
        EnvelopeMethod.Maximum => "Max of env.",
        EnvelopeMethod.Minimum => "Min of env.",
        EnvelopeMethod.Absolute => "Abs of env.",
        EnvelopeMethod.SignedAbsolute => "Sign. abs. of env.",
        _ => method.ToString(),
      };
    }
  }
}
