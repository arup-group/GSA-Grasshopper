using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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
  ///   TODO: in the future, may be good to separate it into two classes. one for downloading, one for getting a list of
  ///   files
  /// </summary>
  public class HttpsFileDownloader : IHttpsFileDownloader {
    private const string ExamplesUrl = "https://samples.oasys-software.com/gsa/10.2/Gsa_GH/";
    private static readonly List<string> AllowedExtensions = new List<string>() {
      ".gh",
      ".gwa",
      ".gwb",
      ".3dm",
    };

    public static string DownloadsPath
      => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

    public string GetFullDownloadPath(FileEntry file) {
      return Path.Combine(DownloadsPath, file.Name);
    }

    /// <summary>
    ///   Get and download a specific file
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public async Task DownloadFileAsync(FileEntry file) {
      using var client = new HttpClient();
      using HttpResponseMessage response = await client.GetAsync(file.Url);
      response.EnsureSuccessStatusCode();
      using var fs = new FileStream(GetFullDownloadPath(file), FileMode.Create, FileAccess.Write);
      await response.Content.CopyToAsync(fs);
    }

    /// <summary>
    ///   Gets the list of example files from the web page, combined with links to them
    /// </summary>
    public async Task<List<FileEntry>> GetFilesFromWebPageAsync() {
      using var client = new HttpClient();
      string html = await client.GetStringAsync(ExamplesUrl);
      var doc = new HtmlDocument();
      doc.LoadHtml(html);

      HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a[@href]");
      return nodes == null ? new List<FileEntry>() : GetFileListFromNodes(ExamplesUrl, nodes);
    }

    private static List<FileEntry> GetFileListFromNodes(string url, HtmlNodeCollection nodes) {
      var fileList = new List<FileEntry>();
      //if possible, use json from the serveer, or nginx directory - if not then this solution is the easiest and most popular
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
