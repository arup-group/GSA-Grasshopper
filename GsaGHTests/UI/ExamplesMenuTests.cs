using System.Windows.Forms;

using GsaGH.Graphics.Menu;

using Xunit;

namespace GsaGHTests.UI {
  [Collection("GrasshopperFixture collection")]
  public class ExamplesMenuTests {
    private ToolStripMenuItem menu;

    public void CreateMenu(string name = "") {
      menu = new ToolStripMenuItem(name ?? ExamplesMenu.Name);
      //MenuLoad.OnStartup(null);
      MenuLoad.PopulateSub(menu);
    }

    [Fact]
    public void ExamplesMenuShouldHaveItems() {
      Assert.True(menu.DropDown.Items.Count > 0);
    }
  }
}
