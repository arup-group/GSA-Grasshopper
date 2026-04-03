using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

using GsaGH.Graphics.Menu;
using GsaGH.UI;

using Xunit;

namespace GsaGHTests.UI {
  [Collection("GrasshopperFixture collection")]
  public class ExamplesMenuTests {
    private static ToolStripMenuItem CreateMenuItem() {
      return new ToolStripMenuItem("Examples");
    }

    private static void SetFileManager(IExampleFileManager manager) {
      var wrapper = new TestMessageBoxWrapper();
      MessageDialogBox.SetMessageBoxWrapper(wrapper);
      ExamplesMenu.CreateExampleFileManager(manager);
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

    private class TestMessageBoxWrapper : IMessageBoxWrapper {
      public DialogResult Show(string message, string title) {
        return DialogResult.OK;
      }

      public DialogResult Show(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon) {
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
