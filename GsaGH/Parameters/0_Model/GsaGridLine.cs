using GsaAPI;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  public class GsaGridLine {
    internal GridLine _gridLine;
    internal PolyCurve _curve;

    internal int _pattern = 0;

    internal GsaGridLine(GridLine gridLine, PolyCurve curve, int pattern = 0) {
      _gridLine = gridLine;
      _curve = curve;
      _pattern = pattern;
    }

    public GsaGridLine Clone() {
      var gridLine = new GridLine(_gridLine.Label) {
        X = _gridLine.X,
        Y = _gridLine.Y,
        Length = _gridLine.Length,
        Shape = _gridLine.Shape,
        Theta1 = _gridLine.Theta1,
      };
      if (_gridLine.Shape == GridLineShape.Arc) {
        gridLine.Theta2 = _gridLine.Theta2;
      }
      PolyCurve curve = _curve.DuplicatePolyCurve();
      var dup = new GsaGridLine(gridLine, curve, _pattern);
      return dup;
    }

    public GsaGridLine Duplicate() {
      return this;
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
