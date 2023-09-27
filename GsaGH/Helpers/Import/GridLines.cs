using System.Collections.Generic;
using GsaAPI;
using GsaGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Helpers.Import {
  internal static class GridLines {
    internal static List<GsaGridLine> GetGridLines(GsaModel model) {
      var gridLines = new List<GsaGridLine>();
      foreach (GridLine gridLine in model.Model.GridLines().Values) {
        PolyCurve curve = GsaGridLine.ToCurve(gridLine);  
        gridLines.Add(new GsaGridLine(gridLine, curve));
      }
      return gridLines;
    }
  }
}
