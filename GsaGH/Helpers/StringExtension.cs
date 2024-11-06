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
    public static bool FindString(string baseString, string stringToFind) {
      foreach (string str in baseString.Split(new char[] { ' ', '\t', ')', '(' })) {
        if (str.Equals(stringToFind)) {
          return true;
        }
      }
      return false;
    }

    public static GsaBool6 ParseBool6(object data, bool isRelease = false) {
      var bool6 = new GsaBool6();
      if (GH_Convert.ToBoolean(data, out bool mybool, GH_Conversion.Both)) {
        bool6.X = mybool;
        bool6.Y = mybool;
        bool6.Z = mybool;
        bool6.Xx = mybool;
        bool6.Yy = mybool;
        bool6.Zz = mybool;
        return bool6;
      }
      if (GH_Convert.ToString(data, out string mystring, GH_Conversion.Both)) {
        mystring = mystring.Trim().ToLower();
        if (mystring == "free") {
          bool6.X = isRelease;
          bool6.Y = isRelease;
          bool6.Z = isRelease;
          bool6.Xx = isRelease;
          bool6.Yy = isRelease;
          bool6.Zz = isRelease;
          return bool6;
        } else if (IsPinned(mystring)) {
          bool6.X = !isRelease;
          bool6.Y = !isRelease;
          bool6.Z = !isRelease;
          bool6.Xx = isRelease;
          bool6.Yy = isRelease;
          bool6.Zz = isRelease;
          return bool6;
        } else if (IsFixed(mystring)) {
          bool6.X = !isRelease;
          bool6.Y = !isRelease;
          bool6.Z = !isRelease;
          bool6.Xx = !isRelease;
          bool6.Yy = !isRelease;
          bool6.Zz = !isRelease;
          return bool6;
        } else if (IsRelease(mystring) || IsHinged(mystring) || mystring == "charnier") {
          bool6.X = isRelease;
          bool6.Y = isRelease;
          bool6.Z = isRelease;
          bool6.Xx = isRelease;
          bool6.Yy = !isRelease;
          bool6.Zz = !isRelease;
          return bool6;
        } else if (mystring.Length == 6) {
          return ConvertString(mystring, isRelease);
        }
      }
      throw new InvalidCastException($"Data conversion failed from {data.GetTypeName()} to Bool6");
    }

    private static bool IsRelease(string input) {
      return input == "release" || input == "released";
    }

    private static bool IsHinged(string input) {
      return input == "hinge" || input == "hinged";
    }

    private static bool IsFixed(string input) {
      return input == "fix" || input == "fixed";
    }

    private static bool IsPinned(string input) {
      return input == "pin" || input == "pinned";
    }

    private static GsaBool6 ConvertString(string txt, bool isRelease) {
      int i = 0;
      var b = new GsaBool6() {
        X = ConvertChar(txt[i++], isRelease),
        Y = ConvertChar(txt[i++], isRelease),
        Z = ConvertChar(txt[i++], isRelease),
        Xx = ConvertChar(txt[i++], isRelease),
        Yy = ConvertChar(txt[i++], isRelease),
        Zz = ConvertChar(txt[i++], isRelease),
      };
      return b;
    }
    private static bool ConvertChar(char character, bool isRelease) {
      return character switch {
        'r' => isRelease,
        'f' => !isRelease,
        _ => throw new ArgumentException(
                    $"Unable to convert string to Bool6, character {character} not recognised"),
      };
    }
  }
}
