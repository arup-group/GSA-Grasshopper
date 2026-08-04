using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using GsaGH.UI;

using Moq;

using Xunit;

namespace GsaGHTests.UI.Helpers {
  [Collection("GrasshopperFixture collection")]
  public class MessageDialogBoxTests : IDisposable {
    private readonly Mock<IMessageBoxWrapper> _mockMessageBox;
    private readonly IMessageBoxWrapper _originalWrapper;

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
    public void ShowMessage_DisplaysMessageAndReturnsExpectedResult_ForFileState(
      MessageDialogBox.FileState state, string errorMessage, string title, DialogResult expectedResult) {
      DialogResult result = MessageDialogBox.ShowMessage(state, "someName", "somePath");

      Assert.Equal(expectedResult, result);
      _mockMessageBox.Verify(m => m.Show(errorMessage, title), Times.Once);
    }

    [Fact]
    public void ShowMessage_ReturnsOk_WithoutShowingMessage_WhenSuccessState() {
      DialogResult result = MessageDialogBox.ShowMessage(MessageDialogBox.FileState.Success, "name");

      Assert.Equal(DialogResult.OK, result);
      _mockMessageBox.Verify(m => m.Show(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ShowMessage_ReturnsCancel_WithoutShowingMessage_WhenCancelledState() {
      DialogResult result = MessageDialogBox.ShowMessage(MessageDialogBox.FileState.Cancelled, "name", "path");

      Assert.Equal(DialogResult.Cancel, result);
      _mockMessageBox.Verify(m => m.Show(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData(DialogResult.OK)]
    [InlineData(DialogResult.Cancel)]
    public void ShowMessage_ReturnsWrapperResult_ForOverrideQuestion(DialogResult wrapperResult) {
      string name = "testfile.txt";
      string path = Path.Combine("some", "path");
      string expectedMessage = $"File \"{name}\" already exists in {path}. Overwrite?";
      _mockMessageBox
       .Setup(m => m.Show(expectedMessage, "File Exists", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
       .Returns(wrapperResult);

      DialogResult result = MessageDialogBox.ShowMessage(MessageDialogBox.FileState.OverrideQuestion, name, path);

      Assert.Equal(wrapperResult, result);
      _mockMessageBox.Verify(
        m => m.Show(expectedMessage, "File Exists", MessageBoxButtons.OKCancel, MessageBoxIcon.Question), Times.Once);
    }

    [Fact]
    public void ShowMessage_DisplaysWarning_WhenMenuStateIsFailedToInitialize() {
      MessageDialogBox.ShowMessage(MessageDialogBox.MenuState.FailedToInitialize);

      _mockMessageBox.Verify(
        m => m.Show(
          "Unable to initialize the Examples menu because the Grasshopper document editor did not become available in time.",
          "GSA Examples", MessageBoxButtons.OK, MessageBoxIcon.Warning), Times.Once);
    }

    [Fact]
    public void ShowMessage_HandlesAllFileStates_WithoutThrowing() {
      var results = new List<DialogResult>();
      Array states = Enum.GetValues(typeof(MessageDialogBox.FileState));

      foreach (MessageDialogBox.FileState state in states) {
        // call must not throw and must return a DialogResult
        DialogResult result = MessageDialogBox.ShowMessage(state, "testfile", "C:\\path");
        Assert.IsType<DialogResult>(result);
        results.Add(result);
      }

      // ensure we exercised every enum value
      Assert.Equal(states.Length, results.Count);
    }

    [Fact]
    public void SetMessageBoxWrapper_DoesNotReplaceWrapper_WhenNullPassed() {
      object currentWrapper = MessageDialogBox.MessageBoxWrapper;

      MessageDialogBox.SetMessageBoxWrapper(null);

      Assert.Equal(currentWrapper, MessageDialogBox.MessageBoxWrapper);
    }

    [Fact]
    public void SetMessageBoxWrapper_ReplacesWrapper_WhenValidPassed() {
      var newMock = new Mock<IMessageBoxWrapper>();

      MessageDialogBox.SetMessageBoxWrapper(newMock.Object);

      Assert.Equal(newMock.Object, MessageDialogBox.MessageBoxWrapper);
    }
  }
}
