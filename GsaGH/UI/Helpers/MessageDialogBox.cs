using System.Windows.Forms;

namespace GsaGH.UI.Helpers {
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
    internal static IMessageBoxWrapper MessageBoxWrapper { get; private set; } = new MessageBoxWrapper();

    public enum FileState {
      Success,
      OpenFailed,
      Downloaded,
      DownloadFailed,
      NoFilesFound,
      Cancelled,
      OverrideQuestion,
      InvalidDownloadPath,
    }

    public enum MenuState {
      FailedToInitialize,
    }

    public static void SetMessageBoxWrapper(IMessageBoxWrapper messageBoxWrapper) {
      MessageBoxWrapper = messageBoxWrapper ?? MessageBoxWrapper;
    }

    public static DialogResult ShowMessage(FileState state, string name, string path = "") {
      const string errorTitle = "Error";
      switch (state) {
        case FileState.Success: return DialogResult.OK;
        case FileState.Downloaded:
          MessageBoxWrapper.Show($"File downloaded to: {path}", "Download Complete");
          return DialogResult.OK;
        case FileState.OpenFailed:
          MessageBoxWrapper.Show($"Failed to open the Grasshopper file: {name}", errorTitle);
          break;
        case FileState.DownloadFailed:
          MessageBoxWrapper.Show($"Download of the file: {name}, failed.", errorTitle);
          break;
        case FileState.NoFilesFound:
          MessageBoxWrapper.Show("Couldn't find any sample files. Please contact support.", errorTitle);
          break;
        case FileState.Cancelled: return DialogResult.Cancel;
        case FileState.OverrideQuestion:
          return MessageBoxWrapper.Show($"File \"{name}\" already exists in {path}. Overwrite?", "File Exists",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        case FileState.InvalidDownloadPath:
          MessageBoxWrapper.Show("Custom download path must be an existing absolute path.", errorTitle);
          break;
      }

      return DialogResult.Abort;
    }

    public static DialogResult ShowMessage(MenuState state) {
      switch (state) {
        case MenuState.FailedToInitialize:
          MessageBoxWrapper.Show(
            "Unable to initialize the Examples menu because the Grasshopper document editor did not become available in time.",
            "GSA Examples", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          break;
      }

      return DialogResult.Abort;
    }
  }
}
