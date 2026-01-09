using System.Collections.Generic;
using System.Linq;

using GsaGH.Helpers;
using GsaGH.Parameters;

using Xunit;

namespace GsaGHTests.Helpers {
  [Collection("GrasshopperFixture collection")]
  public class RangeExtensionTests {
    [Theory]
    [InlineData(new int[] { 1, 3, 2, 4, 14, 13, 12, 11 }, new int[] { 1, 11 }, new int[] { 4, 14 })]
    public void ToRangesTest(IEnumerable<int> range, int[] expectedMins, int[] expectedMaxs) {
      var ranges = range.ToRanges().ToList();
      for (int i = 0; i < ranges.Count; i++) {
        Assert.Equal(expectedMins[i], ranges[i].Min);
        Assert.Equal(expectedMaxs[i], ranges[i].Max);
      }
    }

    [Theory]
    [InlineData(new int[] { 1, 3, 2, 4, 14, 13, 12, 11 }, "1 to 4 11 to 14")]
    [InlineData(new int[] { 1, 3, 2, 4, 7, 8, 14, 13, 12, 11 }, "1 to 4 7 8 11 to 14")]
    public void StringifyRangeTest(IEnumerable<int> range, string expected) {
      var ranges = range.ToRanges().ToList();
      string result = ranges.StringifyRange();
      Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(new int[] { 1, 3, 2, 4, 14, 13, 12, 11 }, "1 to 4 11 to 14")]
    [InlineData(new int[] { 1, 3, 2, 4, 7, 8, 14, 13, 12, 11 }, "1 to 4 7 8 11 to 14")]
    [InlineData(new int[] { 1, 3, 7 }, "1 3 7")]
    public void CreateListDefinitionTest(IEnumerable<int> range, string expected) {
      string result = GsaList.CreateListDefinition(range.ToList());
      Assert.Equal(expected, result);
    }
  }
}
