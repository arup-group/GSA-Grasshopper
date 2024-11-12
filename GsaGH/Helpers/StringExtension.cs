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

    public static GsaBool6 ParseBool6(object data) {
      var bool6 = new GsaBool6();
      if (GH_Convert.ToBoolean(data, out bool mybool, GH_Conversion.Both)) {
        bool6.X = mybool;
        bool6.Y = mybool;
        bool6.Z = mybool;
        bool6.Xx = mybool;
        bool6.Yy = mybool;
        bool6.Zz = mybool;
      } else {
        throw new InvalidCastException($"Data  conversion failed from '{data}' of type {data.GetTypeName()} to Bool6");
      }

      return bool6;
    }

    public static GsaBool6 ParseRestrain(object data) {
      try {
        return ParseBool6(data);
      } catch (InvalidCastException) {
        return !ParseReleaseInternal(data);
      }
    }

    public static GsaBool6 ParseRelease(object data) {
      try {
        return ParseBool6(data);
      } catch (InvalidCastException) {
        return ParseReleaseInternal(data);
      }
    }

    private static GsaBool6 ParseReleaseInternal(object data) {
      var bool6 = new GsaBool6();
      if (GH_Convert.ToString(data, out string mystring, GH_Conversion.Both)) {
        mystring = mystring.Trim().ToLower();
        if (mystring == "free") {
          bool6.X = true;
          bool6.Y = true;
          bool6.Z = true;
          bool6.Xx = true;
          bool6.Yy = true;
          bool6.Zz = true;
        } else if (IsPinned(mystring)) {
          bool6.X = false;
          bool6.Y = false;
          bool6.Z = false;
          bool6.Xx = true;
          bool6.Yy = true;
          bool6.Zz = true;
        } else if (IsFixed(mystring)) {
          bool6.X = false;
          bool6.Y = false;
          bool6.Z = false;
          bool6.Xx = false;
          bool6.Yy = false;
          bool6.Zz = false;
        } else if (IsRelease(mystring) || IsHinged(mystring) || mystring == "charnier") {
          bool6.X = true;
          bool6.Y = true;
          bool6.Z = true;
          bool6.Xx = true;
          bool6.Yy = false;
          bool6.Zz = false;
        } else if (mystring.Length == 6) {
          return ConvertString(mystring, true);
        } else {
          throw new InvalidCastException($"Data conversion failed from {data.GetTypeName()} to Bool6");
        }

        return bool6;
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
        _ => throw new InvalidCastException($"Unable to convert string to Bool6, character {character} not recognised"),
      };
    }
  }

}
