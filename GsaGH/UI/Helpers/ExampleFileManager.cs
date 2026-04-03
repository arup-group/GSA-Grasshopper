using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using static GsaGH.UI.MessageDialogBox;

namespace GsaGH.UI {
  public interface IExampleFileManager {
    Task<List<FileEntry>> GetExampleFilesAsync();
    bool IsOverwriteApproved(FileEntry file);
    Task<DialogResult> DownloadAndOpenFileAsync(FileEntry file, Func<string, bool> openGhFileFunc);
  }

  public class ExampleFileManager : IExampleFileManager {
    private IHttpsFileDownloader Downloader { get; }

    public ExampleFileManager(IHttpsFileDownloader downloader) {
      Downloader = downloader;
    }

    /// <summary>
    ///   Fetches the list of example files asynchronously and returns them.
    /// </summary>
    public async Task<List<FileEntry>> GetExampleFilesAsync() {
      try {
        return await Downloader.GetFilesFromWebPageAsync();
      } catch (Exception) {
        ShowMessage(FileState.NoFilesFound, string.Empty);
        return new List<FileEntry>();
      }
    }

    public bool IsOverwriteApproved(FileEntry file) {
      string savePath = Downloader.GetFullDownloadPath(file);
      if (!File.Exists(savePath)) {
        return true;
      }

      // if file exists ask for overwrite
      DialogResult result = ShowMessage(FileState.OverrideQuestion, file.Name, savePath);

      return result == DialogResult.OK;
    }

    public async Task<DialogResult> DownloadAndOpenFileAsync(FileEntry file, Func<string, bool> openGhFileFunc) {
      if (!IsOverwriteApproved(file)) {
        return ShowMessage(FileState.Cancelled, file.Name);
      }

      try {
        await Downloader.DownloadFileAsync(file);

        string savePath = Downloader.GetFullDownloadPath(file);
        if (!Path.GetExtension(savePath).Equals(".gh", StringComparison.OrdinalIgnoreCase)) {
          return ShowMessage(FileState.Downloaded, file.Name, savePath);
        }

        return openGhFileFunc switch {
          null => throw new ArgumentNullException(nameof(openGhFileFunc),
            "Function to open .gh files must be provided."),
          _ => openGhFileFunc(savePath) ? ShowMessage(FileState.Success, file.Name) :
            ShowMessage(FileState.OpenFailed, file.Name),
        };
      } catch (ArgumentNullException) {
        throw;
      } catch {
        return ShowMessage(FileState.DownloadFailed, file.Name);
      }
    }
  }
}
