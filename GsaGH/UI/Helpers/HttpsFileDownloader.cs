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
    Task DownloadFileAsync(FileEntry file);
    Task<List<FileEntry>> GetFilesFromWebPageAsync();
    string GetFullDownloadPath(FileEntry file);
    Task SaveFileAsync(HttpResponseMessage response, FileEntry file);
    string UrlToSamples { get; }
    string SetCustomDownloadsPath(string customDownloadPath);
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

    public static string DefaultDownloadPath {
      get {
        // ensure the default download path exists before returning it - some systems may not have a downloads folder by default, and this will prevent errors when trying to save files
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        Directory.CreateDirectory(path);

        return path;
      }
    }
    private readonly string _downloadsPath;

    public string UrlToSamples { get; }

    public HttpsFileDownloader(
      IHttpClientWrapper httpClientWrapper, string urlToSamples, string customDownloadPath = "") {
      _httpClientWrapper = httpClientWrapper;
      UrlToSamples = urlToSamples;
      _downloadsPath = SetCustomDownloadsPath(customDownloadPath);
    }

    public string SetCustomDownloadsPath(string customDownloadPath) {
      if (string.IsNullOrWhiteSpace(customDownloadPath)) {
        return DefaultDownloadPath;
      }

#pragma warning disable IDE0046 // Convert to conditional expression
      if (Path.IsPathRooted(customDownloadPath) && Directory.Exists(customDownloadPath)) {
        return customDownloadPath;
      }
#pragma warning restore IDE0046 // Convert to conditional expression

      throw new ArgumentException("Invalid download path", nameof(customDownloadPath));
    }

    public string GetFullDownloadPath(FileEntry file) {
      if (file == null) {
        throw new ArgumentNullException(nameof(file));
      }

      string safeFileName = HttpFileHelper.GetSafeFileName(file.Name);
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
        throw new HttpRequestException($"HTTP error occurred while downloading {file.Url}: {ex.Message}");
      }
    }

    public async Task SaveFileAsync(HttpResponseMessage response, FileEntry file) {
      if (response == null) {
        throw new ArgumentNullException(nameof(response));
      }

      if (response.Content == null) {
#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one 
        throw new ArgumentNullException(nameof(response.Content));
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one 
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
      if (nodes == null) {
        return new List<FileEntry>();
      }

      return nodes.Select(link => HttpFileHelper.GetFileEntry(url, link, AllowedExtensions))
       .Where(entryFile => entryFile != null).ToList();
    }
  }
}
