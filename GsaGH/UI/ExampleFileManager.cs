using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using Grasshopper;
using Grasshopper.Kernel;

using static GsaGH.UI.MessageDialogBox;

namespace GsaGH.UI {
  public interface IExampleFileManager {
    public Task<List<FileEntry>> GetExampleFilesAsync();
    public bool CheckForDuplicatedDownloads(FileEntry file);
    public Task<DialogResult> DownloadAndOpenFileAsync(FileEntry file);
  }

  public class ExampleFileManager : IExampleFileManager {
    private IHttpsFileDownloader _downloader;

    public ExampleFileManager(IHttpsFileDownloader downloader) {
      _downloader = downloader;
    }

    /// <summary>
    ///   Fetches the list of example files asynchronously and returns them.
    /// </summary>
    public async Task<List<FileEntry>> GetExampleFilesAsync() {
      try {
        return await _downloader.GetFilesFromWebPageAsync();
      } catch (Exception ex) {
        ShowMessage(FileOpenState.NoFilesFound, string.Empty);
        return new List<FileEntry>();
      }
    }

    public bool CheckForDuplicatedDownloads(FileEntry file) {
      string savePath = _downloader.GetFullDownloadPath(file);
      if (!File.Exists(savePath)) {
        return true;
      }

      // if file exists ask for overwrite
      DialogResult result = ShowMessage(FileOpenState.OverrideQuestion, file.Name);

      return result == DialogResult.OK;
    }

    public async Task<DialogResult> DownloadAndOpenFileAsync(FileEntry file) {
      if (CheckForDuplicatedDownloads(file)) {
        try {
          await _downloader.DownloadFileAsync(file);

          string savePath = _downloader.GetFullDownloadPath(file);
          if (Path.GetExtension(savePath).Equals(".gh", StringComparison.OrdinalIgnoreCase)) {
            var io = new GH_DocumentIO();
            if (io.Open(savePath)) {
              Instances.DocumentEditor.Invoke((Action)(() => Instances.ActiveCanvas.Document = io.Document));
              return ShowMessage(FileOpenState.Success, file.Name);
            }

            return ShowMessage(FileOpenState.OpenFailed, file.Name);
          }

          return ShowMessage(FileOpenState.Downloaded, file.Name);
        } catch {
          return ShowMessage(FileOpenState.DownloadFailed, file.Name);
        }
      }

      return ShowMessage(FileOpenState.Cancelled, file.Name);
    }
  }
}
