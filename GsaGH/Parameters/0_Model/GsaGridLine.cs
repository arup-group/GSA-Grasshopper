using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaGridLine {
    internal GridLine _gridLine;

    internal GsaGridLine(GridLine gridLine) {
      _gridLine = gridLine;
    }

    public GsaGridLine Duplicate() {
      var dup = new GsaGridLine(_gridLine);
      return dup;
    }

    public override string ToString() {
      string label = _gridLine.Label != "" ? $"{_gridLine.Label} " : string.Empty;
      string type = _gridLine.Shape == GridLineShape.Arc ? "Shape: Arc " : string.Empty;
      string s = $"{label}{type}X:{_gridLine.X} Y:{_gridLine.Y} Length:" +
        $"{_gridLine.Length} Orientation:{_gridLine.Theta1}°";
      if (_gridLine.Shape == GridLineShape.Arc) {
        s.Replace("Orientation", "Theta1");
        s += " Theta2:" + _gridLine.Theta2 + "°";
      }

      return s;
    }
  }
}
