using System.Linq;
using System.Text.RegularExpressions;

namespace GsaGH.Helpers.GH {
  public static class StringExtension {

    /// <summary>
    ///   <b>make string pascal cased</b>
    ///   <br></br>
    ///   example: Use_GPS_Settings
    ///   will be changed to: UseGpsSettings
    /// </summary>
    /// <returns>PascalCased name</returns>
    public static string ToPascalCase(this string value) {
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
  }
}
