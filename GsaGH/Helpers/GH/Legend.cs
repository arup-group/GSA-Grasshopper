using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GsaGH.Helpers.GH {
  public class Legend {
    public List<string> Values { get; set; } = new List<string>();
    public List<int> ValuesPositionY { get; set; } = new List<int>();
    public bool Visible { get; set; } = true;
    public Bitmap Bitmap { get; set; } = null;
    public double Scale { get; set; } = 1.0;

    public Legend() { }

    public Legend(List<string> values, List<int> valuesPositionY, Bitmap bitmap, double scale, bool visible) {
      Values = values;
      ValuesPositionY = valuesPositionY;
      Visible = visible;
      Bitmap = bitmap;
      Scale = scale;
    }

    public bool IsDisplayable() {
      return Values.Any() && Visible;
    }

    public bool ToggleShowLegend() {
      Visible = !Visible;
      return Visible;
    }

    public Bitmap CreateNewBitmap(int widthScaleFactor, int heightScaleFactor) {
      Bitmap = new Bitmap((int)(widthScaleFactor * Scale), (int)(heightScaleFactor * Scale));
      return Bitmap;
    }
  }
}
