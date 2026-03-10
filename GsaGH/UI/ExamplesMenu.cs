using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;

using GsaGH.UI;

namespace GsaGH.Graphics.Menu {

  /// <summary>
  ///   Provides a menu for downloading and opening example files.
  /// </summary>
  public class ExamplesMenu {
    private static ToolStripMenuItem examplesMenu;
    private const string name = "X Examples";
    private static IExampleFileManager exampleFileManager;

    /// <summary>
    ///   Initializes the examples menu on Grasshopper startup.
    /// </summary>
    internal static void OnStartup(GH_Canvas canvas) {
      exampleFileManager = new ExampleFileManager(new HttpsFileDownloader());
      examplesMenu = new ToolStripMenuItem(name) {
        Name = name,
      };

      PopulateSub(examplesMenu);

      AddToMainTab();
    }

    private static void AddToMainTab() {
      GH_DocumentEditor editor = null;

      while (editor == null) {
        editor = Instances.DocumentEditor;
        Thread.Sleep(321);
      }

      if (!editor.MainMenuStrip.Items.ContainsKey(name)) {
        editor.MainMenuStrip.Items.Add(examplesMenu);
      } else {
        examplesMenu = (ToolStripMenuItem)editor.MainMenuStrip.Items[name];
        lock (examplesMenu) {
          examplesMenu.DropDown.Items.Add(new ToolStripSeparator());
          PopulateSub(examplesMenu);
        }
      }

      Instances.CanvasCreated -= OnStartup;
    }

    /// <summary>
    ///   Populates the submenu with example files.
    /// </summary>
    internal static void PopulateSub(ToolStripMenuItem menuItem) {
      AddExampleFilesAsync(menuItem);
    }

    private static async Task AddExampleFilesAsync(ToolStripMenuItem menuItem) {
      List<FileEntry> files = await exampleFileManager.GetExampleFilesAsync();

      if (files != null && files.Count > 0) {
        menuItem.GetCurrentParent().BeginInvoke((Action)(() => {
          foreach (FileEntry file in files) {
            AddFileMenuItem(menuItem, file);
          }
        }));
      }
    }

    private static void AddFileMenuItem(ToolStripMenuItem menuItem, FileEntry file) {
      menuItem.DropDown.Items.Add(file.Name, null,
        async (s, a) => await exampleFileManager.DownloadAndOpenFileAsync(file));
    }
  }
}
