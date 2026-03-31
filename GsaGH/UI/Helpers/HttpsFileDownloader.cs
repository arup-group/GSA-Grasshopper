using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using GsaGH.UI.Helpers;

using HtmlAgilityPack;

namespace GsaGH.UI {

  public interface IHttpsFileDownloader {
    public Task DownloadFileAsync(FileEntry file);
    public Task<List<FileEntry>> GetFilesFromWebPageAsync();
    public string GetFullDownloadPath(FileEntry file);
    public Task SaveFileAsync(HttpResponseMessage response, FileEntry file);
    public string UrlToSamples { get; }
  }

  public class FileEntry {
    public string Name { get; set; }
    public string Url { get; set; }
  }

  /// <summary>
  ///   A class responsible for downloading files via HTTPS and parsing a webpage to retrieve a list of downloadable files.
  ///   in the future, may be good to separate it into two classes. one for downloading, one for getting a list of
  ///   files
  /// </summary>
  public class HttpsFileDownloader : IHttpsFileDownloader {
    private static readonly List<string> AllowedExtensions = new List<string>() {
      ".gh",
      ".gwa",
      ".gwb",
      ".3dm",
    };

    private readonly IHttpClientWrapper _httpClientWrapper;

    public static string DefaultDownloadPath
      => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
    private readonly string _downloadsPath;

    public string UrlToSamples { get; }

    public HttpsFileDownloader(
      IHttpClientWrapper httpClientWrapper, string urlToSamples, string customDownloadPath = "") {
      _httpClientWrapper = httpClientWrapper;
      UrlToSamples = urlToSamples;
      _downloadsPath = SetCustomDownloadsPath(customDownloadPath);
    }

    private static string SetCustomDownloadsPath(string customDownloadPath) {
      if (string.IsNullOrWhiteSpace(customDownloadPath)) {
        return DefaultDownloadPath;
      }

      if (Path.IsPathRooted(customDownloadPath) && Directory.Exists(customDownloadPath)) {
        return customDownloadPath;
      }

      MessageDialogBox.ShowMessage(MessageDialogBox.FileOpenState.InvalidDownloadPath, string.Empty,
        customDownloadPath);
      return DefaultDownloadPath;
    }

    public string GetFullDownloadPath(FileEntry file) {
      if (file == null) {
        throw new ArgumentNullException(nameof(file));
      }

      string safeFileName = HttpNameExtractor.GetSafeFileName(file.Name);
      return Path.Combine(_downloadsPath, safeFileName);
    }

    public async Task DownloadFileAsync(FileEntry file) {
      if (file == null) {
        throw new ArgumentNullException(nameof(file));
      }

      string fileUrl = file.Url;

      try {
        using HttpResponseMessage response = await _httpClientWrapper.GetAsync(fileUrl);

        if (!response.IsSuccessStatusCode) {
          throw new HttpRequestException($"Failed to download file from {fileUrl}. Status Code: {response.StatusCode}");
        }

        await SaveFileAsync(response, file);
      } catch (HttpRequestException ex) {
        Console.WriteLine($"HTTP error occurred while downloading {file.Url}: {ex.Message}");
        throw;
      } catch (Exception ex) {
        Console.WriteLine($"An error occurred while downloading or saving the file: {ex.Message}");
        throw;
      }
    }

    public async Task SaveFileAsync(HttpResponseMessage response, FileEntry file) {
      if (response?.Content == null) {
        throw new ArgumentNullException(nameof(response));
      }

      string filePath = GetFullDownloadPath(file);

      using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
      await response.Content.CopyToAsync(fs)!;
    }

    public async Task<List<FileEntry>> GetFilesFromWebPageAsync() {
      string html = await _httpClientWrapper.GetStringAsync(UrlToSamples);
      var doc = new HtmlDocument();
      doc.LoadHtml(html);

      HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a[@href]");
      return nodes == null ? new List<FileEntry>() : GetFileListFromNodes(UrlToSamples, nodes);
    }

    private static List<FileEntry> GetFileListFromNodes(string url, HtmlNodeCollection nodes) {
      var fileList = new List<FileEntry>();

      foreach (HtmlNode link in nodes) {
        string href = link.GetAttributeValue("href", "");
        if (AllowedExtensions.Any(ext => href.EndsWith(ext, StringComparison.OrdinalIgnoreCase))) {
          fileList.Add(new FileEntry {
            Name = link.InnerText.Trim(),
            Url = href.StartsWith("http") ? href : new Uri(new Uri(url), href).ToString(),
          });
        }
      }

      return fileList;
    }
  }
}
