﻿using GsaGH.Helpers.GsaApi;
using System.Collections.Generic;
using Xunit;

namespace GsaGHTests.Helpers.GsaApi {
  public class ValueHelpersTests {
    [Theory]
    [InlineData(1.23456789, 4, "1.235")]
    [InlineData(123456, 4, "123500")]
    [InlineData(0.000123456, 4, "0.0001235")]
    [InlineData(-1.23456789, 4, "-1.235")]
    [InlineData(-123456, 4, "-123500")]
    [InlineData(-0.000123456, 4, "-0.0001235")]
    public void RoundToSignificantDigitsTest(double value, int digits, string expected) {
      double d = ResultHelper.RoundToSignificantDigits(value, digits);
      Assert.Equal(expected, d.ToString());
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
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
