using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using static GsaGH.UI.Helpers.MessageDialogBox;

namespace GsaGH.UI.SampleFiles {
  public interface IExampleFileManager {
    Task<List<FileEntry>> GetExampleFilesAsync();
    bool IsOverwriteApproved(FileEntry file);
    Task<DialogResult> DownloadAndOpenFileAsync(FileEntry file);
  }

  public class ExampleFileManager : IExampleFileManager {
    private IHttpsFileDownloader Downloader { get; }
    private Func<string, bool> OpenGhFile { get; }

    public ExampleFileManager(IHttpsFileDownloader downloader, Func<string, bool> openGhFileFunc) {
      Downloader = downloader ?? throw new ArgumentNullException(nameof(downloader));
      OpenGhFile = openGhFileFunc ?? throw new ArgumentNullException(nameof(openGhFileFunc));
    }

    /// <summary>
    ///   Fetches the list of example files asynchronously and returns them.
    ///   Initializes ExampleFileRepository so other classes can access the file list.
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

      DialogResult result = ShowMessage(FileState.OverrideQuestion, file.Name, savePath);
      return result == DialogResult.OK;
    }

    public async Task<DialogResult> DownloadAndOpenFileAsync(FileEntry file) {
      if (!IsOverwriteApproved(file)) {
        return ShowMessage(FileState.Cancelled, file.Name);
      }

      try {
        await Downloader.DownloadFileAsync(file);

        string savePath = Downloader.GetFullDownloadPath(file);
        if (!Path.GetExtension(savePath).Equals(".gh", StringComparison.OrdinalIgnoreCase)) {
          return ShowMessage(FileState.Downloaded, file.Name, savePath);
        }

        return OpenGhFile(savePath) ? ShowMessage(FileState.Success, file.Name) :
          ShowMessage(FileState.OpenFailed, file.Name);
      } catch {
        return ShowMessage(FileState.DownloadFailed, file.Name);
      }
    }
  }
}
