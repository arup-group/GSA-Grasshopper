using GsaGH.Helpers;
using GsaGH.Helpers.GH;

using Moq;

using Xunit;

namespace GsaGHTests.Helpers {
  public class ContourLegendTests {
    [Fact]
    public void Constructor_InitializesConfiguration() {
      // Arrange
      var mockConfiguration = new Mock<IContourLegendConfiguration>();

      // Act
      var legend = new ContourLegend(mockConfiguration.Object);

      // Assert
      Assert.NotNull(legend);
    }
  }
}
