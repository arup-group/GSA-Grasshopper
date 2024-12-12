﻿using System.Collections.Generic;
using System.Drawing;

namespace GsaGH.Helpers.GH {

  public static class TextWrapper {
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

      string[] splittedText = text.Split('\n');
      string caseName = splittedText[0];
      string withoutCaseName = splittedText[1];
      string[] words = withoutCaseName.Split(' ');
      var lines = new List<string>() {
        caseName,
      };
      string currentLine = "";
      foreach (string word in words) {
        string testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
        float testLineWidth = CalculateTextWidth(testLine, font, maxWidth);

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

    /// <summary>
    ///   Gets the width of the given text from the cache or measures and caches it.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">Font used for rendering text</param>
    /// <param name="maxWidth">Maximum possible width for text</param>
    /// <returns>The width of the text in pixels.</returns>
    private static float CalculateTextWidth(string text, Font font, int maxWidth) {
      var graphics = System.Drawing.Graphics.FromImage(new Bitmap(maxWidth, 1)); //we care only about width
      return graphics.MeasureString(text, font).Width;
    }
  }
}
