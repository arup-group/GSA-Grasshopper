using GsaAPI;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters.Results {
  public class DriftIndex : IDrift<Ratio> {
    public Ratio X { get; internal set; }
    public Ratio Y { get; internal set; }
    public Ratio Xy { get; internal set; }

    internal DriftIndex(AssemblyDriftIndexResult result) {
      X = new Ratio(result.X, RatioUnit.DecimalFraction);
      Y = new Ratio(result.Y, RatioUnit.DecimalFraction);
      Xy = new Ratio(result.XY, RatioUnit.DecimalFraction);
    }
  }
}
