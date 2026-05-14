using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;

using GsaGH.Properties;
using GsaGH.UI;
using GsaGH.UI.Helpers;

using static GsaGH.UI.MessageDialogBox;

namespace GsaGH.Graphics.Menu {
  public class MenuLoad {
    private static ToolStripMenuItem oasysMenu;
    public const string Name = "Oasys";
    private static IExampleFileManager exampleFileManager;
    private static readonly object menuLock = new object();

    internal static void OnStartup(GH_Canvas canvas) {
      exampleFileManager = CreateExampleFileManager();
      oasysMenu = new ToolStripMenuItem(Name) {
        Name = Name,
      };
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      //PopulateSub(oasysMenu); // we cannot change the signature of this method to async, so we fire and forget the population of the menu, which will update when the files are retrieved
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      AddToRibbon();
    }

    private static void AddToRibbon() {
      GH_DocumentEditor editor = null;

      if (!EnsureDocumentEditorAvailable(ref editor)) {
        return;
      }

      AddOrUpdateOasysMenu(editor);

      Instances.CanvasCreated -= OnStartup;
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
        "Unable to initialize the Oasys menu because the Grasshopper document editor did not become available in time.",
        "GSA Examples", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return false;
    }

    private static void AddOrUpdateOasysMenu(GH_DocumentEditor editor) {
      if (!editor.MainMenuStrip.Items.ContainsKey(Name)) {
        editor.MainMenuStrip.Items.Add(oasysMenu);
      } else {
        oasysMenu = (ToolStripMenuItem)editor.MainMenuStrip.Items[Name];
        lock (menuLock) {
          oasysMenu.DropDown.Items.Add(new ToolStripSeparator());
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
          PopulateSub(
            oasysMenu); // we cannot change the signature of this method to async, so we fire and forget the population of the menu, which will update when the files are retrieved
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
      }
    }

    internal static async Task PopulateSub(ToolStripMenuItem menuItem) {
      try {
        await CreateMenu(menuItem);
      } catch {
        // looks stupid but we need to show message on main UI thread, that's why we have to catch the exception here and then invoke the message box on the main thread
        ToolStrip parent = menuItem.GetCurrentParent();
        if (parent != null) {
          parent.BeginInvoke((Action)(() => ShowMessage(FileState.NoFilesFound, string.Empty)));
        } else {
          ShowMessage(FileState.NoFilesFound, string.Empty);
        }
      }
    }

    private static async Task CreateMenu(ToolStripMenuItem menuItem) {
      ToolStripMenuItem menu1 = CreateDocumentationMenuItem();
      ToolStripMenuItem menu2 = await CreateExamplesMenuItem();
      ToolStripMenuItem menu3 = CreateInfoMenuItem();
      // must add in reverse order to get the correct order in the menu
      menuItem.DropDown.Items.Insert(0, menu3);
      menuItem.DropDown.Items.Insert(0, menu2);
      menuItem.DropDown.Items.Insert(0, menu1);
    }

    private static ToolStripMenuItem CreateDocumentationMenuItem() {
      return new ToolStripMenuItem("GSA Documentation", Resources.Documentation,
        (s, a) => RunProcess(Resources.DocumentationUrl));
    }

    private static async Task<ToolStripMenuItem> CreateExamplesMenuItem() {
      var menuItem = new ToolStripMenuItem("GSA Example files", Resources.ExampleFiles);
      menuItem.DropDownItems.Add(CreateViewOnGithubMenuItem());
      menuItem.DropDownItems.Add(new ToolStripSeparator());
      await AddExampleFilesToMenuAsync(menuItem);
      return menuItem;
    }

    private static ToolStripMenuItem CreateInfoMenuItem() {
      return new ToolStripMenuItem("GSA Info", Resources.GSAInfo, (s, a) => {
        var aboutBox = new AboutBox();
        aboutBox.ShowDialog();
      });
    }

    private static ToolStripMenuItem CreateViewOnGithubMenuItem() {
      return new ToolStripMenuItem("View on GitHub", Resources.ExampleFiles,
        (s, a) => RunProcess(Resources.GithubExamples));
    }

    private static void RunProcess(string url) {
      Process.Start(new ProcessStartInfo {
        FileName = url,
        UseShellExecute = true,
      });
    }

    private static void AddFileMenuItem(ToolStripMenuItem menuItem, FileEntry file) {
      menuItem.DropDown.Items.Add(file.Name, null, async (s, a) => {
        try {
          await exampleFileManager.DownloadAndOpenFileAsync(file, OpenFile);
        } catch (Exception) {
          ShowMessage(FileState.NoFilesFound, string.Empty);
        }
      });
    }

    private static bool OpenFile(string path) {
      var io = new GH_DocumentIO();
      if (!io.Open(path)) {
        return false;
      }

      Instances.DocumentEditor.Invoke((Action)(() => Instances.ActiveCanvas.Document = io.Document));
      return true;
    }

    private static async Task AddExampleFilesToMenuAsync(ToolStripMenuItem menuItem) {
      List<FileEntry> files = await exampleFileManager.GetExampleFilesAsync();

      if (files == null || files.Count <= 0) {
        return;
      }

      // Try to get a non-null UI invoker for marshaling back to the UI thread.
      ToolStrip parent = menuItem.GetCurrentParent() ?? menuItem.Owner;
      if (parent != null) {
        parent.BeginInvoke((Action)(() => files.ForEach(item => AddFileMenuItem(menuItem, item))));
      } else {
        // Fallback: use the document editor if available.
        GH_DocumentEditor editor = Instances.DocumentEditor;
        if (editor != null) {
          editor.BeginInvoke((Action)(() => files.ForEach(item => AddFileMenuItem(menuItem, item))));
        } else {
          // Fallback for unit tests: add directly
          files.ForEach(item => AddFileMenuItem(menuItem, item));
        }
      }
    }

    public static IExampleFileManager CreateExampleFileManager(IExampleFileManager manager = null) {
      if (manager != null) {
        exampleFileManager = manager;
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
  }
}
