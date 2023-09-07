using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GsaGH.Helpers.GH {
  public static class ListExtension {
    public static bool IsNullOrEmpty<T>(this ICollection<T> value) {
      if (value == null || value.Count == 0) {
        return true;
      }

      return false;
    }

    public static bool IsNullOrEmpty<T>(this ConcurrentBag<T> value) {
      if (value == null || value.Count == 0) {
        return true;
      }

      return false;
    }
  }
}
