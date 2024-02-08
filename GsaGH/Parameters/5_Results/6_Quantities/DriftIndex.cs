using GsaAPI;

namespace GsaGH.Parameters.Results {
  public class DriftIndex : IDrift<double> {
    public double X { get; internal set; }
    public double Y { get; internal set; }
    public double Xy { get; internal set; }

    internal DriftIndex(AssemblyDriftIndicesResult result) {
      X = result.DIx;
      Y = result.DIy;
      Xy = result.DIxy;
    }
  }
}
