using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;

using Moq;

using Xunit;

namespace GsaGHTests.Helpers {
  [Collection("GrasshopperFixture collection")]
  public class ContourLegendTests {
    private ContourLegend legend;

    public ContourLegendTests() {
      var configuration = new ContourLegendConfiguration(1, 2, 1.0d);
      legend = new ContourLegend(configuration);
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

    [Fact]
    public void WhenBitmapWidthEqualZeroThenThrowError() {
      Assert.Throws<ArgumentOutOfRangeException>(() => new ContourLegend(ContourLegendConfiguration.GetDefault(), 0));
    }
  }
}
