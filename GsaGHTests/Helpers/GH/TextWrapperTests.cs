using System.Drawing;

using GsaGH.Helpers.GH;

using Xunit;

namespace GsaGHTests.Helpers {
  [Collection("GrasshopperFixture collection")]
  public class TextWrapperTests {
    [Fact]
    public void WrapTextShouldReturnEmptyWhenInputTextIsIsNullOrWhiteSpace() {
      var font = new Font("Arial", 10);
      Assert.Empty(TextWrapper.WrapText(string.Empty, 1, font));
      Assert.Empty(TextWrapper.WrapText(null, 1, font));
      Assert.Empty(TextWrapper.WrapText("\n", 1, font));
      Assert.Empty(TextWrapper.WrapText(" ", 1, font));
    }

    [Fact]
    public void WrapText_ShouldWrapTextCorrectly() {
      string inputText = "t e";
      int maxWidth = 1;
      int fontSize = 10;

      string result = TextWrapper.WrapText(inputText, maxWidth, new Font("Arial", fontSize));

      Assert.NotNull(result);
      Assert.Contains("\n", result);
      Assert.Equal("t\ne", result);
    }

    [Fact]
    public void WrapText_TextShouldNotBeWrapped() {
      string inputText = "T";
      int maxWidth = 100;
      int fontSize = 12;

      string result = TextWrapper.WrapText(inputText, maxWidth, new Font("Arial", fontSize));

      Assert.NotNull(result);
      Assert.Equal("T", result);
    }
  }
}
