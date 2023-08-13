using System;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaGridLine {
    public int Id { get; private set; }
    internal GridLine _gridLine;

    public GsaGridLine() { }

    internal GsaGridLine(int id, GridLine gridLine) {
      Id = id;
      _gridLine = gridLine;
    }

    public GsaGridLine Duplicate() {
      var dup = new GsaGridLine {
        Id = Id,
        _gridLine = _gridLine
      };

      return dup;
    }

    public override string ToString() {
      string id = Id > 0 ? $"{Id} " : string.Empty;
      string type = _gridLine.Shape == GridLineShape.Arc ? " Shape: Arc " : string.Empty;
      string s = $"{id}{_gridLine.Label}{type}X:{_gridLine.X} Y:{_gridLine.Y} Length:" +
        $"{_gridLine.Length} Orientation:{_gridLine.Theta1}";
      if (_gridLine.Shape == GridLineShape.Arc) {
        s.Replace("Orientation", "Theta1");
        s += " Theta2:" + _gridLine.Theta2;
      }

      return s;
    }
  }
}
