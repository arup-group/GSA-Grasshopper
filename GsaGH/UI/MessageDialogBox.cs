using System.Windows.Forms;

namespace GsaGH.UI {
  public static class MessageDialogBox {
    public enum FileOpenState {
      Success,
      Downloaded,
      OpenFailed,
      DownloadFailed,
      NoFilesFound,
      Cancelled,
      OverrideQuestion,
    }

    public static DialogResult ShowMessage(FileOpenState state, string name) {
      switch (state) {
        case FileOpenState.Success: break;
        case FileOpenState.Downloaded:

          MessageBox.Show($"File downloaded to: {HttpsFileDownloader.DownloadsPath}", "Download Complete");
          break;
        case FileOpenState.OpenFailed:
          MessageBox.Show($"Failed to open the Grasshopper file: {name}", "Error");
          break;
        case FileOpenState.DownloadFailed:
          MessageBox.Show($"Download of the file: {name}, failed.", "Error");
          break;
        case FileOpenState.NoFilesFound:
          MessageBox.Show("Couldn't find any files in sample site. Please contact with support.");
          break;
        case FileOpenState.OverrideQuestion:
          return MessageBox.Show($"File \"{name}\" already exists in Downloads. Overwrite?", "File Exists",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
      }

      return DialogResult.OK;
    }
  }
}
