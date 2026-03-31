using System;
using System.IO;
using System.Linq;

namespace GsaGH.UI.Helpers {
  internal class HttpNameExtractor {
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
  }
}
