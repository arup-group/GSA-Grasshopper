using System;
using System.Linq;
using System.Text.RegularExpressions;

using Grasshopper.Kernel;

using GsaGH.Parameters;

using Rhino.Render.Fields;

namespace GsaGH.Helpers {
  public static class StringExtension {

    /// <summary>
    ///   <b>make string pascal cased</b>
    ///   <br></br>
    ///   example: Use_GPS_Settings
    ///   will be changed to: UseGpsSettings
    /// </summary>
    /// <returns>PascalCased name</returns>
    public static string ToPascalCase(this string value) {
      if (string.IsNullOrEmpty(value)) {
        return value;
      }

      if (!value.Contains(' ') && !value.Contains('_')) {
        return value[0].ToString().ToUpper() + value.Substring(1).ToLower();
      }

      string returnValue = string.Empty;
      string[] splittedText = value.ToLower().Replace(' ', '_').Split('_');

      return splittedText.Aggregate(returnValue,
        (current, word) => current + word[0].ToString().ToUpper() + word.Substring(1));
    }

    public static string ToSentenceCase(this string value) {
      value = Regex.Replace(value, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");
      return value.Replace("_", " ");
    }

    public static string TrimSpaces(this string value) {
      while (value.Contains("  ")) {
        value = value.Replace("  ", " ");
      }

      return value.Trim();
    }
  }
}
