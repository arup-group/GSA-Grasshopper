using System;
using System.Collections.Generic;
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

  /// <summary>
  ///   Provides a menu for downloading and opening example files.
  /// </summary>
  public class ExamplesMenu {
    private static ToolStripMenuItem examplesMenu;
    public const string Name = "Examples";
    private static IExampleFileManager exampleFileManager;
    private static readonly object examplesMenuLock = new object();

    // Private backing fields for dependency injection — defaults reflect production behaviour.
    // Use the internal Set* methods (or ResetDependencies) to override them in tests.
    private static Func<GH_DocumentEditor> _getDocumentEditor = () => Instances.DocumentEditor;
    private static Action<int> _sleep = ms => Thread.Sleep(ms);
    private static TimeSpan _editorAvailabilityTimeout = TimeSpan.FromSeconds(60);
    private static Func<string, bool> _ghDocumentLoader = DefaultDocumentLoader;

    protected ExamplesMenu() { }

    // ── Internal setters (test injection, following MessageDialogBox.SetMessageBoxWrapper pattern) ──

    /// <summary>Overrides the provider used to obtain the active <see cref="GH_DocumentEditor" />.</summary>
    internal static void SetDocumentEditorProvider(Func<GH_DocumentEditor> provider) {
      _getDocumentEditor = provider ?? (() => Instances.DocumentEditor);
    }

    /// <summary>Overrides the sleep action used while polling for the document editor.</summary>
    internal static void SetSleepAction(Action<int> sleep) {
      _sleep = sleep ?? (ms => Thread.Sleep(ms));
    }

    /// <summary>Overrides the maximum time to wait for the document editor to become available.</summary>
    internal static void SetEditorAvailabilityTimeout(TimeSpan timeout) {
      _editorAvailabilityTimeout = timeout;
    }

    /// <summary>
    ///   Overrides the function used to open a Grasshopper document from disk.
    ///   Override in tests to avoid touching the file system or Grasshopper runtime.
    /// </summary>
    internal static void SetDocumentLoader(Func<string, bool> loader) {
      _ghDocumentLoader = loader ?? DefaultDocumentLoader;
    }

    /// <summary>Restores all overridable dependencies to their production defaults.</summary>
    internal static void ResetDependencies() {
      _getDocumentEditor = () => Instances.DocumentEditor;
      _sleep = ms => Thread.Sleep(ms);
      _editorAvailabilityTimeout = TimeSpan.FromSeconds(60);
      _ghDocumentLoader = DefaultDocumentLoader;
    }

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

    internal static void AddToMainTab() {
      GH_DocumentEditor editor = null;

      if (!EnsureDocumentEditorAvailable(ref editor)) {
        return;
      }

      AddOrUpdateExamplesMenu(editor);

      Instances.CanvasCreated -= OnStartup;
    }

    private static void AddOrUpdateExamplesMenu(GH_DocumentEditor editor) {
      AddOrUpdateExamplesMenu(editor.MainMenuStrip.Items);
    }

    internal static void AddOrUpdateExamplesMenu(ToolStripItemCollection items) {
      if (!items.ContainsKey(Name)) {
        items.Add(examplesMenu);
      } else {
        examplesMenu = (ToolStripMenuItem)items[Name];
        lock (examplesMenuLock) {
          examplesMenu.DropDown.Items.Add(new ToolStripSeparator());
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
          PopulateSub(
            examplesMenu); // we cannot change the signature of this method to async, so we fire and forget the population of the menu, which will update when the files are retrieved
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
      }
    }

    internal static bool EnsureDocumentEditorAvailable(ref GH_DocumentEditor editor) {
      DateTime start = DateTime.UtcNow;
      while (editor == null && DateTime.UtcNow - start < _editorAvailabilityTimeout) {
        editor = _getDocumentEditor();
        if (editor != null) {
          break;
        }

        _sleep(321);
      }

      if (editor != null) {
        return true;
      }

      // Failed to obtain the document editor within the timeout; skip adding the menu and fail gracefully.
      Instances.CanvasCreated -= OnStartup;
      ShowMessage(MenuState.FailedToInitialize);
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
            => ShowMessage(FileState.NoFilesFound, string.Empty)));
        } else {
          ShowMessage(FileState.NoFilesFound, string.Empty);
        }
      }
    }

    private static async Task AddExampleFilesAsync(ToolStripMenuItem menuItem) {
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

    private static void AddFileMenuItem(ToolStripMenuItem menuItem, FileEntry file) {
      menuItem.DropDown.Items.Add(file.Name, null, async (s, a) => {
        try {
          await exampleFileManager.DownloadAndOpenFileAsync(file, OpenFile);
        } catch (Exception) {
          ShowMessage(FileState.NoFilesFound, string.Empty);
        }
      });
    }

    internal static bool OpenFile(string path) {
      return _ghDocumentLoader(path);
    }

    private static bool DefaultDocumentLoader(string path) {
      var io = new GH_DocumentIO();
      if (!io.Open(path)) {
        return false;
      }

      Instances.DocumentEditor.Invoke((Action)(() => Instances.ActiveCanvas.Document = io.Document));
      return true;
    }
  }
}
