using System.Drawing;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;

using Moq;

using Xunit;

namespace GsaGHTests.Helpers {
  [Collection("GrasshopperFixture collection")]
  public class ContourLegendTests {
    private readonly Mock<IContourLegendConfiguration> _mockConfiguration;
    private readonly ContourLegend legend;
    private readonly Bitmap _mockBitmap;

    public ContourLegendTests() {
      _mockBitmap = new Bitmap(10, 10);
      _mockConfiguration = new Mock<IContourLegendConfiguration>();
      _mockConfiguration.Setup(c => c.Bitmap).Returns(_mockBitmap);
      _mockConfiguration.Setup(c => c.Scale).Returns(1.0);
      legend = new ContourLegend(_mockConfiguration.Object);
    }

    [Fact]
    public void Constructor_InitializesConfiguration() {
      Assert.NotNull(legend);
    }

    [Fact]
    public void DrawGradientLegend_UpdatesBitmapCorrectly() {
      var color = Color.FromArgb(0, 255, 255, 0);
      legend.DrawGradientLegend(2, 5, color);

      for (int y = 2; y < 5; y++) {
        for (int x = 0; x < _mockBitmap.Width; x++) {
          Assert.Equal(color, _mockBitmap.GetPixel(x, _mockBitmap.Height - y - 1));
        }
      }
    }
    //other methods need to be tested manually or by integration tests. 
    //we are not able to mock or inherit DisplayPipeline(which is needed to draw stuff)
    //so we aren't able to check if drawings are correct
  }
}
