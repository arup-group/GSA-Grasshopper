using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using GsaGH.UI;

using Moq;

using Xunit;

namespace GsaGHTests.UI {
  public class ExampleFileManagerTests {
    private class TestMessageBoxWrapper : IMessageBoxWrapper {
      public int ShowCallCount { get; private set; } = 0;
      private readonly DialogResult _result;

      public TestMessageBoxWrapper(DialogResult result) {
        _result = result;
      }

      public DialogResult Show(string message, string title) {
        ShowCallCount++;
        return _result;
      }

      public DialogResult Show(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon) {
        ShowCallCount++;
        return _result;
      }
    }

    private static string SetTempPath(Mock<IHttpsFileDownloader> downloader) {
      string path = Path.GetTempFileName();
      File.WriteAllText(path, "test");
      downloader.Setup(d => d.GetFullDownloadPath(It.IsAny<FileEntry>())).Returns(path);
      return path;
    }

    private bool OpenFileMock(string path) {
      return string.IsNullOrEmpty(path);
    }

    [Fact]
    public async Task GetExampleFilesAsync_ReturnsFiles_WhenDownloaderSucceeds() {
      var files = new List<FileEntry> {
        new FileEntry {
          Name = "a",
          Url = "u",
        },
      };
      var downloader = new Mock<IHttpsFileDownloader>();
      downloader.Setup(d => d.GetFilesFromWebPageAsync()).ReturnsAsync(files);

      MessageDialogBox.SetMessageBoxWrapper(new TestMessageBoxWrapper(DialogResult.OK));
      var manager = new ExampleFileManager(downloader.Object);

      List<FileEntry> result = await manager.GetExampleFilesAsync();

      Assert.Equal(files, result);
    }

    [Fact]
    public async Task GetExampleFilesAsync_ReturnsEmptyList_WhenDownloaderThrows() {
      var downloader = new Mock<IHttpsFileDownloader>();
      downloader.Setup(d => d.GetFilesFromWebPageAsync()).ThrowsAsync(new Exception());

      var wrapper = new TestMessageBoxWrapper(DialogResult.OK);
      MessageDialogBox.SetMessageBoxWrapper(wrapper);
      var manager = new ExampleFileManager(downloader.Object);

      List<FileEntry> result = await manager.GetExampleFilesAsync();

      Assert.Empty(result);
      Assert.True(wrapper.ShowCallCount > 0);
    }

    [Fact]
    public void IsOverwriteApproved_ReturnsTrue_WhenFileDoesNotExist() {
      var downloader = new Mock<IHttpsFileDownloader>();
      downloader.Setup(d => d.GetFullDownloadPath(It.IsAny<FileEntry>())).Returns("notfound.txt");

      var wrapper = new TestMessageBoxWrapper(DialogResult.Cancel);
      MessageDialogBox.SetMessageBoxWrapper(wrapper);
      var manager = new ExampleFileManager(downloader.Object);

      Assert.True(manager.IsOverwriteApproved(new FileEntry()));
    }

    [Fact]
    public void IsOverwriteApproved_ReturnsTrue_WhenUserApprovesOverwrite() {
      var downloader = new Mock<IHttpsFileDownloader>();
      string path = SetTempPath(downloader);

      var wrapper = new TestMessageBoxWrapper(DialogResult.OK);
      MessageDialogBox.SetMessageBoxWrapper(wrapper);
      var manager = new ExampleFileManager(downloader.Object);

      Assert.True(manager.IsOverwriteApproved(new FileEntry()));
      File.Delete(path);
      Assert.True(wrapper.ShowCallCount > 0);
    }

    [Fact]
    public void IsOverwriteApproved_ReturnsFalse_WhenUserCancelsOverwrite() {
      var downloader = new Mock<IHttpsFileDownloader>();
      string path = SetTempPath(downloader);

      var wrapper = new TestMessageBoxWrapper(DialogResult.Cancel);
      MessageDialogBox.SetMessageBoxWrapper(wrapper);
      var manager = new ExampleFileManager(downloader.Object);

      Assert.False(manager.IsOverwriteApproved(new FileEntry()));
      File.Delete(path);
      Assert.True(wrapper.ShowCallCount > 0);
    }

    [Fact]
    public async Task DownloadAndOpenFileAsync_ReturnsCancelled_WhenUserCancelsOverwrite() {
      var downloader = new Mock<IHttpsFileDownloader>();
      string path = SetTempPath(downloader);

      MessageDialogBox.SetMessageBoxWrapper(new TestMessageBoxWrapper(DialogResult.Cancel));
      var manager = new ExampleFileManager(downloader.Object);

      DialogResult result = await manager.DownloadAndOpenFileAsync(new FileEntry {
        Name = "file.gh",
      }, OpenFileMock);

      Assert.Equal(DialogResult.Cancel, result);
      File.Delete(path);
    }

    [Fact]
    public async Task DownloadAndOpenFileAsync_ReturnsDownloaded_WhenNotGhFile() {
      var downloader = new Mock<IHttpsFileDownloader>();
      string path = Path.GetTempFileName() + ".txt";
      downloader.Setup(d => d.GetFullDownloadPath(It.IsAny<FileEntry>())).Returns(path);
      downloader.Setup(d => d.DownloadFileAsync(It.IsAny<FileEntry>())).Returns(Task.CompletedTask);

      MessageDialogBox.SetMessageBoxWrapper(new TestMessageBoxWrapper(DialogResult.Yes));
      var manager = new ExampleFileManager(downloader.Object);

      DialogResult result = await manager.DownloadAndOpenFileAsync(new FileEntry {
        Name = "file.txt",
      }, OpenFileMock);

      Assert.Equal(DialogResult.Yes, result);
      File.Delete(path);
    }

    [Fact]
    public async Task DownloadAndOpenFileAsync_ReturnsDownloadFailed_OnException() {
      var downloader = new Mock<IHttpsFileDownloader>();
      string path = Path.GetTempFileName() + ".txt";
      downloader.Setup(d => d.GetFullDownloadPath(It.IsAny<FileEntry>())).Returns(path);
      downloader.Setup(d => d.DownloadFileAsync(It.IsAny<FileEntry>())).ThrowsAsync(new Exception());

      MessageDialogBox.SetMessageBoxWrapper(new TestMessageBoxWrapper(DialogResult.Abort));
      var manager = new ExampleFileManager(downloader.Object);

      DialogResult result = await manager.DownloadAndOpenFileAsync(new FileEntry {
        Name = "file.txt",
      }, OpenFileMock);

      Assert.Equal(DialogResult.Abort, result);
    }

    [Fact]
    public async Task DownloadAndOpenFileAsync_ReturnsSuccess_WhenOpenGhFileFuncReturnsTrue() {
      var downloader = new Mock<IHttpsFileDownloader>();
      string path = Path.GetTempFileName() + ".gh";
      downloader.Setup(d => d.GetFullDownloadPath(It.IsAny<FileEntry>())).Returns(path);
      downloader.Setup(d => d.DownloadFileAsync(It.IsAny<FileEntry>())).Returns(Task.CompletedTask);

      MessageDialogBox.SetMessageBoxWrapper(new TestMessageBoxWrapper(DialogResult.OK));
      var manager = new ExampleFileManager(downloader.Object);

      DialogResult result = await manager.DownloadAndOpenFileAsync(new FileEntry {
        Name = "file.gh",
      }, _ => true);

      Assert.Equal(DialogResult.OK, result);
      File.Delete(path);
    }

    [Fact]
    public async Task DownloadAndOpenFileAsync_ReturnsOpenFailed_WhenOpenGhFileFuncReturnsFalse() {
      var downloader = new Mock<IHttpsFileDownloader>();
      string path = Path.GetTempFileName() + ".gh";
      downloader.Setup(d => d.GetFullDownloadPath(It.IsAny<FileEntry>())).Returns(path);
      downloader.Setup(d => d.DownloadFileAsync(It.IsAny<FileEntry>())).Returns(Task.CompletedTask);

      MessageDialogBox.SetMessageBoxWrapper(new TestMessageBoxWrapper(DialogResult.Retry));
      var manager = new ExampleFileManager(downloader.Object);

      DialogResult result = await manager.DownloadAndOpenFileAsync(new FileEntry {
        Name = "file.gh",
      }, _ => false);

      Assert.Equal(DialogResult.Retry, result);
      File.Delete(path);
    }

    [Fact]
    public async Task DownloadAndOpenFileAsync_ThrowsArgumentNullException_WhenOpenGhFileFuncIsNull() {
      var downloader = new Mock<IHttpsFileDownloader>();
      string path = Path.GetTempFileName() + ".gh";
      downloader.Setup(d => d.GetFullDownloadPath(It.IsAny<FileEntry>())).Returns(path);
      downloader.Setup(d => d.DownloadFileAsync(It.IsAny<FileEntry>())).Returns(Task.CompletedTask);

      MessageDialogBox.SetMessageBoxWrapper(new TestMessageBoxWrapper(DialogResult.OK));
      var manager = new ExampleFileManager(downloader.Object);

      await Assert.ThrowsAsync<ArgumentNullException>(()
        => manager.DownloadAndOpenFileAsync(new FileEntry {
          Name = "file.gh",
        }, null));
      File.Delete(path);
    }

  }
}
