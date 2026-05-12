using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using HtmlAgilityPack;

namespace GsaGH.UI.SampleFiles {
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
      fileName = Uri.UnescapeDataString(fileName ?? string.Empty).Trim();
      if (string.IsNullOrEmpty(fileName)) {
        fileName = link.InnerText.Trim();
      }

      return new FileEntry() {
        Name = fileName,
        Url = absoluteUri.ToString(),
      };
    }
  }
}
