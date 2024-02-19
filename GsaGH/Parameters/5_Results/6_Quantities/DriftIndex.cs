using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class DriftIndex : IDrift<double> {
    public double X { get; internal set; }
    public double Y { get; internal set; }
    public double Xy { get; internal set; }

    internal DriftIndex(AssemblyDriftIndexResult result) {
      X = result.X;
      Y = result.Y;
      Xy = result.XY;
    }
  }
}
