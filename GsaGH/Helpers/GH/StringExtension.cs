using System.Linq;

namespace GsaGH.Helpers.GH {

  public static class StringExtension {

    #region Public Methods
    /// <summary>
    /// <b>make string pascal cased</b>
    /// <br></br>
    /// example: Use_GPS_Settings
    /// will be changed to: UseGpsSettings
    /// </summary>
    /// <returns>PascalCased name</returns>
    public static string ToPascalCase(this string value) {
      string returnValue = string.Empty;
      string[] splittedText = value.ToLower().Replace(' ', '_').Split('_');

      return splittedText.Aggregate(returnValue,
        (current, word) => current
          + (word[0]
              .ToString()
              .ToUpper()
            + word.Substring(1)));
    }

    #endregion Public Methods
  }
}
