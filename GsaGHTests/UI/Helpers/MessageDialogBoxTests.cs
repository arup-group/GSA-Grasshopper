using System;
using System.IO;
using System.Windows.Forms;

using GsaGH.UI;

using Moq;

using Xunit;

namespace GsaGHTests.UI {
  [Collection("RunOneByOne")]
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
    [InlineData(MessageDialogBox.FileState.Downloaded, "File downloaded to: somePath", "Download Complete",
      DialogResult.OK)]
    [InlineData(MessageDialogBox.FileState.OpenFailed, "Failed to open the Grasshopper file: someName", "Error",
      DialogResult.Abort)]
    [InlineData(MessageDialogBox.FileState.DownloadFailed, "Download of the file: someName, failed.", "Error",
      DialogResult.Abort)]
    [InlineData(MessageDialogBox.FileState.NoFilesFound, "Couldn't find any sample files. Please contact support.",
      "Error", DialogResult.Abort)]
    [InlineData(MessageDialogBox.FileState.InvalidDownloadPath,
      "Custom download path must be an existing absolute path.", "Error", DialogResult.Abort)]
    public void ShowMessage_ShouldDisplayMessage_WhenFileStateIs(
      MessageDialogBox.FileState state, string errorMessage, string title, DialogResult expectedResult) {
      DialogResult result = MessageDialogBox.ShowMessage(state, "someName", "somePath");

      Assert.Equal(expectedResult, result);
      _mockMessageBox.Verify(m => m.Show(errorMessage, title), Times.Once);
    }

    [Fact]
    public void ShowMessage_ShouldDisplayMessage_WhenMenuStateIsInvalid() {
      MessageDialogBox.ShowMessage(MessageDialogBox.MenuState.FailedToInitialize);

      _mockMessageBox.Verify(
        m => m.Show(
          "Unable to initialize the Examples menu because the Grasshopper document editor did not become available in time.",
          "GSA Examples", MessageBoxButtons.OK, MessageBoxIcon.Warning), Times.Once);
    }

    [Fact]
    public void ShowMessage_ShouldDisplayMessage_WhenOverrideQuestion() {
      string name = "testfile.txt";
      string path = Path.Combine("some", "path");

      string expectedMessage = $"File \"{name}\" already exists in {path}. Overwrite?";
      string expectedTitle = "File Exists";

      _mockMessageBox
       .Setup(m => m.Show(expectedMessage, expectedTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
       .Returns(DialogResult.OK);

      DialogResult result = MessageDialogBox.ShowMessage(MessageDialogBox.FileState.OverrideQuestion, name, path);

      _mockMessageBox.Verify(
        m => m.Show(expectedMessage, expectedTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question), Times.Once);
      Assert.Equal(DialogResult.OK, result);
    }

    [Fact]
    public void ShowMessage_ShouldReturnCorrectDialogResult_WhenOverrideQuestionCancelled() {
      string name = "testfile2.txt";
      string path = Path.Combine("some", "otherpath");

      string message = $"File \"{name}\" already exists in {path}. Overwrite?";
      string fileExists = "File Exists";

      _mockMessageBox.Setup(m => m.Show(message, fileExists, MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
       .Returns(DialogResult.Cancel);

      DialogResult result = MessageDialogBox.ShowMessage(MessageDialogBox.FileState.OverrideQuestion, name, path);

      _mockMessageBox.Verify(m => m.Show(message, fileExists, MessageBoxButtons.OKCancel, MessageBoxIcon.Question),
        Times.Once);

      Assert.Equal(DialogResult.Cancel, result);
    }

    [Fact]
    public void ShowMessage_ShouldNotShowMessage_WhenSuccessState() {
      MessageDialogBox.ShowMessage(MessageDialogBox.FileState.Success, "name");

      _mockMessageBox.Verify(m => m.Show(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ShowMessage_ShouldHandleAllFileOpenStates() {
      Array allFileOpenStates = Enum.GetValues(typeof(MessageDialogBox.FileState));

      foreach (object state in allFileOpenStates) {
        bool isHandled = false;

        try {
          MessageDialogBox.ShowMessage((MessageDialogBox.FileState)state, "testfile", "C:\\path");
          isHandled = true;
        } catch {
          isHandled = false;
        }

        Assert.True(isHandled, $"State '{state}' is not handled in ShowMessage.");
      }
    }

    [Fact]
    public void ShowMessage_ShouldReturnCancel_WhenCancelledState() {
      DialogResult result = MessageDialogBox.ShowMessage(MessageDialogBox.FileState.Cancelled, "name", "path");

      Assert.Equal(DialogResult.Cancel, result);
      _mockMessageBox.Verify(m => m.Show(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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
