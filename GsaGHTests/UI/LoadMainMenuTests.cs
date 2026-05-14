using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

using GsaGH.Graphics.Menu;
using GsaGH.UI;

using Xunit;

namespace GsaGHTests.UI {
  [Collection("GrasshopperFixture collection")]
  public class LoadMainMenuTests {
    [Fact]
    public async Task PopulateOasysMenu_ItemsInCorrectOrder() {
      ExamplesMenu.CreateExampleFileManager(new MockExampleFileManager(new List<FileEntry>()));
      var oasysMenu = new ToolStripMenuItem("Oasys");

      await MenuLoad.PopulateOasysMenu(oasysMenu);

      Assert.Equal(3, oasysMenu.DropDown.Items.Count);
      Assert.Equal("GSA Documentation", oasysMenu.DropDown.Items[0].Text);
      Assert.Equal("GSA Example files", oasysMenu.DropDown.Items[1].Text);
      Assert.Equal("GSA Info", oasysMenu.DropDown.Items[2].Text);
    }

    [Fact]
    public async Task PopulateOasysMenu_ShowsError_WhenExamplesMenuThrows() {
      var recordingWrapper = new RecordingMessageBoxWrapper();
      MessageDialogBox.SetMessageBoxWrapper(recordingWrapper);
      ExamplesMenu.CreateExampleFileManager(new ExceptionThrowingFileManager());
      var oasysMenu = new ToolStripMenuItem("Oasys");

      await MenuLoad.PopulateOasysMenu(oasysMenu);

      Assert.True(recordingWrapper.ShowCalled, "ShowMessage should be called when PopulateOasysMenu fails");
    }

    [Fact]
    public void CreateDocumentationMenuItem_HasCorrectText() {
      ToolStripMenuItem item = MenuLoad.CreateDocumentationMenuItem();

      Assert.Equal("GSA Documentation", item.Text);
    }

    [Fact]
    public void CreateInfoMenuItem_HasCorrectText() {
      ToolStripMenuItem item = MenuLoad.CreateInfoMenuItem();

      Assert.Equal("GSA Info", item.Text);
    }
  }
}
