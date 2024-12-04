using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;

namespace GsaGH.Helpers {
  public interface IContourLegend {
    public void DrawLegendRectangle(
      IGH_PreviewArgs args, string title, string bottomText,
      List<(int startY, int endY, Color gradientColor)> gradients);
  }
}
