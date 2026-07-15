using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

using Grasshopper;
using Grasshopper.GUI;

using GsaGH.Properties;
using GsaGH.UI;
using GsaGH.UI.Helpers;

using static GsaGH.UI.MessageDialogBox;

namespace GsaGH.Graphics.Menu {

  /// <summary>
  ///   Builds a menu item populated with downloadable example files.
  /// </summary>
  public class ExamplesMenu {
    public static IExampleFileManager ExampleFileManager { get; private set; }

    protected ExamplesMenu() { }

    public static IExampleFileManager CreateExampleFileManager(IExampleFileManager manager = null) {
      if (manager != null) {
        ExampleFileManager = manager;
        return manager;
      }

      if (ExampleFileManager != null) {
        return ExampleFileManager;
      }

      var httpClient = new HttpClient();
      IHttpClientWrapper httpClientWrapper = new HttpClientWrapper(httpClient);
      IHttpsFileDownloader downloader = new HttpsFileDownloader(httpClientWrapper, Resources.SamplesUrl);
      return new ExampleFileManager(downloader);
    }

    /// <summary>
    ///   Creates a "GSA Example files" menu item with a GitHub link and all example files.
    /// </summary>
    internal static async Task<ToolStripMenuItem> CreateExamplesMenuItemAsync() {
      ExampleFileManager = CreateExampleFileManager();
      var menuItem = new ToolStripMenuItem("GSA Example files", Resources.ExampleFiles);
      menuItem.DropDownItems.Add(CreateViewOnGithubMenuItem());
      menuItem.DropDownItems.Add(new ToolStripSeparator());
      try {
        await AddExampleFilesAsync(menuItem);
      } catch {
        ShowMessage(FileState.NoFilesFound, string.Empty);
      }

      return menuItem;
    }

    private static ToolStripMenuItem CreateViewOnGithubMenuItem() {
      return new ToolStripMenuItem("View on GitHub", Resources.ExampleFiles,
        (s, a) => OpenUrl(Resources.GithubExamples));
    }

    private static async Task AddExampleFilesAsync(ToolStripMenuItem menuItem) {
      List<FileEntry> files = await ExampleFileManager.GetExampleFilesAsync();

      if (files == null || files.Count <= 0) {
        return;
      }

      ToolStrip parent = menuItem.GetCurrentParent() ?? menuItem.Owner;
      if (parent != null) {
        parent.BeginInvoke((Action)(() => files.ForEach(item => AddFileMenuItem(menuItem, item))));
      } else {
        GH_DocumentEditor editor = Instances.DocumentEditor;
        if (editor != null) {
          editor.BeginInvoke((Action)(() => files.ForEach(item => AddFileMenuItem(menuItem, item))));
        } else {
          files.ForEach(item => AddFileMenuItem(menuItem, item));
        }
      }
    }

    private static void AddFileMenuItem(ToolStripMenuItem menuItem, FileEntry file) {
      menuItem.DropDown.Items.Add(file.Name, null, async (s, a) => {
        try {
          await ExampleFileManager.DownloadAndOpenFileAsync(file, GrasshopperFileOpener.Open);
        } catch (Exception) {
          ShowMessage(FileState.NoFilesFound, string.Empty);
        }
      });
    }

    private static void OpenUrl(string url) {
      Process.Start(new ProcessStartInfo {
        FileName = url,
        UseShellExecute = true,
      });
    }
  }
}
