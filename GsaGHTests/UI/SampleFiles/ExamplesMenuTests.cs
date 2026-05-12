using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Xunit;


namespace GsaGHTests.UI.SampleFiles {
  [Collection("GrasshopperFixture collection")]
  public class ExamplesMenuTests 

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

    private class TestMessageBoxWrapper : IMessageBoxWrapper {
      public DialogResult Show(string message, string title) {
        return DialogResult.OK;
      }

      public DialogResult Show(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon) {
        return DialogResult.OK;
      }
    }{

    private static ToolStripMenuItem CreateMenuItem() {
      return new ToolStripMenuItem("Examples");
    }

    private static IExampleFileManager SetFileManager(IExampleFileManager manager) {
      var wrapper = new TestMessageBoxWrapper();
      MessageDialogBox.SetMessageBoxWrapper(wrapper);
      return ExamplesMenu.CreateExampleFileManager(manager);
    }

    [Fact]
    public void CreateExampleFileManager_SetsDefaultFileManager_WhenArgumentIsNull() {
      IExampleFileManager manager = SetFileManager(null);

      Assert.NotNull(manager);
    }

    [Fact]
    public async Task PopulateSub_AddsMenuItems_WhenFilesExist() {
      SetFileManager(new MockExampleFileManager(new List<FileEntry> 
         new FileEntry 
           Name = "Test1.gh"
           Url = "C:\\Examples\\Test1.gh"
         }
         new FileEntry 
           Name = "Test2.gh"
           Url = "C:\\Examples\\Test2.gh"
         }
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
      SetFileManager(new MockExampleFileManager(new List<FileEntry> 
         new FileEntry 
           Name = "ąęść.gh"
           Url = "C:\\Examples\\ąęść.gh"
         }
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
      FieldInfo field = typeof(ExamplesMenu).GetField("exampleFileManager"
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


