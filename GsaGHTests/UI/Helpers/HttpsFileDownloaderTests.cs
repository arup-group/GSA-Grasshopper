using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using GsaGH.UI;

using Moq;

using Xunit;

namespace GsaGHTests.UI {
  public class HttpsFileDownloaderTests {
    [Fact]
    public void GetFullDownloadPath_ShouldReturnCorrectPath() {
      var file = new FileEntry {
        Name = "testfile.gh",
      };
      string expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads",
        "testfile.gh");

      var downloader = new HttpsFileDownloader();
      string fullPath = downloader.GetFullDownloadPath(file);

      Assert.Equal(expectedPath, fullPath);
    }

    [Fact]
    public async Task DownloadFileAsync_ShouldDownloadFileCorrectly() {
      var mockHttpClient = new Mock<HttpClient>();
      var mockHttpResponseMessage = new Mock<HttpResponseMessage>();
      mockHttpResponseMessage.Setup(r => r.IsSuccessStatusCode).Returns(true);
      mockHttpClient.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(mockHttpResponseMessage.Object);

      var file = new FileEntry {
        Name = "testfile.gh",
        Url = "http://example.com/testfile.gh",
      };
      var downloader = new HttpsFileDownloader();
      await downloader.DownloadFileAsync(file);

      mockHttpClient.Verify(c => c.GetAsync(file.Url), Times.Once);
    }

    [Fact]
    public async Task GetFilesFromWebPageAsync_ShouldParseHtmlAndReturnFileList() {
      string htmlContent
        = "<html><body><a href='/file1.gh'>file1.gh</a><a href='/file2.3dm'>file2.3dm</a></body></html>";

      var mockHttpClient = new Mock<HttpClient>();
      mockHttpClient.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync(htmlContent);

      var downloader = new HttpsFileDownloader();
      List<FileEntry> files = await downloader.GetFilesFromWebPageAsync();

      Assert.Equal(2, files.Count);
      Assert.Equal("file1.gh", files[0].Name);
      Assert.Equal("file2.3dm", files[1].Name);
    }

    [Fact]
    public async Task GetFilesFromWebPageAsync_ShouldReturnEmptyList_WhenNoFilesFound() {
      string htmlContent
        = "<html><body><a href='/file1.txt'>file1.txt</a><a href='/file2.txt'>file2.txt</a></body></html>";

      var mockHttpClient = new Mock<HttpClient>();
      mockHttpClient.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync(htmlContent);

      var downloader = new HttpsFileDownloader();
      List<FileEntry> files = await downloader.GetFilesFromWebPageAsync();

      Assert.Empty(files);
    }

    [Fact]
    public void GetFullDownloadPath_ShouldReturnCorrectPath_WhenFileNameWithPath() {
      var file = new FileEntry {
        Name = "testfile/gh",
      };
      var downloader = new HttpsFileDownloader();

      string expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads",
        "testfile/gh");

      string fullPath = downloader.GetFullDownloadPath(file);

      Assert.Equal(expectedPath, fullPath);
    }

    [Fact]
    public void GetFilesFromWebPageAsync_ShouldHandleEmptyHtmlGracefully() {
      string htmlContent = "<html><body></body></html>";

      var mockHttpClient = new Mock<HttpClient>();
      mockHttpClient.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync(htmlContent);

      var downloader = new HttpsFileDownloader();
      List<FileEntry> files = downloader.GetFilesFromWebPageAsync().Result;

      Assert.Empty(files);
    }
  }
}
