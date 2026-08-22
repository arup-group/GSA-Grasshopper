using System.Collections.Generic;
using System.Windows.Forms;

using GsaGH.Graphics.Menu;
using GsaGH.Properties;
using GsaGH.UI.Helpers;

namespace GsaGH.UI.SampleFiles {
  public static class SampleFilesMenuHelper {
    public static ToolStripMenuItem CreateSampleFilesMenu(List<string> keywords) {
      var samplesMenu = new ToolStripMenuItem("Sample files", Resources.ModelTitles) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };

      IEnumerable<FileEntry> files = ExampleFileRepository.GetFileEntriesByKeywords(keywords);
      foreach (FileEntry file in files) {
        samplesMenu.DropDownItems.Add(new ToolStripMenuItem(file.Name, null, async (s, e) => {
          await ExamplesMenu.ExampleFileManager.DownloadAndOpenFileAsync(file, GrasshopperFileOpener.Open);
        }) {
          Enabled = true,
        });
      }

      return samplesMenu.DropDownItems.Count > 0 ? samplesMenu : null;
    }
  }
}
