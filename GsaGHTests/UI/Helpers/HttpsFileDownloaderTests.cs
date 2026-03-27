using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using GsaGH.UI;
using GsaGH.UI.Helpers;

using Moq;

using Xunit;

namespace GsaGHTests.UI {
  public class HttpsFileDownloaderTests {
    private readonly Mock<IHttpClientWrapper> _mockHttpClient;

    public HttpsFileDownloaderTests() {
      _mockHttpClient = new Mock<IHttpClientWrapper>();
    }

    [Fact]
    public void GetFullDownloadPath_ShouldReturnCorrectPath() {
      var file = new FileEntry {
        Name = "testfile.gh",
      };
      string expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads",
        "testfile.gh");

      var downloader = new HttpsFileDownloader(_mockHttpClient.Object, string.Empty);
      string fullPath = downloader.GetFullDownloadPath(file);

      Assert.Equal(expectedPath, fullPath);
    }

    [Fact]
    public void GetFullDownloadPath_ShouldReturnCorrectPath_WhenFileNameWithPath() {
      var file = new FileEntry {
        Name = "testfile/gh",
      };
      var downloader = new HttpsFileDownloader(_mockHttpClient.Object, string.Empty);

      string expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads",
        "testfile/gh");

      string fullPath = downloader.GetFullDownloadPath(file);

      Assert.Equal(expectedPath, fullPath);
    }

    [Fact]
    public async Task DownloadFileAsync_ShouldDownloadFileCorrectly() {
      var mockHttpClientWrapper = new Mock<IHttpClientWrapper>();

      var mockHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent("sample file content"),
      };
      mockHttpClientWrapper.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(mockHttpResponseMessage);

      var downloader = new HttpsFileDownloader(mockHttpClientWrapper.Object, "https://example.com");

      var file = new FileEntry {
        Name = "testfile.gh",
        Url = "http://example.com/testfile.gh",
      };

      await downloader.DownloadFileAsync(file);

      mockHttpClientWrapper.Verify(c => c.GetAsync(file.Url), Times.Once);
    }

    [Fact]
    public async Task DownloadFileAsync_ShouldThrowException_WhenHttpResponseIsNotSuccess() {
      var mockHttpClientWrapper = new Mock<IHttpClientWrapper>();

      var mockHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
      mockHttpClientWrapper.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(mockHttpResponseMessage);

      var downloader = new HttpsFileDownloader(mockHttpClientWrapper.Object, "https://example.com");

      var file = new FileEntry {
        Name = "testfile.gh",
        Url = "http://example.com/testfile.gh",
      };

      await Assert.ThrowsAsync<HttpRequestException>(() => downloader.DownloadFileAsync(file));

      mockHttpClientWrapper.Verify(c => c.GetAsync(file.Url), Times.Once);
    }

    [Fact]
    public async Task DownloadFileAsync_ShouldThrowException_WhenHttpRequestExceptionOccurs() {
      var mockHttpClientWrapper = new Mock<IHttpClientWrapper>();

      mockHttpClientWrapper.Setup(c => c.GetAsync(It.IsAny<string>()))
       .ThrowsAsync(new HttpRequestException("Request failed"));

      var downloader = new HttpsFileDownloader(mockHttpClientWrapper.Object, "https://example.com");

      var file = new FileEntry {
        Name = "testfile.gh",
        Url = "http://example.com/testfile.gh",
      };

      await Assert.ThrowsAsync<HttpRequestException>(() => downloader.DownloadFileAsync(file));

      mockHttpClientWrapper.Verify(c => c.GetAsync(file.Url), Times.Once);
    }

    [Fact]
    public async Task SaveFileAsync_ShouldSaveFileOnDisk_WhenValidResponse() {
      var mockHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent("sample file content"),
      };

      var downloader = new HttpsFileDownloader(Mock.Of<IHttpClientWrapper>(), "http://example.com/samples");

      var file = new FileEntry {
        Name = "testfile.gh",
        Url = "http://example.com/testfile.gh",
      };

      string filePath = downloader.GetFullDownloadPath(file);

      await downloader.SaveFileAsync(mockHttpResponseMessage, file);

      Assert.True(File.Exists(filePath));

      string content = File.ReadAllText(filePath);
      Assert.Equal("sample file content", content);

      File.Delete(filePath);
    }

    //[Fact]
    //public async Task GetFilesFromWebPageAsync_ShouldParseHtmlAndReturnFileList() {
    //  string htmlContent
    //    = "<html><body><a href='/file1.gh'>file1.gh</a><a href='/file2.3dm'>file2.3dm</a></body></html>";

    //  _mockHttpClient.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync(htmlContent);

    //  var downloader = new HttpsFileDownloader(_mockHttpClient.Object);
    //  List<FileEntry> files = await downloader.GetFilesFromWebPageAsync();

    //  Assert.Equal(2, files.Count);
    //  Assert.Equal("file1.gh", files[0].Name);
    //  Assert.Equal("file2.3dm", files[1].Name);
    //}

    //[Fact]
    //public async Task GetFilesFromWebPageAsync_ShouldReturnEmptyList_WhenNoFilesFound() {
    //  string htmlContent
    //    = "<html><body><a href='/file1.txt'>file1.txt</a><a href='/file2.txt'>file2.txt</a></body></html>";

    //  _mockHttpClient.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync(htmlContent);

    //  var downloader = new HttpsFileDownloader(_mockHttpClient.Object);
    //  List<FileEntry> files = await downloader.GetFilesFromWebPageAsync();

    //  Assert.Empty(files);
    //}

    //[Fact]
    //public async Task GetFilesFromWebPageAsync_ShouldHandleEmptyHtmlGracefully() {
    //  string htmlContent = "<html><body></body></html>";

    //  _mockHttpClient.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync(htmlContent);

    //  var downloader = new HttpsFileDownloader(_mockHttpClient.Object);
    //  List<FileEntry> files = await downloader.GetFilesFromWebPageAsync();

    //  Assert.Empty(files);
    //}

    //[Fact]
    //public void GetFileListFromNodes_ShouldReturnValidFileList() {
    //  string baseUrl = "http://example.com";
    //  var nodes = new HtmlNodeCollection(null) {
    //    HtmlNode.CreateNode("<a href='/file1.gh'>file1.gh</a>"),
    //    HtmlNode.CreateNode("<a href='/file2.3dm'>file2.3dm</a>"),
    //  };

    //  var downloader = new HttpsFileDownloader(_mockHttpClient.Object);
    //  var fileList
    //    = downloader?.GetType()?.GetMethod("GetFileListFromNodes", BindingFlags.NonPublic | BindingFlags.Instance)
    //    ?.Invoke(downloader, new object[] {
    //        baseUrl,
    //        nodes,
    //      }) as List<FileEntry>;

    //  Assert.NotNull(fileList);
    //  Assert.Equal(2, fileList.Count);
    //  Assert.Equal("file1.gh", fileList[0].Name);
    //  Assert.Equal("file2.3dm", fileList[1].Name);
    //  Assert.Equal("http://example.com/file1.gh", fileList[0].Url);
    //  Assert.Equal("http://example.com/file2.3dm", fileList[1].Url);
    //}
  }
}
