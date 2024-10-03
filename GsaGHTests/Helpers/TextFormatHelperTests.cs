using System;
using System.Collections.Generic;
using System.Linq;

using GsaGH.Helpers;

using Xunit;

namespace GsaGHTests.Helpers {
  public class TextFormatHelperTests {

    [Fact]
    public void ShouldIncludeNewLineBetweenItems() {
      var elementTypeMapping = new Dictionary<int, string> {
        { 1, "One" },
        { 2, "Two" },
        { 3, "Three" }
      };
      string result = TextFormatHelper.FormatNumberedList(elementTypeMapping);
      string separator = Environment.NewLine;
      int itemCount = result.Split(new[] { separator }, StringSplitOptions.None).Length;
      Assert.Equal(3, itemCount);
    }
  }
}
