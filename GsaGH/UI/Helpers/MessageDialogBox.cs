using System.Runtime.CompilerServices;
using System.Windows.Forms;

[assembly: InternalsVisibleTo("GsaGHTests")]

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
    internal static IMessageBoxWrapper MessageBoxWrapper { get; private set; } = new MessageBoxWrapper();

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
      MessageBoxWrapper = messageBoxWrapper ?? MessageBoxWrapper;
    }

    public static DialogResult ShowMessage(FileOpenState state, string name, string path = "") {
      const string errorTitle = "Error";
      switch (state) {
        case FileOpenState.Success: return DialogResult.OK;
        case FileOpenState.Cancelled: return DialogResult.Cancel;
        case FileOpenState.Downloaded:
          MessageBoxWrapper.Show($"File downloaded to: {path}", "Download Complete");
          break;
        case FileOpenState.OpenFailed:
          MessageBoxWrapper.Show($"Failed to open the Grasshopper file: {name}", errorTitle);
          break;
        case FileOpenState.DownloadFailed:
          MessageBoxWrapper.Show($"Download of the file: {name}, failed.", errorTitle);
          break;
        case FileOpenState.NoFilesFound:
          MessageBoxWrapper.Show("Couldn't find any sample files. Please contact with support.", errorTitle);
          break;
        case FileOpenState.OverrideQuestion:
          return MessageBoxWrapper.Show($"File \"{name}\" already exists in {path}. Overwrite?", "File Exists",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
      }

      return DialogResult.Abort;
    }
  }
}
