using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;

using Moq;

using Xunit;

namespace GsaGHTests.Helpers {
  [Collection("GrasshopperFixture collection")]
  public class ContourLegendTests {
    private readonly Mock<ContourLegendConfiguration> _mockConfiguration;
    private ContourLegend legend;
    private readonly Bitmap _mockBitmap;

    public ContourLegendTests() {
      _mockBitmap = new Bitmap(10, 10);
      _mockConfiguration = new Mock<ContourLegendConfiguration>();
      // _mockConfiguration.Setup(c => c.Bitmap).Returns(_mockBitmap);
      _mockConfiguration.Setup(c => c.Scale).Returns(1.0);
      legend = new ContourLegend(_mockConfiguration.Object);
    }

    [Fact]
    public void Constructor_InitializesConfiguration() {
      Assert.NotNull(legend);
    }

    [Fact]
    public void ShouldNotDrawILegendIsNotDisplayable() {
      legend = new ContourLegend(ContourLegendConfiguration.GetDefault());
      var previewArgsSpy = new Mock<IGH_PreviewArgs>();

      legend.DrawLegendRectangle(previewArgsSpy.Object, string.Empty, string.Empty,
        new List<(int startY, int endY, Color gradientColor)>());
      Assert.True(legend.IsInvalidConfiguration);
    }
  }
}
