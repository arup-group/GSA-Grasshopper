using System;
using System.Collections.Generic;
using System.Linq;

namespace GsaGH.Helpers {
  public static class TextFormatHelper {

    public static string FormatNumberedList(Dictionary<int, string> elementTypeMapping) {
      return string.Join(Environment.NewLine,
        elementTypeMapping.Select(x => $"{x.Key}: {x.Value}"));
    }
  }
}
