using System.Windows.Forms;

using GsaGH.Helpers.GH;

using OasysGH.Units.Helpers;

using Xunit;

namespace GsaGHTests.Helpers {
  [Collection("GrasshopperFixture collection")]
  public class GenerateToolStripMenuItemTest {
    [Fact]
    public void WhenGenerateToolStripMenuItemWithEmptyStrings() {
      ToolStripMenuItem obj = GenerateToolStripMenuItem.GetSubMenuItem(string.Empty,
        EngineeringUnits.Acceleration, string.Empty, s => { });

      Assert.Null(obj);
    }

    [Fact]
    public void WhenGenerateToolStripMenuItem() {
      ToolStripMenuItem obj = GenerateToolStripMenuItem.GetSubMenuItem("Force",
        EngineeringUnits.Force, "kN", s => { });

      Assert.Equal("Force", obj.Text);
      Assert.True(obj.Enabled);
      Assert.False(obj.Checked);
      Assert.True(obj.DropDownItems[1].Text == "kN");
      Assert.True(6 == obj.DropDownItems.Count);

      obj.Checked = true;
      obj.Enabled = false;
      Assert.True(obj.Checked);
      Assert.False(obj.Enabled);
    }
  }
}
