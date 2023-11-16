namespace GsaGH.Parameters.Results {
  public class ExtremaKey1d {
    public int Id { get; internal set; }
    public int Permutation { get; internal set; }
    public double Position { get; internal set; }

    public ExtremaKey1d(int id, double position, int permutation = 0) {
      Id = id;
      Position = position;
      Permutation = permutation;
    }
  }
}
