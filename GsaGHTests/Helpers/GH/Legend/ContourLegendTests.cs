using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;

using Moq;

using Xunit;

namespace GsaGHTests.Helpers {
  [Collection("GrasshopperFixture collection")]
  public class ContourLegendTests {
    private readonly Mock<IContourLegendConfiguration> _mockConfiguration;
    private ContourLegend legend;
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

    [Fact]
    public void DrawGradientWillThrowErrorWhenStartValuesIsBiggerThanEndValue() {
      Color color = Color.Red;
      Assert.Throws<ArgumentOutOfRangeException>(() => legend.DrawGradientLegend(5, 1, color));
    }

    [Fact]
    public void DrawGradientWillThrowErrorWhenEndValueIsBiggerThanBitmapHeight() {
      Color color = Color.Red;
      Assert.Throws<ArgumentOutOfRangeException>(() => legend.DrawGradientLegend(5, 200, color));
    }

    [Fact]
    public void DrawGradientWillThrowErrorWhenStartValueLessThan0() {
      Color color = Color.Green;
      Assert.Throws<ArgumentOutOfRangeException>(() => legend.DrawGradientLegend(-1, 1, color));
    }

    [Fact]
    public void ShouldNotDrawILegendIsNotDisplayable() {
      legend = new ContourLegend(new ContourLegendConfiguration());
      var previewArgsSpy = new Mock<IGH_PreviewArgs>();

      legend.DrawLegendRectangle(previewArgsSpy.Object, string.Empty, string.Empty,
        new List<(int startY, int endY, Color gradientColor)>());
      Assert.True(legend.IsInvalidConfiguration);
    }
  }
}
