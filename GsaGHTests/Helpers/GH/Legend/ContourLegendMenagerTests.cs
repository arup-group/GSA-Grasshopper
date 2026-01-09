using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Grasshopper.Kernel;

using GsaGH.Components;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;

using Moq;

using Xunit;

namespace GsaGHTests.Helpers {
  [Collection("GrasshopperFixture collection")]
  public class ContourLegendMenagerTests {
    private static ContourLegend _legend;
    private static ContourLegendConfiguration _configuration;
    private static ContourLegendManager _manager;
    private static Mock<IGH_PreviewArgs> _mockArgs;

    public ContourLegendMenagerTests() {
      _configuration = new ContourLegendConfiguration(1, 1, 1.0d);
      _legend = new ContourLegend(_configuration);
      var menuBuilder = new LegendMenuBuilder();
      _manager = new ContourLegendManager(_configuration, _legend, menuBuilder);

      _mockArgs = new Mock<IGH_PreviewArgs>();
    }

    [Fact]
    public void DrawLegend_CallsDrawLegendRectangle() {
      string title = "Test Title";
      string bottomText = "Test Bottom Text";
      var gradients = new List<(int, int, Color)> {
        (0, 10, Color.Red),
        (10, 20, Color.Green),
      };

      _manager.Legend.DrawLegendRectangle(_mockArgs.Object, title, bottomText, gradients);

      Assert.True(_manager.Legend.IsInvalidConfiguration);
    }

    [Fact]
    public void CreateMenu_CallsCreateLegendToolStripMenuItem() {
      var mockUpdateUI = new Mock<Action>();

      ToolStripMenuItem menuItem
        = _manager.CreateMenu(new Contour1dResults(), mockUpdateUI.Object); // don't care of the component type

      Assert.NotNull(menuItem);
    }

    [Fact]
    public void UpdateScale_CallsSetLegendScale() {
      double scale = 2.5;
      _manager.UpdateScale(scale);
      Assert.Equal(scale, _configuration.Scale);
    }
  }
}
