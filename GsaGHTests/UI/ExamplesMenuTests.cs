using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Grasshopper.GUI;

using GsaGH.Graphics.Menu;
using GsaGH.UI;

using Xunit;

namespace GsaGHTests.UI {
  [Collection("RunOneByOne")]
  public class ExamplesMenuTests : IDisposable {
    private readonly IMessageBoxWrapper _originalWrapper;
    private readonly Func<GH_DocumentEditor> _originalGetDocumentEditor;
    private readonly Action<int> _originalSleep;
    private readonly TimeSpan _originalTimeout;
    private readonly Func<string, bool> _originalGhDocumentLoader;

    public ExamplesMenuTests() {
      _originalWrapper = MessageDialogBox.MessageBoxWrapper;
      _originalGetDocumentEditor = ExamplesMenu.GetDocumentEditor;
      _originalSleep = ExamplesMenu.Sleep;
      _originalTimeout = ExamplesMenu.EditorAvailabilityTimeout;
      _originalGhDocumentLoader = ExamplesMenu.GhDocumentLoader;
    }

    public void Dispose() {
      try {
        MessageDialogBox.SetMessageBoxWrapper(_originalWrapper);
        ExamplesMenu.GetDocumentEditor = _originalGetDocumentEditor;
        ExamplesMenu.Sleep = _originalSleep;
        ExamplesMenu.EditorAvailabilityTimeout = _originalTimeout;
        ExamplesMenu.GhDocumentLoader = _originalGhDocumentLoader;
      } catch {
        // best-effort cleanup
      }
    }

    private static ToolStripMenuItem CreateMenuItem() {
      return new ToolStripMenuItem("Examples");
    }

    private static IExampleFileManager SetFileManager(IExampleFileManager manager) {
      var wrapper = new TestMessageBoxWrapper();
      MessageDialogBox.SetMessageBoxWrapper(wrapper);
      return ExamplesMenu.CreateExampleFileManager(manager);
    }

    /// <summary>Injects a test environment where the Grasshopper document editor is unavailable and operations complete immediately.</summary>
    private static void InjectNoEditorEnvironment() {
      ExamplesMenu.GetDocumentEditor = () => null;
      ExamplesMenu.Sleep = _ => { };
      ExamplesMenu.EditorAvailabilityTimeout = TimeSpan.FromMilliseconds(1);
      MessageDialogBox.SetMessageBoxWrapper(new TestMessageBoxWrapper());
    }

    [Fact]
    public void CreateExampleFileManager_SetsDefaultFileManager_WhenArgumentIsNull() {
      IExampleFileManager manager = SetFileManager(null);

      Assert.NotNull(manager);
    }

    [Fact]
    public async Task PopulateSub_AddsMenuItems_WhenFilesExist() {
      SetFileManager(new MockExampleFileManager(new List<FileEntry> {
        new FileEntry {
          Name = "Test1.gh",
          Url = "C:\\Examples\\Test1.gh",
        },
        new FileEntry {
          Name = "Test2.gh",
          Url = "C:\\Examples\\Test2.gh",
        },
      }));

      ToolStripMenuItem menuItem = CreateMenuItem();
      await ExamplesMenu.PopulateSub(menuItem);

      Assert.Equal(2, menuItem.DropDownItems.Count);
      Assert.Equal("Test1.gh", menuItem.DropDownItems[0].Text);
      Assert.Equal("Test2.gh", menuItem.DropDownItems[1].Text);
    }

    [Fact]
    public async Task PopulateSub_DoesNotAddMenuItems_WhenNoFilesExist() {
      SetFileManager(new MockExampleFileManager(new List<FileEntry>()));

      ToolStripMenuItem menuItem = CreateMenuItem();
      await ExamplesMenu.PopulateSub(menuItem);

      Assert.Empty(menuItem.DropDownItems);
    }

    [Fact]
    public async Task PopulateSub_DoesNotAddMenuItems_WhenFileListIsNull() {
      SetFileManager(new MockExampleFileManager(null));

      ToolStripMenuItem menuItem = CreateMenuItem();
      await ExamplesMenu.PopulateSub(menuItem);

      Assert.Empty(menuItem.DropDownItems);
    }

    [Fact]
    public async Task PopulateSub_AddsMenuItems_WithSpecialCharacters() {
      SetFileManager(new MockExampleFileManager(new List<FileEntry> {
        new FileEntry {
          Name = "ąęść.gh",
          Url = "C:\\Examples\\ąęść.gh",
        },
      }));

      ToolStripMenuItem menuItem = CreateMenuItem();
      await ExamplesMenu.PopulateSub(menuItem);

      Assert.Single(menuItem.DropDownItems);
      Assert.Equal("ąęść.gh", menuItem.DropDownItems[0].Text);
    }

    [Fact]
    public async Task PopulateSub_DoesNotAddMenuItems_WhenFileManagerThrows() {
      SetFileManager(new ExceptionThrowingFileManager());

      ToolStripMenuItem menuItem = CreateMenuItem();
      await ExamplesMenu.PopulateSub(menuItem);

      Assert.Empty(menuItem.DropDownItems);
    }

    [Fact]
    public void CreateExampleFileManager_ReturnsNewDefaultManager_WhenBothNull() {
      FieldInfo field = typeof(ExamplesMenu).GetField("exampleFileManager",
        BindingFlags.Static | BindingFlags.NonPublic);
      object originalValue = field.GetValue(null);
      field.SetValue(null, null);
      try {
        IExampleFileManager result = ExamplesMenu.CreateExampleFileManager(null);
        Assert.NotNull(result);
      } finally {
        field.SetValue(null, originalValue);
      }
    }

    [Fact]
    public async Task PopulateSub_ClickHandler_InvokesDownloadAndOpenFile_WhenItemClicked() {
      var mock = new DownloadRecordingManager();
      SetFileManager(mock);

      ToolStripMenuItem menuItem = CreateMenuItem();
      await ExamplesMenu.PopulateSub(menuItem);

      Assert.NotEmpty(menuItem.DropDownItems);
      menuItem.DropDownItems[0].PerformClick();

      Assert.True(mock.WaitForCall(), "DownloadAndOpenFileAsync should have been called after click");
    }

    [Fact]
    public async Task PopulateSub_ClickHandler_ShowsNoFilesError_WhenDownloadThrows() {
      var recordingWrapper = new RecordingMessageBoxWrapper();
      MessageDialogBox.SetMessageBoxWrapper(recordingWrapper);
      ExamplesMenu.CreateExampleFileManager(new DownloadThrowingManager());

      ToolStripMenuItem menuItem = CreateMenuItem();
      await ExamplesMenu.PopulateSub(menuItem);

      Assert.NotEmpty(menuItem.DropDownItems);
      menuItem.DropDownItems[0].PerformClick();

      Assert.True(recordingWrapper.WaitForShow(), "Error message should have been shown after download failure");
    }

    [Fact]
    public void EnsureDocumentEditorAvailable_ReturnsFalse_WhenEditorNeverAvailable() {
      InjectNoEditorEnvironment();

      GH_DocumentEditor editor = null;
      bool result = ExamplesMenu.EnsureDocumentEditorAvailable(ref editor);

      Assert.False(result);
      Assert.Null(editor);
    }

    [Fact]
    public void AddOrUpdateExamplesMenu_AddsNewMenu_WhenMenuNotPresent() {
      SetFileManager(new MockExampleFileManager(new List<FileEntry>()));
      // Populate the examplesMenu static field with a named item for testing.
      var examplesMenuItem = new ToolStripMenuItem(ExamplesMenu.Name) {
        Name = ExamplesMenu.Name,
      };
      typeof(ExamplesMenu)
        .GetField("examplesMenu", BindingFlags.Static | BindingFlags.NonPublic)
        ?.SetValue(null, examplesMenuItem);

      var strip = new MenuStrip();
      ExamplesMenu.AddOrUpdateExamplesMenu(strip.Items);

      Assert.True(strip.Items.ContainsKey(ExamplesMenu.Name));
    }

    [Fact]
    public void AddOrUpdateExamplesMenu_AddsSeparatorToExistingMenu_WhenMenuPresent() {
      SetFileManager(new MockExampleFileManager(new List<FileEntry>()));
      var existingMenu = new ToolStripMenuItem(ExamplesMenu.Name) {
        Name = ExamplesMenu.Name,
      };
      var strip = new MenuStrip();
      strip.Items.Add(existingMenu);

      ExamplesMenu.AddOrUpdateExamplesMenu(strip.Items);

      Assert.NotEmpty(existingMenu.DropDownItems);
      Assert.IsType<ToolStripSeparator>(existingMenu.DropDownItems[0]);
    }

    [Fact]
    public void AddToMainTab_ReturnsSilently_WhenEditorNotAvailable() {
      InjectNoEditorEnvironment();

      // Should complete quickly and without throwing.
      ExamplesMenu.AddToMainTab();
    }

    [Fact]
    public void OnStartup_InitializesFileManagerAndMenu_WhenEditorNotAvailable() {
      InjectNoEditorEnvironment();
      SetFileManager(new MockExampleFileManager(new List<FileEntry>()));

      // Should complete without throwing even though no Grasshopper editor is present.
      ExamplesMenu.OnStartup(null);

      IExampleFileManager result = ExamplesMenu.CreateExampleFileManager(null);
      Assert.NotNull(result);
    }

    [Fact]
    public void OpenFile_ReturnsFalse_WhenLoaderReturnsFalse() {
      ExamplesMenu.GhDocumentLoader = _ => false;

      bool result = ExamplesMenu.OpenFile("nonexistent.gh");

      Assert.False(result);
    }

    [Fact]
    public void OpenFile_ReturnsTrue_WhenLoaderReturnsTrue() {
      ExamplesMenu.GhDocumentLoader = _ => true;

      bool result = ExamplesMenu.OpenFile("test.gh");

      Assert.True(result);
    }

    private class TestMessageBoxWrapper : IMessageBoxWrapper {
      public DialogResult Show(string message, string title) {
        return DialogResult.OK;
      }

      public DialogResult Show(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon) {
        return DialogResult.OK;
      }
    }

    private class DownloadRecordingManager : IExampleFileManager {
      private readonly ManualResetEventSlim _called = new ManualResetEventSlim(false);

      public bool WaitForCall(int milliseconds = 2000) {
        return _called.Wait(milliseconds);
      }

      public Task<List<FileEntry>> GetExampleFilesAsync() {
        return Task.FromResult(new List<FileEntry> {
          new FileEntry {
            Name = "test.gh",
          },
        });
      }

      public Task<DialogResult> DownloadAndOpenFileAsync(FileEntry file, Func<string, bool> openGhFileFunc) {
        _called.Set();
        return Task.FromResult(DialogResult.OK);
      }

      public bool IsOverwriteApproved(FileEntry file) {
        return true;
      }
    }

    private class DownloadThrowingManager : IExampleFileManager {
      public Task<List<FileEntry>> GetExampleFilesAsync() {
        return Task.FromResult(new List<FileEntry> {
          new FileEntry {
            Name = "test.gh",
          },
        });
      }

      public Task<DialogResult> DownloadAndOpenFileAsync(FileEntry file, Func<string, bool> openGhFileFunc) {
        throw new Exception("Download failed");
      }

      public bool IsOverwriteApproved(FileEntry file) {
        return true;
      }
    }

    private class RecordingMessageBoxWrapper : IMessageBoxWrapper {
      private readonly ManualResetEventSlim _shown = new ManualResetEventSlim(false);

      public bool WaitForShow(int milliseconds = 2000) {
        return _shown.Wait(milliseconds);
      }

      public DialogResult Show(string message, string title) {
        _shown.Set();
        return DialogResult.OK;
      }

      public DialogResult Show(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon) {
        _shown.Set();
        return DialogResult.OK;
      }
    }
  }

  public class MockExampleFileManager : IExampleFileManager {
    private readonly List<FileEntry> _files;

    public MockExampleFileManager(List<FileEntry> files) {
      _files = files;
    }

    public Task<List<FileEntry>> GetExampleFilesAsync() {
      return Task.FromResult(_files);
    }

    public Task<DialogResult> DownloadAndOpenFileAsync(FileEntry file, Func<string, bool> openGhFileFunc) {
      throw new NotImplementedException();
    }

    public bool IsOverwriteApproved(FileEntry file) {
      throw new NotImplementedException();
    }
  }

  public class ExceptionThrowingFileManager : IExampleFileManager {
    public Task<List<FileEntry>> GetExampleFilesAsync() {
      throw new Exception("Test error");
    }

    public Task<DialogResult> DownloadAndOpenFileAsync(FileEntry file, Func<string, bool> openGhFileFunc) {
      throw new NotImplementedException();
    }

    public bool IsOverwriteApproved(FileEntry file) {
      throw new NotImplementedException();
    }
  }
}

