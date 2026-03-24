using System;
using System.Windows.Forms;

using GsaGH.UI;

using Moq;

using Xunit;

namespace GsaGHTests.UI {
  public class MessageDialogBoxTests {
    private readonly Mock<IMessageBoxWrapper> _mockMessageBox;

    public MessageDialogBoxTests() {
      _mockMessageBox = new Mock<IMessageBoxWrapper>();
      MessageDialogBox.SetMessageBoxWrapper(_mockMessageBox.Object);
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
      // No additional parameters required for this state.
      MessageDialogBox.ShowMessage(MessageDialogBox.FileOpenState.NoFilesFound, "testfile.txt");

      _mockMessageBox.Verify(
        m => m.Show("Couldn't find any files in sample site. Please contact with support.", "Error"), Times.Once);
    }

    [Fact]
    public void ShowMessage_ShouldDisplayMessage_WhenOverrideQuestion() {
      string name = "testfile.txt";
      string path = "C:\\Downloads";

      _mockMessageBox
       .Setup(m => m.Show(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButtons>(),
          It.IsAny<MessageBoxIcon>())).Returns(DialogResult.OK); // Symulation OK

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
          It.IsAny<MessageBoxIcon>())).Returns(DialogResult.Cancel); // Symulation Cancel

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
  }
}
