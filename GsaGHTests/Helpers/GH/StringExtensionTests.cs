using System;
using System.Collections.Generic;
using System.Linq;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Helpers.Export.GH {
  [Collection("GrasshopperFixture collection")]
  public class StringExtensionTests {
    [Theory]
    [InlineData("this is a test", "ThisIsATest")]
    [InlineData("this_is_a test", "ThisIsATest")]
    [InlineData("Use_GPS_Settings", "UseGpsSettings")]
    public void ToPascalCaseTest(string val, string expected) {
      string result = val.ToPascalCase();
      Assert.Equal(expected, result);
    }
  }
}
