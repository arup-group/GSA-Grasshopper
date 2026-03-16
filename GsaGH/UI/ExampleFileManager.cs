using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Grasshopper;
using Grasshopper.Kernel;

using static GsaGH.UI.MessageDialogBox;

namespace GsaGH.UI {
  public interface IExampleFileManager {
    public Task<List<FileEntry>> GetExampleFilesAsync();
    public DialogResult CheckForDuplicatedDownloads(FileEntry file);
    public Task<DialogResult> DownloadAndOpenFileAsync(FileEntry file);
  }

  public class ExampleFileManager : IExampleFileManager {
    private static IHttpsFileDownloader _downloader;

    public ExampleFileManager(IHttpsFileDownloader downloader) {
      _downloader = downloader;
    }

    /// <summary>
    ///   Fetches the list of example files asynchronously and returns them.
    /// </summary>
    public async Task<List<FileEntry>> GetExampleFilesAsync() {
      List<FileEntry> files = null;
      try {
        files = await _downloader.GetFilesFromWebPageAsync();
      } catch (Exception ex) {
        files ??= new List<FileEntry>();
        ShowMessage(FileOpenState.NoSampleFilesFound, string.Empty);
      } finally {
        ExampleFileRepository.SetFiles(files); // we need to fill this filerepo with items
      }

      return files;
    }

    public DialogResult CheckForDuplicatedDownloads(FileEntry file) {
      string savePath = _downloader.GetFullDownloadPath(file);
      if (!File.Exists(savePath)) {
        return DialogResult.Yes; //if file doesn't exist we want to download it
      }

      // if file exists ask for overwrite
      DialogResult result = ShowMessage(FileOpenState.OverrideQuestion, file.Name);

      return result;
    }

    public async Task<DialogResult> DownloadAndOpenFileAsync(FileEntry file) {
      string savePath = _downloader.GetFullDownloadPath(file);
      DialogResult overrideResult = CheckForDuplicatedDownloads(file);

      switch (overrideResult) {
        case DialogResult.Yes: {
          try {
            await _downloader.DownloadFileAsync(file);
            return TryOpenFile(file, savePath, out DialogResult downloadAndOpenFileAsync) ? downloadAndOpenFileAsync :
              ShowMessage(FileOpenState.Downloaded, file.Name);
          } catch {
            return ShowMessage(FileOpenState.DownloadFailed, file.Name);
          }
        }
        case DialogResult.No when TryOpenFile(file, savePath, out DialogResult downloadAndOpenFileAsync):
          return downloadAndOpenFileAsync;
      }

      return ShowMessage(FileOpenState.Cancelled, file.Name);
    }

    private static bool TryOpenFile(FileEntry file, string path, out DialogResult downloadAndOpenFileAsync) {
      if (Path.GetExtension(path).Equals(".gh", StringComparison.OrdinalIgnoreCase)) {
        var io = new GH_DocumentIO();
        if (io.Open(path)) {
          Instances.DocumentEditor.Invoke((Action)(() => Instances.ActiveCanvas.Document = io.Document));
          downloadAndOpenFileAsync = ShowMessage(FileOpenState.Success, file.Name);
          return true;
        }

        downloadAndOpenFileAsync = ShowMessage(FileOpenState.OpenFailed, file.Name);
        return false;
      }

      downloadAndOpenFileAsync = ShowMessage(FileOpenState.FileNoExist, file.Name);
      return false;
    }
  }

  public class ExampleFileRepository {
    public static bool _isInitialized = false;
    private static List<FileEntry> _files = new List<FileEntry>();
    private ExampleFileRepository() { }

    public static void SetFiles(List<FileEntry> files) {
      if (_isInitialized) {
        return;
      }

      _files = files ?? new List<FileEntry>();
      _isInitialized = true;
    }

    public static List<FileEntry> GetAllFiles() {
      return _files;
    }

    public static IEnumerable<FileEntry> GetFileEntriesByKeywords(List<string> keywords) {
      return keywords == null || keywords.Count == 0 ? new List<FileEntry>() : _files.Where(item
        => keywords.Any(keyword => item.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0));
    }
  }
}
