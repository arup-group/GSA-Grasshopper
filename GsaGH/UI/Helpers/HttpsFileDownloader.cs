using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using GsaGH.Properties;
using GsaGH.UI.Helpers;

using HtmlAgilityPack;

namespace GsaGH.UI {

  public interface IHttpsFileDownloader {
    public Task DownloadFileAsync(FileEntry file);
    public Task<List<FileEntry>> GetFilesFromWebPageAsync();
    public string GetFullDownloadPath(FileEntry file);
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

    public static string DownloadsPath
      => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

    public HttpsFileDownloader(IHttpClientWrapper httpClientWrapper) {
      _httpClientWrapper = httpClientWrapper;
    }

    public string GetFullDownloadPath(FileEntry file) {
      return Path.Combine(DownloadsPath, file.Name);
    }

    public async Task DownloadFileAsync(FileEntry file) {
      string fileUrl = file.Url;
      string filePath = GetFullDownloadPath(file);

      HttpResponseMessage response = await _httpClientWrapper.GetAsync(fileUrl);
      response.EnsureSuccessStatusCode();

      using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
      await response.Content.CopyToAsync(fs);
    }

    public async Task<List<FileEntry>> GetFilesFromWebPageAsync() {
      string examplesUrl = Resources.SamplesUrl;
      string html = await _httpClientWrapper.GetStringAsync(examplesUrl);
      var doc = new HtmlDocument();
      doc.LoadHtml(html);

      HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a[@href]");
      return nodes == null ? new List<FileEntry>() : GetFileListFromNodes(examplesUrl, nodes);
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
