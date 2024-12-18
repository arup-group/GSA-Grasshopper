using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;

using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Helpers.GH {

  public class ContourLegend {
    public int BitmapWidth {
      get => _width;
      set {
        if (value <= 0) {
          throw new ArgumentOutOfRangeException(nameof(_width), "Width must be greater than zero.");
        }

        _width = value;
      }
    }
    public Bitmap Bitmap { get; private set; }
    private readonly ContourLegendConfiguration _configuration;

    private int _width = 15;
    public int ActualBitmapWidth => (int)(BitmapWidth * _configuration.Scale);
    private int _leftBitmapEdge;
    private int _actualTextHeight;
    internal bool IsInvalidConfiguration = false; //only for tests

    public ContourLegend(ContourLegendConfiguration configuration, int bitmapInitialWidth = 15) {
      _configuration = configuration;
      BitmapWidth = bitmapInitialWidth;
    }

    /// <summary>
    ///   Draws the complete legend including the rectangle, title, values, and bottom text.
    /// </summary>
    public void DrawLegendRectangle(
      IGH_PreviewArgs args, string title, string bottomText,
      List<(int startY, int endY, Color gradientColor)> gradients) {
      if (!_configuration.IsLegendDisplayable()) {
        IsInvalidConfiguration = true;
        return;
      }

      InitializeDimensions(args.Viewport.Bounds.Right);

      // Step 1: Apply all gradients to the bitmap
      foreach ((int startY, int endY, Color gradientColor) in gradients) {
        DrawGradient(startY, endY, gradientColor);
      }

      //Step2 Draw other elements of the legend
      DrawBitmap(args);
      DrawTitle(args, title);
      DrawValues(args);
      DrawBottomText(args, bottomText);
    }

    private void DrawGradient(int startY, int endY, Color gradientColor) {
      if (startY < 0 || endY > Bitmap.Height || startY >= endY) {
        throw new ArgumentOutOfRangeException(nameof(startY), "Invalid start or end positions for the gradient.");
      }

      for (int y = startY; y < endY; y++) {
        for (int x = 0; x < Bitmap.Width; x++) {
          Bitmap.SetPixel(x, Bitmap.Height - y - 1, gradientColor);
        }
      }
    }

    private void DrawBitmap(IGH_PreviewArgs args) {
      const int TopOffset = 20;
      int topPosition = GetScaledValue(TopOffset);

      args.Display.DrawBitmap(new DisplayBitmap(Bitmap), _leftBitmapEdge, topPosition);
    }

    private void DrawTitle(IGH_PreviewArgs args, string title) {
      const int TopOffset = 7;
      int topPosition = GetScaledValue(TopOffset);

      args.Display.Draw2dText(title, Color.Black, new Point2d(_leftBitmapEdge, topPosition), false, _actualTextHeight);
    }

    private void DrawValues(IGH_PreviewArgs args) {
      const int LeftOffset = 25;
      int leftEdge = _leftBitmapEdge + GetScaledValue(LeftOffset);
      var zippedLists = _configuration.Values.Zip(_configuration.ValuePositionsY, (value, positionY) => new {
        value,
        positionY,
      });
      foreach (var pair in zippedLists) {
        args.Display.Draw2dText(pair.value, Color.Black, new Point2d(leftEdge, pair.positionY), false,
          _actualTextHeight);
      }
    }

    private void DrawBottomText(IGH_PreviewArgs args, string bottomText) {
      const int BottomOffset = 145;
      const int ExtraOffset = 10;
      int textMaxWidth = _configuration.ActualWidth - GetScaledValue(ExtraOffset);
      int topPosition = GetScaledValue(BottomOffset);

      string wrappedText = WrapText(bottomText, textMaxWidth);
      args.Display.Draw2dText(wrappedText, Color.Black, new Point2d(_leftBitmapEdge, topPosition), false,
        _actualTextHeight);
    }

    private void InitializeDimensions(int viewportEdge) {
      _actualTextHeight = GetScaledSystemFont().Height;
      _leftBitmapEdge = viewportEdge - _configuration.ActualWidth;
      Bitmap = new Bitmap(ActualBitmapWidth, _configuration.ActualHeight);
    }

    private string WrapText(string bottomText, int maxTextWidth) {
      return TextWrapper.WrapText(bottomText, maxTextWidth, GetScaledSystemFont());
    }

    private int GetScaledValue(int value) {
      return (int)(value * _configuration.Scale);
    }

    private Font GetScaledSystemFont() {
      Font systemFont = SystemFonts.DefaultFont;
      float fontSize = (float)(systemFont.Size * _configuration.Scale);
      return new Font(systemFont.FontFamily, fontSize);
    }
  }
}
