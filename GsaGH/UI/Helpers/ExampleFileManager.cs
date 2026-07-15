using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using GsaGH.UI.Helpers;

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
      if (downloader == null) {
        throw new ArgumentNullException(nameof(downloader));
      }

      Downloader = downloader;
    }

    /// <summary>
    ///   Fetches the list of example files asynchronously and returns them.
    /// </summary>
    public async Task<List<FileEntry>> GetExampleFilesAsync() {
      List<FileEntry> files;
      try {
        files = await Downloader.GetFilesFromWebPageAsync();
      } catch (Exception) {
        ShowMessage(FileState.NoFilesFound, string.Empty);
        files = new List<FileEntry>();
      }

      ExampleFileRepository.SetFiles(files);
      return files;
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
      if (file == null) {
        throw new ArgumentNullException(nameof(file));
      }

      if (openGhFileFunc == null) {
        throw new ArgumentNullException(nameof(openGhFileFunc));
      }

      if (!IsOverwriteApproved(file)) {
        return ShowMessage(FileState.Cancelled, file.Name);
      }

      try {
        await Downloader.DownloadFileAsync(file);

        string savePath = Downloader.GetFullDownloadPath(file);
        if (!Path.GetExtension(savePath).Equals(".gh", StringComparison.OrdinalIgnoreCase)) {
          return ShowMessage(FileState.Downloaded, file.Name, savePath);
        }

        return openGhFileFunc(savePath) ? ShowMessage(FileState.Success, file.Name) :
          ShowMessage(FileState.OpenFailed, file.Name);
      } catch {
        return ShowMessage(FileState.DownloadFailed, file.Name);
      }
    }
  }
}
