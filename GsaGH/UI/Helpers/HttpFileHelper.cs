using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using HtmlAgilityPack;

namespace GsaGH.UI.Helpers {
  internal static class HttpFileHelper {
    public static string GetSafeFileName(string fileName) {
      if (string.IsNullOrWhiteSpace(fileName)) {
        throw new ArgumentException("File name must not be null or empty.", nameof(fileName));
      }

      if (Path.IsPathRooted(fileName)) {
        throw new ArgumentException("Rooted file paths are not allowed.", nameof(fileName));
      }

      char[] invalidChars = Path.GetInvalidFileNameChars();
      return fileName.Any(c => invalidChars.Contains(c)) ?
        throw new ArgumentException("File name contains invalid characters.", nameof(fileName)) : fileName;
    }

    public static FileEntry GetFileEntry(string url, HtmlNode link, List<string> allowedExtensions) {
      if (link == null) {
        throw new ArgumentNullException(nameof(link));
      }

      string href = link.GetAttributeValue("href", "");
      if (!allowedExtensions.Any(ext => href.EndsWith(ext, StringComparison.OrdinalIgnoreCase))) {
        return null;
      }

      Uri absoluteUri = href.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? new Uri(href) :
        new Uri(new Uri(url), href);
      string fileName = Path.GetFileName(absoluteUri.LocalPath);
      fileName = System.Net.WebUtility.HtmlDecode(Uri.UnescapeDataString(fileName ?? string.Empty)).Trim();
      if (string.IsNullOrEmpty(fileName)) {
        fileName = System.Net.WebUtility.HtmlDecode(link.InnerText.Trim());
      }
      string absoluteUrl = absoluteUri.ToString();
      int lastSlashIndex = absoluteUrl.LastIndexOf('/');
      if (lastSlashIndex >= 0) {
        absoluteUrl = absoluteUrl.Substring(0, lastSlashIndex + 1) + Uri.EscapeDataString(fileName);
      }


      return new FileEntry() {
        Name = fileName,
        Url = absoluteUrl,
      };
    }

    /// <summary>
    ///   Converts a file name into text safe for WinForms menu display.
    ///   Decodes HTML entities, normalizes whitespace, and escapes ampersands
    ///   so characters like '&' are shown literally instead of treated as mnemonics.
    /// </summary>
    /// <param name="fileName">Raw file name from HTML/link source.</param>
    /// <returns>Menu-safe text for <see cref="ToolStripMenuItem" />.</returns>
    public static string GetMenuText(string fileName) {
      string text = System.Net.WebUtility.HtmlDecode(fileName ?? string.Empty).Trim();
      text = text.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
      return text.Replace("&", "&&");
    }
  }
}
