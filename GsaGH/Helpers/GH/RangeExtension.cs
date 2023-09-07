using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGH.Helpers.GH {
  public static class RangeExtension {
    internal static string StringifyRange(this IEnumerable<Range> ranges) {
      var values = new List<string>();
      foreach (Range range in ranges) {
        if (IsEmpty(range)) {
          values.Add(range.Min.ToString());
        } else if (IsSingle(range)) {
          values.Add($"{range.Min} {range.Max}");
        } else {
          values.Add($"{range.Min} to {range.Max}");
        }
      }
      return string.Join(" ", values);

      static bool IsEmpty(Range range) {
        return range.Length == 0;
      }

      static bool IsSingle(Range range) {
        return range.Length == 1;
      }
    }

    internal static IEnumerable<Range> ToRanges(this IEnumerable<int> source) {
      source = source.OrderBy(x => x);
      using IEnumerator<int> e = source.GetEnumerator();

      if (!e.MoveNext()) {
        yield break;
      }

      int previous = e.Current;

      var range = new Range(previous, e.Current);

      while (e.MoveNext()) {
        bool isConsecutive = e.Current - previous == 1;
        if (isConsecutive) {
          range = new Range(range.Min, e.Current);
        } else {
          yield return range;
          range = new Range(e.Current, e.Current);
        }
        previous = e.Current;
      }

      yield return range;
    }
  }
}
