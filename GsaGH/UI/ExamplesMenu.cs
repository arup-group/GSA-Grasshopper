using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;

using GsaGH.Properties;
using GsaGH.UI;
using GsaGH.UI.Helpers;

namespace GsaGH.Graphics.Menu {

  /// <summary>
  ///   Provides a menu for downloading and opening example files.
  /// </summary>
  public class ExamplesMenu {
    private static ToolStripMenuItem examplesMenu;
    public const string Name = "Examples";
    private static IExampleFileManager exampleFileManager;
    private static readonly object examplesMenuLock = new object();

    protected ExamplesMenu() { }

    /// <summary>
    ///   Initializes the examples menu on Grasshopper startup.
    /// </summary>
    internal static void OnStartup(GH_Canvas canvas) {
      exampleFileManager = CreateExampleFileManager();
      examplesMenu = new ToolStripMenuItem(Name) {
        Name = Name,
      };

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      PopulateSub(examplesMenu); // we cannot change the signature of this method to async, so we fire and forget the population of the menu, which will update when the files are retrieved
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      AddToMainTab();
    }

    public static IExampleFileManager CreateExampleFileManager(IExampleFileManager manager = null) {
      if (manager != null) {
        return manager;
      }

      if (exampleFileManager != null) {
        return exampleFileManager;
      }

      var httpClient = new HttpClient();
      IHttpClientWrapper httpClientWrapper = new HttpClientWrapper(httpClient);
      IHttpsFileDownloader downloader = new HttpsFileDownloader(httpClientWrapper, Resources.SamplesUrl);
      return new ExampleFileManager(downloader);
    }

    private static void AddToMainTab() {
      GH_DocumentEditor editor = null;

      while (editor == null) {
        editor = Instances.DocumentEditor;
        Thread.Sleep(321);
      }

      if (!editor.MainMenuStrip.Items.ContainsKey(Name)) {
        editor.MainMenuStrip.Items.Add(examplesMenu);
      } else {
        examplesMenu = (ToolStripMenuItem)editor.MainMenuStrip.Items[Name];
        lock (examplesMenuLock) {
          examplesMenu.DropDown.Items.Add(new ToolStripSeparator());
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
          PopulateSub(
            examplesMenu); // we cannot change the signature of this method to async, so we fire and forget the population of the menu, which will update when the files are retrieved
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
      }

      Instances.CanvasCreated -= OnStartup;
    }

    /// <summary>
    ///   Populates the submenu with example files.
    /// </summary>
    internal static async Task PopulateSub(ToolStripMenuItem menuItem) {
      try {
        await AddExampleFilesAsync(menuItem);
      } catch {
        // looks stupid but we need to show message on main UI thread, that's why we have to catch the exception here and then invoke the message box on the main thread
        ToolStrip parent = menuItem.GetCurrentParent();
        if (parent != null) {
          parent.BeginInvoke((Action)(()
            => MessageDialogBox.ShowMessage(MessageDialogBox.FileOpenState.NoFilesFound, string.Empty)));
        } else {
          MessageDialogBox.ShowMessage(MessageDialogBox.FileOpenState.NoFilesFound, string.Empty);
        }
      }
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
