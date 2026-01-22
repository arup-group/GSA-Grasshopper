using System.Collections.Generic;

using GsaGH.Helpers.GsaApi;

using Xunit;

namespace GsaGHTests.Helpers.GsaApi {
  public class ValueHelpersTests {
    [Theory]
    [InlineData(0, 0, 0.000000000001, -0.000000000001)]
    [InlineData(74, 26, 75, 25)]
    [InlineData(7499, 26, 8000, 0)]
    [InlineData(24, 15, 25, 15)]
    [InlineData(0, -24, 0, -25)]
    public void SmartRounderTests(double max, double min, double expectedMax, double expectedMin) {
      List<double> vals = ResultHelper.SmartRounder(max, min);
      Assert.Equal(expectedMax, vals[0]);
      Assert.Equal(expectedMin, vals[1]);
    }
  }
}
