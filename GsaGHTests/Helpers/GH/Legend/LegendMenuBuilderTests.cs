using System;
using System.Windows.Forms;

using GsaGH.Components;
using GsaGH.Helpers;

using Moq;

using Xunit;

namespace GsaGHTests.Helpers {
  [Collection("GrasshopperFixture collection")]
  public class LegendMenuBuilderTests {
    private Mock<Contour1dResults> _componentMock;
    private double _initialScale;
    private ToolStripMenuItem _menuItem;

    public LegendMenuBuilderTests() {
      _componentMock = new Mock<Contour1dResults>(); // any component you want
      var updateUiMock = new Mock<Action>();
      var setLegendScaleMock = new Mock<LegendMenuBuilder.SetLegendScaleDelegate>();
      _initialScale = 1.5;
      var builder = new LegendMenuBuilder();

      _menuItem = builder.CreateLegendToolStripMenuItem(_componentMock.Object, updateUiMock.Object,
        setLegendScaleMock.Object, _initialScale);
    }

    [Fact]
    public void CreateLegendToolStripMenuItem_CreatesMenuItemCorrectly() {
      Assert.NotNull(_menuItem);
      Assert.Equal("Scale Legend", _menuItem.Text);
      Assert.True(_menuItem.Enabled);
      Assert.NotNull(_menuItem.DropDownItems);
      Assert.Equal(_initialScale.ToString(), _menuItem.DropDownItems[0]?.Text);
    }

    [Fact]
    public void CreateLegendToolStripMenuItem_UpdatesScale_OnTextChange() {
      ToolStripItem scaleTextBox = _menuItem.DropDownItems[0];
      double newScale = 2.5d;
      // Act - Simulate text change
      scaleTextBox.Text = $"{newScale}"; // Updates private _scaleLegendTxt indirectly
      _menuItem.DropDownItems[1].PerformClick(); // Simulate mouse up to trigger UpdateLegendScale

      Assert.Equal(newScale.ToString(), _menuItem.DropDownItems[0]?.Text);
    }
  }
}
