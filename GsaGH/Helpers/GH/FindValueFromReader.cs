using System;
using System.Collections.Generic;

using GH_IO.Serialization;

namespace GsaGH.Helpers.GH {
  public static class FindValueFromReader {

    public static bool TryGetEnum(
      this GH_IReader reader, string valueToFind, Type enumToFind, out int foundedValue) {
      foundedValue = 0;

      IEnumerable<string> elementsToCheck = GetNamesToCheck(valueToFind);
      string result = TryGetStringFrom(reader, elementsToCheck);
      if (string.IsNullOrEmpty(result)) {
        return false;
      }

      foundedValue = (int)Enum.Parse(enumToFind, result.ToPascalCase());

      return true;
    }

    private static IEnumerable<string> GetNamesToCheck(string defaultValue) {
      return new List<string> {
        defaultValue,
        $"_{defaultValue}",
      };
    }

    /// <summary>
    ///   Try get string for a specified names
    /// </summary>
    /// <returns>founded value or string.Empty</returns>
    private static string TryGetStringFrom(GH_IReader reader, IEnumerable<string> elementsToCheck) {
      string result = string.Empty;
      foreach (string value in elementsToCheck) {
        if (reader.TryGetString(value, ref result)) {
          return result;
        }
      }

      return result;
    }
  }
}
