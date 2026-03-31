using System;
using System.Reflection;
using System.Windows.Forms;

using GsaGH.UI;

using Moq;

using Xunit;

namespace GsaGHTests.UI {
  public class MessageDialogBoxTests : IDisposable {
    private readonly Mock<IMessageBoxWrapper> _mockMessageBox;
    private readonly IMessageBoxWrapper _originalWrapper;

    /// <summary>
    ///   Initializes a new instance of the <see cref="MessageDialogBoxTests" /> class.
    ///   Stores the original MessageBoxWrapper and sets a mock wrapper for isolation in each test.
    /// </summary>
    public MessageDialogBoxTests() {
      _originalWrapper = MessageDialogBox.MessageBoxWrapper;
      _mockMessageBox = new Mock<IMessageBoxWrapper>();

      MessageDialogBox.SetMessageBoxWrapper(_mockMessageBox.Object);
    }

    public void Dispose() {
      MessageDialogBox.SetMessageBoxWrapper(_originalWrapper);
    }

    [Fact]
    public void ShowMessage_ShouldDisplayMessage_WhenDownloaded() {
      string path = "C:\\Downloads\\testfile.txt";
      MessageDialogBox.ShowMessage(MessageDialogBox.FileOpenState.Downloaded, "testfile.txt", path);

      _mockMessageBox.Verify(m => m.Show($"File downloaded to: {path}", "Download Complete"), Times.Once);
    }

    [Fact]
    public void ShowMessage_ShouldDisplayMessage_WhenOpenFailed() {
      string name = "testfile.txt";
      MessageDialogBox.ShowMessage(MessageDialogBox.FileOpenState.OpenFailed, name);

      _mockMessageBox.Verify(m => m.Show($"Failed to open the Grasshopper file: {name}", "Error"), Times.Once);
    }

    [Fact]
    public void ShowMessage_ShouldDisplayMessage_WhenDownloadFailed() {
      string name = "testfile.txt";
      MessageDialogBox.ShowMessage(MessageDialogBox.FileOpenState.DownloadFailed, name);

      _mockMessageBox.Verify(m => m.Show($"Download of the file: {name}, failed.", "Error"), Times.Once);
    }

    [Fact]
    public void ShowMessage_ShouldDisplayMessage_WhenNoFilesFound() {
      MessageDialogBox.ShowMessage(MessageDialogBox.FileOpenState.NoFilesFound, "testfile.txt");

      _mockMessageBox.Verify(m => m.Show("Couldn't find any sample files. Please contact with support.", "Error"),
        Times.Once);
    }

    [Fact]
    public void ShowMessage_ShouldDisplayMessage_WhenOverrideQuestion() {
      string name = "testfile.txt";
      string path = "C:\\Downloads";

      _mockMessageBox
       .Setup(m => m.Show(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButtons>(),
          It.IsAny<MessageBoxIcon>())).Returns(DialogResult.OK); // Simulation OK

      DialogResult result = MessageDialogBox.ShowMessage(MessageDialogBox.FileOpenState.OverrideQuestion, name, path);

      _mockMessageBox.Verify(
        m => m.Show($"File \"{name}\" already exists in {path}. Overwrite?", "File Exists", MessageBoxButtons.OKCancel,
          MessageBoxIcon.Question), Times.Once);
      Assert.Equal(DialogResult.OK, result);
    }

    [Fact]
    public void ShowMessage_ShouldReturnCorrectDialogResult_WhenOverrideQuestionCancelled() {
      string name = "testfile.txt";
      string path = "C:\\Downloads";

      _mockMessageBox
       .Setup(m => m.Show(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButtons>(),
          It.IsAny<MessageBoxIcon>())).Returns(DialogResult.Cancel); // Simulation Cancel

      DialogResult result = MessageDialogBox.ShowMessage(MessageDialogBox.FileOpenState.OverrideQuestion, name, path);

      _mockMessageBox.Verify(
        m => m.Show($"File \"{name}\" already exists in {path}. Overwrite?", "File Exists", MessageBoxButtons.OKCancel,
          MessageBoxIcon.Question), Times.Once);
      Assert.Equal(DialogResult.Cancel, result);
    }

    [Fact]
    public void ShowMessage_ShouldNotShowMessage_WhenSuccessState() {
      string name = "testfile.txt";
      string path = "C:\\Downloads";

      MessageDialogBox.ShowMessage(MessageDialogBox.FileOpenState.Success, name, path);

      _mockMessageBox.Verify(m => m.Show(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ShowMessage_ShouldHandleAllFileOpenStates() {
      Array allFileOpenStates = Enum.GetValues(typeof(MessageDialogBox.FileOpenState));

      foreach (object state in allFileOpenStates) {
        bool isHandled = false;

        try {
          MessageDialogBox.ShowMessage((MessageDialogBox.FileOpenState)state, "testfile.txt", "C:\\Downloads");
          isHandled = true;
        } catch {
          isHandled = false;
        }

        Assert.True(isHandled, $"State '{state}' is not handled in ShowMessage.");
      }
    }

    [Fact]
    public void ShowMessage_ShouldReturnCancel_WhenCancelledState() {
      string name = "testfile.txt";
      string path = "C:\\Downloads";

      DialogResult result = MessageDialogBox.ShowMessage(MessageDialogBox.FileOpenState.Cancelled, name, path);

      Assert.Equal(DialogResult.Cancel, result);
      _mockMessageBox.Verify(m => m.Show(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ShowMessage_ShouldReturnAbort_ForUnknownState() {
      var unknownState = (MessageDialogBox.FileOpenState)999;

      DialogResult result = MessageDialogBox.ShowMessage(unknownState, "testfile.txt", "C:\\Downloads");

      Assert.Equal(DialogResult.Abort, result);
      _mockMessageBox.Verify(m => m.Show(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void SetMessageBoxWrapper_ShouldNotChangeWrapper_WhenNullPassed() {
      object currentWrapper = typeof(MessageDialogBox)
       .GetField("_messageBoxWrapper", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

      MessageDialogBox.SetMessageBoxWrapper(null);

      object afterWrapper = typeof(MessageDialogBox)
       .GetField("_messageBoxWrapper", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

      Assert.Equal(currentWrapper, afterWrapper);
    }

    [Fact]
    public void SetMessageBoxWrapper_ShouldChangeWrapper_WhenValidPassed() {
      var newMock = new Mock<IMessageBoxWrapper>();
      MessageDialogBox.SetMessageBoxWrapper(newMock.Object);

      object currentWrapper = typeof(MessageDialogBox)
       .GetField("_messageBoxWrapper", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

      Assert.Equal(newMock.Object, currentWrapper);
    }

  }
}
