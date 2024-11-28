﻿using System.Drawing;

using Grasshopper.Kernel;

using Rhino;
using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Helpers.GH {
  internal class DrawContour {
    private const int _defaultTextHeight = 12;
    private static int _textHeight = _defaultTextHeight;
    private static int _leftBitmapEdge;
    private DrawContour() { }

    public static void DrawViewportWires(IGH_PreviewArgs args, Legend legend, string title, string bottomText) {
      if (!legend.IsDisplayable()) {
        return;
      }

      int bitmapWidth = (int)(110 * legend.Scale);
      _textHeight = (int)(_defaultTextHeight * legend.Scale);
      _leftBitmapEdge = args.Viewport.Bounds.Right - bitmapWidth;

      DrawBitmap(args, legend);
      DrawTitle(args, legend, title);
      DrawValues(args, legend);
      DrawBottomText(args, legend, bottomText, bitmapWidth);
    }

    private static void DrawBottomText(IGH_PreviewArgs args, Legend legend, string bottomText, int bitmapWidth) {
      string wrappedText = WrapText(bottomText, bitmapWidth);
      const int disctanceFromTopEdge = 145;
      int topPosition = (int)(disctanceFromTopEdge * legend.Scale);

      args.Display.Draw2dText(wrappedText, Color.Black, new Point2d(_leftBitmapEdge, topPosition), false, _textHeight);
    }

    private static void DrawValues(IGH_PreviewArgs args, Legend legend) {
      const int disctanceFromLeftBitmapEdge = 25;
      int leftEdgeOfText = _leftBitmapEdge + (int)(disctanceFromLeftBitmapEdge * legend.Scale);

      for (int i = 0; i < legend.Values.Count; i++) {
        args.Display.Draw2dText(legend.Values[i], Color.Black, new Point2d(leftEdgeOfText, legend.ValuesPositionY[i]),
          false, _textHeight);
      }
    }

    private static void DrawTitle(IGH_PreviewArgs args, Legend legend, string title) {
      const int disctanceFromTopViewportEdge = 7;
      int topPosition = (int)(disctanceFromTopViewportEdge * legend.Scale);

      args.Display.Draw2dText(title, Color.Black, new Point2d(_leftBitmapEdge, topPosition), false, _textHeight);
    }

    private static void DrawBitmap(IGH_PreviewArgs args, Legend legend) {
      const int disctanceFromTopViewportEdge = 20;
      int topPosition = (int)(disctanceFromTopViewportEdge * legend.Scale);
      args.Display.DrawBitmap(new DisplayBitmap(legend.Bitmap), _leftBitmapEdge, topPosition);
    }

    private static string WrapText(string bottomText, int bitmapWidth) {
      var font = new Font(RhinoDoc.ActiveDoc.DimStyles.Current.Font.LogfontName, _textHeight);
      string wrappedText = TextWrapper.WrapText(bottomText, bitmapWidth, font);
      return wrappedText;
    }
  }
}
