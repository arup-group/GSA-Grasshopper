using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;

using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Helpers.GH {

  internal class ContourLegend : IContourLegend {
    private readonly IContourLegendConfiguration _configuration;

    private const int DefaultTextHeight = 12;
    private const int DefaultBitmapWidth = 110;

    private int _textHeight = DefaultTextHeight;
    private int _leftBitmapEdge;
    private bool _isDrawLegendCalled = false;

    public ContourLegend(IContourLegendConfiguration configuration) {
      _configuration = configuration;
    }

    /// <summary>
    ///   Draws the complete legend including the rectangle, title, values, and bottom text.
    /// </summary>
    public void DrawLegendRectangle(
      IGH_PreviewArgs args, string title, string bottomText,
      List<(int startY, int endY, Color gradientColor)> gradients) {
      if (!_configuration.IsLegendDisplayable()) {
        return;
      }

      _isDrawLegendCalled = true;
      InitializeDimensions(args.Viewport.Bounds.Right);

      // Step 1: Apply all gradients to the bitmap
      foreach ((int startY, int endY, Color gradientColor) in gradients) {
        DrawGradientLegend(startY, endY, gradientColor);
      }

      //Step2 Draw other elements of the legend
      DrawBitmap(args);
      DrawTitle(args, title);
      DrawValues(args);
      DrawBottomText(args, bottomText);

      _isDrawLegendCalled = false;
    }

    public void DrawGradientLegend(int startY, int endY, Color gradientColor) {
      if (startY < 0 || endY > _configuration.Bitmap.Height || startY >= endY) {
        throw new ArgumentOutOfRangeException(nameof(startY), "Invalid start or end positions for the gradient.");
      }

      for (int y = startY; y < endY; y++) {
        for (int x = 0; x < _configuration.Bitmap.Width; x++) {
          _configuration.Bitmap.SetPixel(x, _configuration.Bitmap.Height - y - 1, gradientColor);
        }
      }
    }

    public void DrawBitmap(IGH_PreviewArgs args) {
      EnsureDimensionsInitialized(args);
      const int TopOffset = 20;
      int topPosition = CalculateScaledOffset(TopOffset);

      args.Display.DrawBitmap(new DisplayBitmap(_configuration.Bitmap), _leftBitmapEdge, topPosition);
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
      var zippedLists = _configuration.Values.Zip(_configuration.ValuePositionsY, (value, positionY) => new {
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
      return TextWrapper.WrapText(bottomText, width, _textHeight);
    }

    private int CalculateScaledOffset(int value) {
      return (int)(value * _configuration.Scale);
    }
  }
}
