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

      if (!EnsureDocumentEditorAvailable(ref editor)) {
        return;
      }

      AddOrUpdateExamplesMenu(editor);

      Instances.CanvasCreated -= OnStartup;
    }

    private static void AddOrUpdateExamplesMenu(GH_DocumentEditor editor) {
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
    }

    private static bool EnsureDocumentEditorAvailable(ref GH_DocumentEditor editor) {
      DateTime start = DateTime.UtcNow;
      var timeout = TimeSpan.FromSeconds(60);
      while (editor == null && DateTime.UtcNow - start < timeout) {
        editor = Instances.DocumentEditor;
        if (editor != null) {
          break;
        }

        Thread.Sleep(321);
      }

      if (editor != null) {
        return true;
      }

      // Failed to obtain the document editor within the timeout; skip adding the menu and fail gracefully.
      Instances.CanvasCreated -= OnStartup;
      MessageBox.Show(
        "Unable to initialize the Examples menu because the Grasshopper document editor did not become available in time.",
        "GSA Examples", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return false;
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
            => MessageDialogBox.ShowMessage(MessageDialogBox.FileState.NoFilesFound, string.Empty)));
        } else {
          MessageDialogBox.ShowMessage(MessageDialogBox.FileState.NoFilesFound, string.Empty);
        }
      }
    }

    private static async Task AddExampleFilesAsync(ToolStripMenuItem menuItem) {
      List<FileEntry> files = await exampleFileManager.GetExampleFilesAsync();

      if (files != null && files.Count > 0) {
        // Try to get a non-null UI invoker for marshaling back to the UI thread.
        ToolStrip parent = menuItem.GetCurrentParent() ?? menuItem.Owner;
        if (parent != null) {
          parent.BeginInvoke((Action)(() => {
            foreach (FileEntry file in files) {
              AddFileMenuItem(menuItem, file);
            }
          }));
        } else {
          // Fallback: use the document editor if available.
          GH_DocumentEditor editor = Instances.DocumentEditor;
          editor?.BeginInvoke((Action)(() => {
            foreach (FileEntry file in files) {
              AddFileMenuItem(menuItem, file);
            }
          }));
        }
      }
    }

    private static void AddFileMenuItem(ToolStripMenuItem menuItem, FileEntry file) {
      menuItem.DropDown.Items.Add(file.Name, null, async (s, a) => {
        try {
          await exampleFileManager.DownloadAndOpenFileAsync(file);
        } catch (Exception) {
          MessageDialogBox.ShowMessage(MessageDialogBox.FileState.NoFilesFound, string.Empty);
        }
      });
    }
  }
}
