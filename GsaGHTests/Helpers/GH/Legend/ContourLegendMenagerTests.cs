﻿using System;
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
    private static Mock<ContourLegend> _mockLegend;
    private static Mock<ContourLegendConfiguration> _mockConfiguration;
    private static Mock<LegendMenuBuilder> _mockMenuBuilder;
    private static ContourLegendManager _manager;
    private static Mock<IGH_PreviewArgs> _mockArgs;

    public ContourLegendMenagerTests() {
      _mockLegend = new Mock<ContourLegend>();
      _mockConfiguration = new Mock<ContourLegendConfiguration>();
      _mockMenuBuilder = new Mock<LegendMenuBuilder>();
      _manager = new ContourLegendManager(_mockConfiguration.Object, _mockLegend.Object, _mockMenuBuilder.Object);

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

      _manager.DrawLegend(_mockArgs.Object, title, bottomText, gradients);

      _mockLegend.Verify(l => l.DrawLegendRectangle(_mockArgs.Object, title, bottomText, gradients), Times.Once);
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
      Assert.Equal(scale, _mockConfiguration.Object.Scale);
    }
  }
}
