using System.Collections.Generic;
using System.Drawing;

using Rhino;

namespace GsaGH.Helpers.GH {

  public static class TextWrapper {
    /// <summary>
    ///   Cache for storing text widths.
    ///   Key = Text, font size;
    ///   Value = text width;
    /// </summary>
    private static readonly Dictionary<(string, float), float> textWidthCache
      = new Dictionary<(string, float), float>();

    /// <summary>
    ///   Wraps the given text to fit within the specified maximum width.
    /// </summary>
    /// <param name="text">The text to wrap.</param>
    /// <param name="maxWidth">The maximum width in pixels.</param>
    /// <param name="fontSize">The font size of the text</param>
    /// <returns>Wrapped text with line breaks.</returns>
    public static string WrapText(string text, int maxWidth, int fontSize) {
      if (string.IsNullOrWhiteSpace(text)) {
        return string.Empty;
      }

      string[] words = text.Split(' ');
      var lines = new List<string>();
      string currentLine = "";
      var font = new Font(GetFontName(), fontSize);
      foreach (string word in words) {
        string testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
        float testLineWidth = GetCachedTextWidth(testLine, font, maxWidth);

        if (testLineWidth > maxWidth) {
          lines.Add(currentLine);
          textWidthCache[(text, font.Size)] = testLineWidth;
          currentLine = word;
        } else {
          currentLine = testLine;
        }
      }

      if (!string.IsNullOrEmpty(currentLine)) {
        lines.Add(currentLine);
      }

      return string.Join("\n", lines);
    }

    /// <summary>
    ///   Gets the width of the given text from the cache or measures and caches it.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">Font used for rendering text</param>
    /// <param name="maxWidth">Maximum possible width for text</param>
    /// <returns>The width of the text in pixels.</returns>
    private static float GetCachedTextWidth(string text, Font font, int maxWidth) {
      if (textWidthCache.TryGetValue((text, font.Size), out float cachedWidth)) {
        return cachedWidth;
      }

      var graphics = System.Drawing.Graphics.FromImage(new Bitmap(maxWidth, 1)); //we care only about width
      float width = graphics.MeasureString(text, font).Width;

      return width;
    }

    public static string GetFontName(RhinoDoc testPurpose = null) {
      try {
        RhinoDoc activeRhino = testPurpose ?? RhinoDoc.ActiveDoc;
        return activeRhino.DimStyles.Current.Font.FamilyName;
      } catch { //will be catched only by tests
        return "Arial";
      }
    }
  }
}
