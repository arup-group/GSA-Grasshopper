using System.Windows.Forms;

using GsaGH.Graphics.Menu;

using Xunit;

namespace GsaGHTests.UI {
  [Collection("GrasshopperFixture collection")]
  public class LoadMainMenuTests {
    [Fact]
    public void LoadMainMenuTest() {
      var oasysMenu = new ToolStripMenuItem("Oasys");
      MenuLoad.PopulateSub(oasysMenu);
      Assert.Equal(3, oasysMenu.DropDown.Items.Count);
    }
  }
}
