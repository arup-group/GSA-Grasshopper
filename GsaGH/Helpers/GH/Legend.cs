using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;

using Rhino;
using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Helpers.GH {
  internal class Legend {
    public LegendConfiguration Configuration { get; private set; }

    private const int DefaultTextHeight = 12;
    private const int DefaultBitmapWidth = 110;

    private int _textHeight = DefaultTextHeight;
    private int _leftBitmapEdge;
    private bool _isDrawLegendCalled = false;

    public Legend() {
      Configuration = new LegendConfiguration();
    }

    /// <summary>
    ///   Draws the complete legend including the rectangle, title, values, and bottom text.
    /// </summary>
    public void DrawLegendRectangle(IGH_PreviewArgs args, string title, string bottomText) {
      if (!IsDrawable()) {
        return;
      }

      _isDrawLegendCalled = true;
      InitializeDimensions(args.Viewport.Bounds.Right);

      DrawBitmap(args);
      DrawTitle(args, title);
      DrawValues(args);
      DrawBottomText(args, bottomText);

      _isDrawLegendCalled = false;
    }

    public void DrawBitmap(IGH_PreviewArgs args) {
      EnsureDimensionsInitialized(args);
      const int TopOffset = 20;
      int topPosition = CalculateScaledOffset(TopOffset);

      args.Display.DrawBitmap(new DisplayBitmap(Configuration.Bitmap), _leftBitmapEdge, topPosition);
    }

    public void DrawTitle(IGH_PreviewArgs args, string title) {
      EnsureDimensionsInitialized(args);
      const int TopOffset = 7;
      int topPosition = CalculateScaledOffset(TopOffset);

      args.Display.Draw2dText(title, Color.Black, new Point2d(_leftBitmapEdge, topPosition), false, _textHeight);
    }

    public void DrawValues(IGH_PreviewArgs args) {
      EnsureDimensionsInitialized(args);
      const int LeftOffset = 25;
      int leftEdge = _leftBitmapEdge + CalculateScaledOffset(LeftOffset);
      var zippedLists = Configuration.Values.Zip(Configuration.ValuePositionsY, (value, positionY) => new {
        value,
        positionY,
      });
      foreach (var pair in zippedLists) {
        args.Display.Draw2dText(pair.value, Color.Black, new Point2d(leftEdge, pair.positionY), false, _textHeight);
      }
    }

    public void DrawBottomText(IGH_PreviewArgs args, string bottomText) {
      EnsureDimensionsInitialized(args);
      const int BottomOffset = 145;
      const int ExtraOffset = 20;
      int bitmapWidth = CalculateScaledOffset(DefaultBitmapWidth);
      int bitmapWidthWithOffset = bitmapWidth + CalculateScaledOffset(ExtraOffset);
      int topPosition = CalculateScaledOffset(BottomOffset);

      string wrappedText = WrapText(bottomText, bitmapWidthWithOffset);
      args.Display.Draw2dText(wrappedText, Color.Black, new Point2d(_leftBitmapEdge, topPosition), false, _textHeight);
    }

    private bool IsDrawable() {
      return Configuration != null && Configuration.IsLegendDisplayable();
    }

    private void InitializeDimensions(int viewportEdge) {
      _textHeight = CalculateScaledOffset(DefaultTextHeight);
      _leftBitmapEdge = viewportEdge - CalculateScaledOffset(DefaultBitmapWidth);
    }

    /// <summary>
    ///   Ensures dimensions are initialized for individual drawing calls.
    /// </summary>
    private void EnsureDimensionsInitialized(IGH_PreviewArgs args) {
      if (!_isDrawLegendCalled) {
        InitializeDimensions(args.Viewport.Bounds.Right);
      }
    }

    private string WrapText(string bottomText, int width) {
      var font = new Font(RhinoDoc.ActiveDoc.DimStyles.Current.Font.LogfontName, _textHeight);
      return TextWrapper.WrapText(bottomText, width, font);
    }

    private int CalculateScaledOffset(int value) {
      return (int)(value * Configuration.Scale);
    }
  }
}
