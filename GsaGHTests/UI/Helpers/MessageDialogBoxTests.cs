using System;
using System.Linq;
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

    [Theory]
    [InlineData(MessageDialogBox.FileOpenState.Downloaded, "File downloaded to: somePath", "Download Complete")]
    [InlineData(MessageDialogBox.FileOpenState.OpenFailed, "Failed to open the Grasshopper file: someName", "Error")]
    [InlineData(MessageDialogBox.FileOpenState.DownloadFailed, "Download of the file: someName, failed.", "Error")]
    [InlineData(MessageDialogBox.FileOpenState.NoFilesFound,
      "Couldn't find any sample files. Please contact with support.", "Error")]
    [InlineData(MessageDialogBox.FileOpenState.InvalidDownloadPath,
      "Custom download path must be an existing absolute path.", "Error")]
    public void ShowMessage_ShouldDisplayMessage_WhenStateIsInvalid(
      MessageDialogBox.FileOpenState state, string errorMessage, string title) {
      MessageDialogBox.ShowMessage(state, "someName", "somePath");

      _mockMessageBox.Verify(m => m.Show(errorMessage, title), Times.Once);
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
      MessageDialogBox.ShowMessage(MessageDialogBox.FileOpenState.Success, "name");

      _mockMessageBox.Verify(m => m.Show(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ShowMessage_ShouldHandleAllFileOpenStates() {
      Array allFileOpenStates = Enum.GetValues(typeof(MessageDialogBox.FileOpenState));

      foreach (object state in allFileOpenStates) {
        bool isHandled = false;

        try {
          MessageDialogBox.ShowMessage((MessageDialogBox.FileOpenState)state, "testfile", "path");
          isHandled = true;
        } catch {
          isHandled = false;
        }

        Assert.True(isHandled, $"State '{state}' is not handled in ShowMessage.");
      }
    }

    [Fact]
    public void ShowMessage_ShouldReturnCancel_WhenCancelledState() {
      DialogResult result = MessageDialogBox.ShowMessage(MessageDialogBox.FileOpenState.Cancelled, "name", "path");

      Assert.Equal(DialogResult.Cancel, result);
      _mockMessageBox.Verify(m => m.Show(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ShowMessage_ShouldReturnAbort_ForUnhandledStates() {
      MessageDialogBox.FileOpenState[] handledStates = new[] {
        MessageDialogBox.FileOpenState.Success,
        MessageDialogBox.FileOpenState.Downloaded,
        MessageDialogBox.FileOpenState.Cancelled,
        MessageDialogBox.FileOpenState.OverrideQuestion,
      };

      foreach (MessageDialogBox.FileOpenState state in Enum.GetValues(typeof(MessageDialogBox.FileOpenState))) {
        if (handledStates.Contains(state)) {
          continue;
        }

        _mockMessageBox.Invocations.Clear();

        DialogResult result = MessageDialogBox.ShowMessage(state, "testfile.txt", "test");
        Assert.Equal(DialogResult.Abort, result);
        _mockMessageBox.Verify(m => m.Show(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      }
    }

    [Fact]
    public void SetMessageBoxWrapper_ShouldNotChangeWrapper_WhenNullPassed() {
      object currentWrapper = MessageDialogBox.MessageBoxWrapper;

      MessageDialogBox.SetMessageBoxWrapper(null);

      object afterWrapper = MessageDialogBox.MessageBoxWrapper;

      Assert.Equal(currentWrapper, afterWrapper);
    }

    [Fact]
    public void SetMessageBoxWrapper_ShouldChangeWrapper_WhenValidPassed() {
      var newMock = new Mock<IMessageBoxWrapper>();
      MessageDialogBox.SetMessageBoxWrapper(newMock.Object);

      object currentWrapper = MessageDialogBox.MessageBoxWrapper;

      Assert.Equal(newMock.Object, currentWrapper);
    }
  }
}
