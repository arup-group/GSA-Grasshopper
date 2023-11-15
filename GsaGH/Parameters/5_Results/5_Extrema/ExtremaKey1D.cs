namespace GsaGH.Parameters.Results {
  public class ExtremaKey1D {
    public int Id { get; internal set; }
    public int Permutation { get; internal set; }
    public double Position { get; internal set; }

    public ExtremaKey1D(int id, double position, int permutation = 0) {
      Id = id;
      Position = position;
      Permutation = permutation;
    }
  }
}
