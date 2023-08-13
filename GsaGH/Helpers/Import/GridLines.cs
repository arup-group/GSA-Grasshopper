using GsaAPI;
using GsaGH.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGH.Helpers.Import {
  internal class GridLines {
    internal static List<GsaGridLine> GetGridLines(GsaModel model) {
      var gridLines = new List<GsaGridLine>();
      foreach (KeyValuePair<int, GridLine> gridLine in model.Model.GridLines()) {
        gridLines.Add(new GsaGridLine(gridLine.Key, gridLine.Value));
      }
      return gridLines;
    }
  }
}
