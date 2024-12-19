using System.Collections.Generic;
using System.Drawing;

namespace GsaGH.Helpers.GH {

  public static class TextWrapper {
    private static Bitmap _bitmap = new Bitmap(1, 1);
    private static System.Drawing.Graphics _graphics = System.Drawing.Graphics.FromImage(_bitmap);

    /// <summary>
    ///   Wraps the given text to fit within the specified maximum width.
    /// </summary>
    /// <param name="text">The text to wrap.</param>
    /// <param name="maxWidth">The maximum width in pixels.</param>
    /// <param name="font">The system font</param>
    /// <returns>Wrapped text with line breaks.</returns>
    public static string WrapText(string text, int maxWidth, Font font) {
      if (string.IsNullOrWhiteSpace(text)) {
        return string.Empty;
      }

      string[] words = GetTextToWrap(text, out List<string> lines);
      string currentLine = string.Empty;
      foreach (string word in words) {
        string testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
        float testLineWidth = CalculateTextWidth(testLine, font);

        if (testLineWidth > maxWidth) {
          lines.Add(currentLine);
          currentLine = word;
        } else {
          currentLine = testLine;
        }
      }

      if (!string.IsNullOrEmpty(currentLine)) {
        lines.Add(currentLine);
      }

      return string.Join("\n", lines).Trim();
    }

    private static string[] GetTextToWrap(string text, out List<string> lines) {
      lines = new List<string>();
      string[] splittedText = text.Split('\n');
      string textToWrap;

      if (splittedText.Length > 1) {
        lines.Add(splittedText[0]);
        textToWrap = splittedText[1];
      } else {
        textToWrap = text;
      }

      return textToWrap.Split(' ');
    }

    private static float CalculateTextWidth(string text, Font font) {
      int dpi = 96;
      var newFont = new Font(font.FontFamily, font.Size / (_graphics.DpiX / dpi));
      return _graphics.MeasureString(text, newFont).Width;
    }
  }
}
