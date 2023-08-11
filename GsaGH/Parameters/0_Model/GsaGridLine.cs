using System;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaGridLine {
    public Guid Guid { get; set; } = Guid.NewGuid();
    public int Id => _id;
#pragma warning disable IDE0032 // Use auto property
    private int _id;
#pragma warning restore IDE0032 // Use auto property
    private GridLine _gridLine;

    public GsaGridLine() { }

    internal GsaGridLine(int id, GridLine gridLine) {
      _id = id;
      _gridLine = gridLine;
    }

    public GsaGridLine Duplicate() {
      var dup = new GsaGridLine {
        Guid = new Guid(Guid.ToString()),
        _id = Id,
        _gridLine = _gridLine
      };

      return dup;
    }

    public override string ToString() {
      string s = "ID:" + Id + " Label:" + _gridLine.Label + " Shape:" + _gridLine.Shape + " X:" + _gridLine.X + " Y:" + _gridLine.Y + " Length:" + _gridLine.Length + " Theta1:" + _gridLine.Theta1;
      if (_gridLine.Shape == GridLineShape.Arc) {
        s += " Theta2:" + _gridLine.Theta2;
      }
      return s;
    }
  }
}
