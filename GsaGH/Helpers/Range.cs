namespace GsaGH.Helpers {
  public struct Range {
    public int Min;
    public int Max;
    public int Length;
    public Range(int min, int max) {
      Min = min;
      Max = max;
      Length = max - min;
    }
  }
}
