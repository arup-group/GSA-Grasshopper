using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GsaGH.Helpers.GH {
  public class Legend {
    public List<string> Values { get; set; } = new List<string>();
    public List<int> ValuesY { get; set; } = new List<int>();
    public bool ShowLegend { get; set; } = true;
    public Bitmap Bitmap { get; set; } = null;
    public double Scale { get; set; } = 1.0;

    public Legend() { }

    public Legend(List<string> values, List<int> valuesY, Bitmap bitmap, double scale, bool showLegend) {
      Values = values;
      ValuesY = valuesY;
      ShowLegend = showLegend;
      Bitmap = bitmap;
      Scale = scale;
    }

    public bool IsDisplayable() {
      return Values.Any() && ShowLegend;
    }

    public bool ToggleShowLegend() {
      ShowLegend = !ShowLegend;
      return ShowLegend;
    }

    public Bitmap CreateNewBitmap(int widthScaleFactor, int heightScaleFactor) {
      Bitmap = new Bitmap((int)(widthScaleFactor * Scale), (int)(heightScaleFactor * Scale));
      return Bitmap;
    }
  }
}
