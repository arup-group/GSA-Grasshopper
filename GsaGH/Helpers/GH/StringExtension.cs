using System.Collections.Generic;
using System.Linq;

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

    public static string StringifyRange(this IEnumerable<(int Min, int Max)> ranges) {
      var values = new List<string>();
      foreach((int Min, int Max) range in ranges) {
        if (IsEmpty(range)) {
          values.Add(range.Min.ToString());
        } else if (IsSingle(range)) {
          values.Add($"{range.Min} {range.Max}");
        } else {
          values.Add($"{range.Min} to {range.Max}");
        }
      }
      return string.Join(" ", values);

      static bool IsEmpty((int Min, int Max) range) {
        return range.Max - range.Min == 0;
      }

      static bool IsSingle((int Min, int Max) range) {
        return range.Max - range.Min == 1;
      }
    }

    public static IEnumerable<(int Min, int Max)> ToRanges(this IEnumerable<int> source) {
      source = source.OrderBy(x => x);
      using IEnumerator<int> e = source.GetEnumerator();
      
      if (!e.MoveNext()) {
        yield break;
      }

      int previous = e.Current;

      (int Min, int Max) range = (Min: previous, Max: e.Current);

      while (e.MoveNext()) {
        bool isConsecutive = e.Current - previous == 1;
        if (isConsecutive) {
          range = (range.Min, e.Current);
        } else {
          yield return range;
          range = (e.Current, e.Current);
        }
        previous = e.Current;
      }

      yield return range;
    }
  }
}
