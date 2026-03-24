using System.Windows.Forms;

namespace GsaGH.UI {
  public interface IMessageBoxWrapper {
    DialogResult Show(string message, string title);
    DialogResult Show(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon);
  }

  public class MessageBoxWrapper : IMessageBoxWrapper {
    public DialogResult Show(string message, string title) {
      return MessageBox.Show(message, title);
    }

    public DialogResult Show(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon) {
      return MessageBox.Show(message, title, buttons, icon);
    }
  }

  public static class MessageDialogBox {
    private static IMessageBoxWrapper _messageBoxWrapper = new MessageBoxWrapper();

    public enum FileOpenState {
      Success,
      Downloaded,
      OpenFailed,
      DownloadFailed,
      NoFilesFound,
      Cancelled,
      OverrideQuestion,
    }

    public static void SetMessageBoxWrapper(IMessageBoxWrapper messageBoxWrapper) {
      _messageBoxWrapper = messageBoxWrapper;
    }

    public static DialogResult ShowMessage(FileOpenState state, string name, string path = "") {
      string errorTitle = "Error";
      switch (state) {
        case FileOpenState.Success: break;
        case FileOpenState.Downloaded:
          _messageBoxWrapper.Show($"File downloaded to: {path}", "Download Complete");
          break;
        case FileOpenState.OpenFailed:
          _messageBoxWrapper.Show($"Failed to open the Grasshopper file: {name}", errorTitle);
          break;
        case FileOpenState.DownloadFailed:
          _messageBoxWrapper.Show($"Download of the file: {name}, failed.", errorTitle);
          break;
        case FileOpenState.NoFilesFound:
          _messageBoxWrapper.Show("Couldn't find any files in sample site. Please contact with support.", errorTitle);
          break;
        case FileOpenState.OverrideQuestion:
          return _messageBoxWrapper.Show($"File \"{name}\" already exists in {path}. Overwrite?", "File Exists",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
      }

      return DialogResult.OK;
    }
  }
}
