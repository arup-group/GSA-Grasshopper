using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using GsaGH.Graphics.Menu;
using GsaGH.UI;

using Xunit;

namespace GsaGHTests.UI {
  [Collection("GrasshopperFixture collection")]
  public class ExamplesMenuTests : IDisposable {
    private static readonly FieldInfo ExampleFileManagerField = typeof(ExamplesMenu).GetField(
      "exampleFileManager", BindingFlags.Static | BindingFlags.NonPublic);

    private readonly IMessageBoxWrapper _originalWrapper;
    private readonly object _originalFileManager;

    public ExamplesMenuTests() {
      _originalWrapper = MessageDialogBox.MessageBoxWrapper;
      _originalFileManager = ExampleFileManagerField.GetValue(null);
    }

    public void Dispose() {
      try {
        ExampleFileManagerField.SetValue(null, _originalFileManager);
      } finally {
        MessageDialogBox.SetMessageBoxWrapper(_originalWrapper);
      }
    }

    private static IExampleFileManager SetFileManager(IExampleFileManager manager) {
      var wrapper = new TestMessageBoxWrapper();
      MessageDialogBox.SetMessageBoxWrapper(wrapper);
      return ExamplesMenu.CreateExampleFileManager(manager);
    }

    [Fact]
    public void CreateExampleFileManager_ReturnsNewDefaultManager_WhenBothNull() {
      ExampleFileManagerField.SetValue(null, null);

      IExampleFileManager result = ExamplesMenu.CreateExampleFileManager(null);

      Assert.NotNull(result);
    }

    [Fact]
    public void CreateExampleFileManager_WithManager_OverridesPrevious() {
      var first = new MockExampleFileManager(new List<FileEntry>());
      var second = new MockExampleFileManager(new List<FileEntry>());
      ExamplesMenu.CreateExampleFileManager(first);

      IExampleFileManager result = ExamplesMenu.CreateExampleFileManager(second);

      Assert.Same(second, result);
    }

    [Fact]
    public void CreateExampleFileManager_CalledTwiceWithNull_ReturnsSameInstance() {
      var mock = new MockExampleFileManager(new List<FileEntry>());
      ExamplesMenu.CreateExampleFileManager(mock);

      IExampleFileManager first = ExamplesMenu.CreateExampleFileManager(null);
      IExampleFileManager second = ExamplesMenu.CreateExampleFileManager(null);

      Assert.Same(first, second);
    }

    [Fact]
    public async Task CreateExamplesMenuItemAsync_ContainsGithubLinkAndSeparator() {
      SetFileManager(new MockExampleFileManager(new List<FileEntry>()));

      ToolStripMenuItem menuItem = await ExamplesMenu.CreateExamplesMenuItemAsync();

      Assert.Equal("GSA Example files", menuItem.Text);
      Assert.Equal(2, menuItem.DropDownItems.Count);
      Assert.Equal("View on GitHub", menuItem.DropDownItems[0].Text);
      Assert.IsType<ToolStripSeparator>(menuItem.DropDownItems[1]);
    }

    [Fact]
    public async Task CreateExamplesMenuItemAsync_AddsFileItems_WhenFilesExist() {
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

      ToolStripMenuItem menuItem = await ExamplesMenu.CreateExamplesMenuItemAsync();

      Assert.Equal(4, menuItem.DropDownItems.Count);
      Assert.Equal("Test1.gh", menuItem.DropDownItems[2].Text);
      Assert.Equal("Test2.gh", menuItem.DropDownItems[3].Text);
    }

    [Fact]
    public async Task CreateExamplesMenuItemAsync_NoFileItems_WhenFileListIsNull() {
      SetFileManager(new MockExampleFileManager(null));

      ToolStripMenuItem menuItem = await ExamplesMenu.CreateExamplesMenuItemAsync();

      Assert.Equal(2, menuItem.DropDownItems.Count);
    }

    [Fact]
    public async Task CreateExamplesMenuItemAsync_AddsFileItems_WithSpecialCharacters() {
      SetFileManager(new MockExampleFileManager(new List<FileEntry> {
        new FileEntry {
          Name = "ąęść.gh",
          Url = "C:\\Examples\\ąęść.gh",
        },
      }));

      ToolStripMenuItem menuItem = await ExamplesMenu.CreateExamplesMenuItemAsync();

      Assert.Equal(3, menuItem.DropDownItems.Count);
      Assert.Equal("ąęść.gh", menuItem.DropDownItems[2].Text);
    }

    [Fact]
    public async Task CreateExamplesMenuItemAsync_ReturnsMenuWithFixedItems_WhenFileManagerThrows() {
      var recordingWrapper = new RecordingMessageBoxWrapper();
      MessageDialogBox.SetMessageBoxWrapper(recordingWrapper);
      ExamplesMenu.CreateExampleFileManager(new ExceptionThrowingFileManager());

      ToolStripMenuItem menuItem = await ExamplesMenu.CreateExamplesMenuItemAsync();

      Assert.True(menuItem.DropDownItems.Count >= 2);
      Assert.True(recordingWrapper.WaitForShow(), "ShowMessage should be called when file manager throws");
    }

    [Fact]
    public async Task CreateExamplesMenuItemAsync_ClickHandler_InvokesDownload_WhenItemClicked() {
      var mock = new DownloadRecordingManager();
      SetFileManager(mock);

      ToolStripMenuItem menuItem = await ExamplesMenu.CreateExamplesMenuItemAsync();

      Assert.True(menuItem.DropDownItems.Count > 2);
      menuItem.DropDownItems[2].PerformClick();

      Assert.True(mock.WaitForCall(), "DownloadAndOpenFileAsync should have been called after click");
    }

    [Fact]
    public async Task CreateExamplesMenuItemAsync_ClickHandler_ShowsError_WhenDownloadThrows() {
      var recordingWrapper = new RecordingMessageBoxWrapper();
      MessageDialogBox.SetMessageBoxWrapper(recordingWrapper);
      ExamplesMenu.CreateExampleFileManager(new DownloadThrowingManager());

      ToolStripMenuItem menuItem = await ExamplesMenu.CreateExamplesMenuItemAsync();

      Assert.True(menuItem.DropDownItems.Count > 2);
      menuItem.DropDownItems[2].PerformClick();

      Assert.True(recordingWrapper.WaitForShow(), "Error message should have been shown after download failure");
    }

    [Fact]
    public async Task CreateExamplesMenuItemAsync_CalledTwice_DoesNotShareState() {
      SetFileManager(new MockExampleFileManager(new List<FileEntry> {
        new FileEntry {
          Name = "File1.gh",
          Url = "C:\\Examples\\File1.gh",
        },
      }));

      ToolStripMenuItem first = await ExamplesMenu.CreateExamplesMenuItemAsync();

      SetFileManager(new MockExampleFileManager(new List<FileEntry> {
        new FileEntry {
          Name = "File2.gh",
          Url = "C:\\Examples\\File2.gh",
        },
      }));

      ToolStripMenuItem second = await ExamplesMenu.CreateExamplesMenuItemAsync();

      Assert.Equal("File1.gh", first.DropDownItems[2].Text);
      Assert.Equal("File2.gh", second.DropDownItems[2].Text);
    }

    [Fact]
    public async Task CreateExamplesMenuItemAsync_ReturnsMenu_WhenGetFilesReturnsCancelledTask() {
      var recordingWrapper = new RecordingMessageBoxWrapper();
      MessageDialogBox.SetMessageBoxWrapper(recordingWrapper);
      ExamplesMenu.CreateExampleFileManager(new CancelledFileManager());

      ToolStripMenuItem menuItem = await ExamplesMenu.CreateExamplesMenuItemAsync();

      Assert.True(menuItem.DropDownItems.Count >= 2);
      Assert.True(recordingWrapper.WaitForShow(), "ShowMessage should be called when task is cancelled");
    }
  }

  // --- Shared test helpers ---

  public class TestMessageBoxWrapper : IMessageBoxWrapper {
    public DialogResult Show(string message, string title) {
      return DialogResult.OK;
    }

    public DialogResult Show(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon) {
      return DialogResult.OK;
    }
  }

  public class RecordingMessageBoxWrapper : IMessageBoxWrapper {
    private readonly ManualResetEventSlim _shown = new ManualResetEventSlim(false);

    public bool ShowCalled => _shown.IsSet;

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
      return true;
    }
  }

  public class CancelledFileManager : IExampleFileManager {
    public Task<List<FileEntry>> GetExampleFilesAsync() {
      var tcs = new TaskCompletionSource<List<FileEntry>>();
      tcs.SetCanceled();
      return tcs.Task;
    }

    public Task<DialogResult> DownloadAndOpenFileAsync(FileEntry file, Func<string, bool> openGhFileFunc) {
      throw new NotImplementedException();
    }

    public bool IsOverwriteApproved(FileEntry file) {
      return true;
    }
  }

  public class DownloadRecordingManager : IExampleFileManager {
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

  public class DownloadThrowingManager : IExampleFileManager {
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
}
