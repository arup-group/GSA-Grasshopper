using GsaGH.Helpers;

using Xunit;

namespace GsaGHTests.Helpers {
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

    [Theory]
    [InlineData("CreateSentenceCase", "Create sentence case")]
    [InlineData("this_is_a test", "this is a test")]
    [InlineData("Use_GPS_Settings", "Use GPS Settings")]
    public void ToSentenceCaseTest(string val, string expected) {
      string result = val.ToSentenceCase();
      Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("A                                      B", "A B")]
    [InlineData("A     B", "A B")]
    [InlineData(" A   B", "A B")]
    [InlineData("A  B ", "A B")]
    [InlineData(" A  B ", "A B")]
    public void TrimSpacesTest(string val, string expected) {
      string result = val.TrimSpaces();
      Assert.Equal(expected, result);
    }
  }
}
