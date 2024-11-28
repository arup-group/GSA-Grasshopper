using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;

using Rhino;
using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Helpers.GH {
  internal class DrawLegend {
    private const int DefaultTextHeight = 12;
    private const int DefaultBitmapWidth = 110;

    private static int _textHeight = DefaultTextHeight;
    private static int _leftBitmapEdge;

    private DrawLegend() { }

    /// <summary>
    ///   Draws the complete legend including the rectangle, title, values, and bottom text.
    /// </summary>
    public static void DrawLegendRectangle(IGH_PreviewArgs args, Legend legend, string title, string bottomText) {
      if (!legend.IsDisplayable()) {
        return;
      }

      InitializeDimensions(legend, args.Viewport.Bounds.Right);

      DrawBitmap(args, legend);
      DrawTitle(args, legend, title);
      DrawValues(args, legend);
      DrawBottomText(args, legend, bottomText);
    }

    /// <summary>
    ///   Draws the bitmap of the legend.
    /// </summary>
    private static void DrawBitmap(IGH_PreviewArgs args, Legend legend) {
      const int TopOffset = 20;
      int topPosition = (int)(TopOffset * legend.Scale);

      args.Display.DrawBitmap(new DisplayBitmap(legend.Bitmap), _leftBitmapEdge, topPosition);
    }

    /// <summary>
    ///   Draws the title of the legend.
    /// </summary>
    private static void DrawTitle(IGH_PreviewArgs args, Legend legend, string title) {
      const int TopOffset = 7;
      int topPosition = (int)(TopOffset * legend.Scale);

      args.Display.Draw2dText(title, Color.Black, new Point2d(_leftBitmapEdge, topPosition), false, _textHeight);
    }

    /// <summary>
    ///   Draws the list of values from the legend.
    /// </summary>
    private static void DrawValues(IGH_PreviewArgs args, Legend legend) {
      const int LeftOffset = 25;
      int leftEdge = _leftBitmapEdge + (int)(LeftOffset * legend.Scale);
      var zippedLists = legend.Values.Zip(legend.ValuePositionsY, (value, positionY) => new {
        value,
        positionY,
      });
      foreach (var pair in zippedLists) {
        args.Display.Draw2dText(pair.value, Color.Black, new Point2d(leftEdge, pair.positionY), false, _textHeight);
      }
    }

    /// <summary>
    ///   Draws the wrapped bottom text of the legend.
    /// </summary>
    private static void DrawBottomText(IGH_PreviewArgs args, Legend legend, string bottomText) {
      const int BottomOffset = 145;
      const int ExtraOffset = 20;
      int bitmapWidth = (int)(DefaultBitmapWidth * legend.Scale);
      int bitmapWidthWithOffset = bitmapWidth + (int)(ExtraOffset * legend.Scale);
      int topPosition = (int)(BottomOffset * legend.Scale);

      string wrappedText = WrapText(bottomText, bitmapWidthWithOffset);

      args.Display.Draw2dText(wrappedText, Color.Black, new Point2d(_leftBitmapEdge, topPosition), false, _textHeight);
    }

    /// <summary>
    ///   Initializes dimensions based on legend scale and viewport size.
    /// </summary>
    private static void InitializeDimensions(Legend legend, int viewportRightEdge) {
      int bitmapWidth = (int)(DefaultBitmapWidth * legend.Scale);
      _textHeight = (int)(DefaultTextHeight * legend.Scale);
      _leftBitmapEdge = viewportRightEdge - bitmapWidth;
    }

    private static string WrapText(string bottomText, int width) {
      var font = new Font(RhinoDoc.ActiveDoc.DimStyles.Current.Font.LogfontName, _textHeight);
      string wrappedText = TextWrapper.WrapText(bottomText, width, font);
      return wrappedText;
    }
  }
}
