using System;

using GsaGH.UI.Helpers;

using Xunit;

namespace GsaGHTests.UI {
  public class HttpNameExtractorTests {
    [Theory]
    [InlineData("file.txt")]
    [InlineData("my_file-123.csv")]
    [InlineData("ąęśćżź.txt")]
    public void GetSafeFileName_ValidFileName_ReturnsSameName(string fileName) {
      string result = HttpNameExtractor.GetSafeFileName(fileName);
      Assert.Equal(fileName, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetSafeFileName_NullOrWhitespace_ThrowsArgumentException(string fileName) {
      Assert.Throws<ArgumentException>(() => HttpNameExtractor.GetSafeFileName(fileName));
    }

    [Theory]
    [InlineData(@"C:\file.txt")]
    [InlineData(@"/usr/file.txt")]
    public void GetSafeFileName_RootedPath_ThrowsArgumentException(string fileName) {
      Assert.Throws<ArgumentException>(() => HttpNameExtractor.GetSafeFileName(fileName));
    }

    [Theory]
    [InlineData("file?.txt")]
    [InlineData("file<.txt")]
    [InlineData("file|.txt")]
    public void GetSafeFileName_InvalidChars_ThrowsArgumentException(string fileName) {
      Assert.Throws<ArgumentException>(() => HttpNameExtractor.GetSafeFileName(fileName));
    }
  }
}
