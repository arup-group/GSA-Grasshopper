using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using GsaGH.UI;

using Moq;

using Xunit;

namespace GsaGHTests.UI {
  [Collection("RunOneByOne")]
  public class ExampleFileManagerTests : IDisposable {
    private readonly List<string> _tempFiles = new List<string>();
    private readonly IMessageBoxWrapper _originalWrapper;

    public ExampleFileManagerTests() {
      _originalWrapper = MessageDialogBox.MessageBoxWrapper;
    }

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

    private static TestMessageBoxWrapper SetMessageBoxWrapper(DialogResult result) {
      var wrapper = new TestMessageBoxWrapper(result);
      MessageDialogBox.SetMessageBoxWrapper(wrapper);
      return wrapper;
    }

    private ExampleFileManager CreateManagerWithFiles(
      List<FileEntry> files = null, DialogResult dialogResult = DialogResult.OK,
      Action<Mock<IHttpsFileDownloader>> setupDownloader = null) {
      var downloader = new Mock<IHttpsFileDownloader>();
      if (files != null) {
        downloader.Setup(d => d.GetFilesFromWebPageAsync()).ReturnsAsync(files);
      }

      setupDownloader?.Invoke(downloader);
      SetMessageBoxWrapper(dialogResult);
      return new ExampleFileManager(downloader.Object);
    }

    private string CreateTempFile(string extension = ".gh") {
      string path = Path.Combine(Path.GetTempPath(), $"file{_tempFiles.Count + 1}") + extension;
      File.WriteAllText(path, "test");
      _tempFiles.Add(path);
      return path;
    }

    private static bool OpenFileMock(string path) {
      return string.IsNullOrEmpty(path);
    }

    public void Dispose() {
      try {
        foreach (string file in _tempFiles.Where(File.Exists)) {
          File.Delete(file);
        }
      } finally {
        MessageDialogBox.SetMessageBoxWrapper(_originalWrapper);
      }
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenDownloaderIsNull() {
      Assert.Throws<ArgumentNullException>(() => new ExampleFileManager(null));
    }

    [Fact]
    public async Task GetExampleFilesAsync_ReturnsFiles_WhenDownloaderSucceeds() {
      var files = new List<FileEntry> {
        new FileEntry {
          Name = "a",
          Url = "u",
        },
      };
      ExampleFileManager manager = CreateManagerWithFiles(files);

      List<FileEntry> result = await manager.GetExampleFilesAsync();

      Assert.Equal(files, result);
    }

    [Fact]
    public async Task GetExampleFilesAsync_ReturnsEmptyList_WhenDownloaderThrows() {
      var downloader = new Mock<IHttpsFileDownloader>();
      downloader.Setup(d => d.GetFilesFromWebPageAsync()).ThrowsAsync(new Exception());
      TestMessageBoxWrapper wrapper = SetMessageBoxWrapper(DialogResult.OK);
      var manager = new ExampleFileManager(downloader.Object);

      List<FileEntry> result = await manager.GetExampleFilesAsync();

      Assert.Empty(result);
      Assert.True(wrapper.ShowCallCount > 0);
    }

    [Fact]
    public void IsOverwriteApproved_ReturnsTrue_WhenFileDoesNotExist() {
      ExampleFileManager manager = CreateManagerWithFiles(
        setupDownloader: d => d.Setup(x => x.GetFullDownloadPath(It.IsAny<FileEntry>())).Returns("notfound.txt"),
        dialogResult: DialogResult.Cancel);

      Assert.True(manager.IsOverwriteApproved(new FileEntry()));
    }

    [Fact]
    public void IsOverwriteApproved_ReturnsTrue_WhenUserApprovesOverwrite() {
      string path = CreateTempFile();
      ExampleFileManager manager = CreateManagerWithFiles(
        setupDownloader: d => d.Setup(x => x.GetFullDownloadPath(It.IsAny<FileEntry>())).Returns(path),
        dialogResult: DialogResult.OK);

      Assert.True(manager.IsOverwriteApproved(new FileEntry()));
    }

    [Fact]
    public void IsOverwriteApproved_ReturnsFalse_WhenUserCancelsOverwrite() {
      string path = CreateTempFile();
      ExampleFileManager manager = CreateManagerWithFiles(
        setupDownloader: d => d.Setup(x => x.GetFullDownloadPath(It.IsAny<FileEntry>())).Returns(path),
        dialogResult: DialogResult.Cancel);

      Assert.False(manager.IsOverwriteApproved(new FileEntry()));
    }

    [Fact]
    public async Task DownloadAndOpenFileAsync_ThrowsArgumentNullException_WhenFileIsNull() {
      ExampleFileManager manager = CreateManagerWithFiles(dialogResult: DialogResult.OK);

      await Assert.ThrowsAsync<ArgumentNullException>(() => manager.DownloadAndOpenFileAsync(null, _ => true));
    }

    [Fact]
    public async Task DownloadAndOpenFileAsync_ThrowsArgumentNullException_WhenOpenGhFileFuncIsNull() {
      ExampleFileManager manager = CreateManagerWithFiles(dialogResult: DialogResult.OK);

      await Assert.ThrowsAsync<ArgumentNullException>(()
        => manager.DownloadAndOpenFileAsync(new FileEntry {
          Name = "test.gh",
        }, null));
    }

    [Fact]
    public async Task DownloadAndOpenFileAsync_ReturnsCancelled_WhenUserCancelsOverwrite() {
      string path = CreateTempFile();
      ExampleFileManager manager = CreateManagerWithFiles(
        setupDownloader: d => d.Setup(x => x.GetFullDownloadPath(It.IsAny<FileEntry>())).Returns(path),
        dialogResult: DialogResult.Cancel);

      DialogResult result = await manager.DownloadAndOpenFileAsync(new FileEntry {
        Name = Path.GetFileName(path),
      }, OpenFileMock);

      Assert.Equal(DialogResult.Cancel, result);
    }

    [Fact]
    public async Task DownloadAndOpenFileAsync_ReturnsDownloaded_WhenNotGhFile() {
      string path = CreateTempFile(".txt");
      ExampleFileManager manager = CreateManagerWithFiles(setupDownloader: d => {
        d.Setup(x => x.GetFullDownloadPath(It.IsAny<FileEntry>())).Returns(path);
        d.Setup(x => x.DownloadFileAsync(It.IsAny<FileEntry>())).Returns(Task.CompletedTask);
      }, dialogResult: DialogResult.OK);

      DialogResult result = await manager.DownloadAndOpenFileAsync(new FileEntry {
        Name = Path.GetFileName(path),
      }, OpenFileMock);

      Assert.Equal(DialogResult.OK, result);
    }

    [Fact]
    public async Task DownloadAndOpenFileAsync_ReturnsDownloadFailed_OnException() {
      string path = CreateTempFile(".txt");
      ExampleFileManager manager = CreateManagerWithFiles(setupDownloader: d => {
        d.Setup(x => x.GetFullDownloadPath(It.IsAny<FileEntry>())).Returns(path);
        d.Setup(x => x.DownloadFileAsync(It.IsAny<FileEntry>())).ThrowsAsync(new Exception());
      }, dialogResult: DialogResult.OK);

      DialogResult result = await manager.DownloadAndOpenFileAsync(new FileEntry {
        Name = Path.GetFileName(path),
      }, _ => true);

      Assert.Equal(DialogResult.Abort, result);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public async Task DownloadAndOpenFileAsync_ReturnsCorrectResult_WhenOpeningGhFile(
      bool openFileReturnsTrue, bool expectedResultIsOk) {
      string path = CreateTempFile();
      ExampleFileManager manager = CreateManagerWithFiles(setupDownloader: d => {
        d.Setup(x => x.GetFullDownloadPath(It.IsAny<FileEntry>())).Returns(path);
        d.Setup(x => x.DownloadFileAsync(It.IsAny<FileEntry>())).Returns(Task.CompletedTask);
      }, dialogResult: DialogResult.OK);

      DialogResult result = await manager.DownloadAndOpenFileAsync(new FileEntry {
        Name = Path.GetFileName(path),
      }, _ => openFileReturnsTrue);

      DialogResult expectedResult = expectedResultIsOk ? DialogResult.OK : DialogResult.Abort;
      Assert.Equal(expectedResult, result);
    }
  }
}
