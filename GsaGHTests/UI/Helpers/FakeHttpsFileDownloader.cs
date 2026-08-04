using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using GsaGH.UI;

namespace GsaGHTests.TestHelpers {
  /// <summary>
  /// Lightweight fake for IHttpsFileDownloader used in unit tests.
  /// Can simulate successful listing/download and failures.
  /// </summary>
  public class FakeHttpsFileDownloader : IHttpsFileDownloader {
    private readonly List<FileEntry> _files;
    private readonly bool _throwOnGet;
    private readonly bool _throwOnDownload;
    private readonly string _downloadsPath;

    public FakeHttpsFileDownloader(
      List<FileEntry> files = null,
      bool throwOnGet = false,
      bool throwOnDownload = false,
      string customDownloadPath = "") {
      _files = files ?? new List<FileEntry>();
      _throwOnGet = throwOnGet;
      _throwOnDownload = throwOnDownload;
      _downloadsPath = string.IsNullOrWhiteSpace(customDownloadPath) ? Path.GetTempPath() : customDownloadPath;
      UrlToSamples = "https://example.test/samples";
    }

    public string UrlToSamples { get; }

    public Task<List<FileEntry>> GetFilesFromWebPageAsync() {
      if (_throwOnGet) {
        throw new Exception("Simulated network error");
      }

      // return copy to avoid external mutation
      return Task.FromResult(new List<FileEntry>(_files));
    }

    public Task DownloadFileAsync(FileEntry file) {
      if (_throwOnDownload) {
        throw new HttpRequestException("Simulated download failure");
      }

      if (file == null) {
        throw new ArgumentNullException(nameof(file));
      }

      string path = GetFullDownloadPath(file);
      Directory.CreateDirectory(Path.GetDirectoryName(path) ?? _downloadsPath);

      // create an empty file to simulate a downloaded file
      using (File.Create(path)) { }

      return Task.CompletedTask;
    }

    public string GetFullDownloadPath(FileEntry file) {
      if (file == null) {
        throw new ArgumentNullException(nameof(file));
      }

      string safe = MakeSafeFileName(file.Name);
      return Path.Combine(_downloadsPath, safe);
    }

    public Task SaveFileAsync(HttpResponseMessage response, FileEntry file) {
      throw new NotImplementedException();
    }

    public string SetCustomDownloadsPath(string customDownloadPath) {
      return string.IsNullOrWhiteSpace(customDownloadPath) ? _downloadsPath : customDownloadPath;
    }

    private static string MakeSafeFileName(string name) {
      if (string.IsNullOrEmpty(name)) {
        return "file";
      }

      char[] invalid = Path.GetInvalidFileNameChars();

      return invalid.Aggregate(name, (current, c) => current.Replace(c, '_'));
    }
  }
}
