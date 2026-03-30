using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using GsaGH.UI;
using GsaGH.UI.Helpers;

using HtmlAgilityPack;

using Moq;

using Xunit;

namespace GsaGHTests.UI {
  public class HttpsFileDownloaderTests {
    private readonly Mock<IHttpClientWrapper> _mockHttpClient;
    private const string _httpExampleComSamples = "http://example.com/samples";
    private readonly HttpsFileDownloader _downloader;

    public HttpsFileDownloaderTests() {
      _mockHttpClient = new Mock<IHttpClientWrapper>();
      _downloader = new HttpsFileDownloader(_mockHttpClient.Object, _httpExampleComSamples);
    }

    [Fact]
    public void GetFullDownloadPath_ShouldReturnCorrectPath() {
      FileEntry file = GetSampleFileEntry();
      string expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads",
        "testfile.gh");

      string fullPath = _downloader.GetFullDownloadPath(file);

      Assert.Equal(expectedPath, fullPath);
    }

    [Fact]
    public void GetFullDownloadPath_ShouldReturnCorrectPath_WhenFileNameWithPath() {
      var file = new FileEntry {
        Name = "testfile/gh",
      };

      string expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads",
        "testfile/gh");

      string fullPath = _downloader.GetFullDownloadPath(file);

      Assert.Equal(expectedPath, fullPath);
    }

    [Fact]
    public async Task DownloadFileAsync_ShouldDownloadFileCorrectly() {
      var mockHttpClientWrapper = new Mock<IHttpClientWrapper>();

      var mockHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent("sample file content"),
      };
      mockHttpClientWrapper.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(mockHttpResponseMessage);

      var downloader = new HttpsFileDownloader(mockHttpClientWrapper.Object, _httpExampleComSamples);

      FileEntry file = GetSampleFileEntry();

      await downloader.DownloadFileAsync(file);

      mockHttpClientWrapper.Verify(c => c.GetAsync(file.Url), Times.Once);
    }

    [Fact]
    public async Task DownloadFileAsync_ShouldThrowException_WhenHttpResponseIsNotSuccess() {
      var mockHttpClientWrapper = new Mock<IHttpClientWrapper>();

      var mockHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
      mockHttpClientWrapper.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(mockHttpResponseMessage);

      var downloader = new HttpsFileDownloader(mockHttpClientWrapper.Object, _httpExampleComSamples);

      FileEntry file = GetSampleFileEntry();

      await Assert.ThrowsAsync<HttpRequestException>(() => downloader.DownloadFileAsync(file));

      mockHttpClientWrapper.Verify(c => c.GetAsync(file.Url), Times.Once);
    }

    [Fact]
    public async Task DownloadFileAsync_ShouldThrowException_WhenHttpRequestExceptionOccurs() {
      var mockHttpClientWrapper = new Mock<IHttpClientWrapper>();

      mockHttpClientWrapper.Setup(c => c.GetAsync(It.IsAny<string>()))
       .ThrowsAsync(new HttpRequestException("Request failed"));

      var downloader = new HttpsFileDownloader(mockHttpClientWrapper.Object, _httpExampleComSamples);

      FileEntry file = GetSampleFileEntry();

      await Assert.ThrowsAsync<HttpRequestException>(() => downloader.DownloadFileAsync(file));

      mockHttpClientWrapper.Verify(c => c.GetAsync(file.Url), Times.Once);
    }

    [Fact]
    public async Task SaveFileAsync_ShouldSaveFileOnDisk_WhenValidResponse() {
      var mockHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent("sample file content"),
      };

      var downloader = new HttpsFileDownloader(Mock.Of<IHttpClientWrapper>(), _httpExampleComSamples);

      FileEntry file = GetSampleFileEntry();

      string filePath = downloader.GetFullDownloadPath(file);

      await downloader.SaveFileAsync(mockHttpResponseMessage, file);

      Assert.True(File.Exists(filePath));

      string content = File.ReadAllText(filePath);
      Assert.Equal("sample file content", content);

      File.Delete(filePath);
    }

    [Fact]
    public async Task GetFilesFromWebPageAsync_ShouldParseHtmlAndReturnFileList() {
      string htmlContent
        = "<html><body><a href='/file1.gh'>file1.gh</a><a href='/file2.3dm'>file2.3dm</a></body></html>";

      _mockHttpClient.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync(htmlContent);

      var downloader = new HttpsFileDownloader(_mockHttpClient.Object, _httpExampleComSamples);

      List<FileEntry> files = await downloader.GetFilesFromWebPageAsync();

      Assert.Equal(2, files.Count);
      Assert.Equal("file1.gh", files[0].Name);
      Assert.Equal("file2.3dm", files[1].Name);
    }

    [Fact]
    public async Task GetFilesFromWebPageAsync_ShouldReturnEmptyList_WhenNoFilesFound() {
      string htmlContent
        = "<html><body><a href='/file1.txt'>file1.txt</a><a href='/file2.txt'>file2.txt</a></body></html>";

      _mockHttpClient.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync(htmlContent);

      var downloader = new HttpsFileDownloader(_mockHttpClient.Object, _httpExampleComSamples);
      List<FileEntry> files = await downloader.GetFilesFromWebPageAsync();

      Assert.Empty(files);
    }

    [Fact]
    public async Task GetFilesFromWebPageAsync_ShouldHandleEmptyHtmlGracefully() {
      string htmlContent = "<html><body></body></html>";

      _mockHttpClient.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync(htmlContent);

      var downloader = new HttpsFileDownloader(_mockHttpClient.Object, _httpExampleComSamples);
      List<FileEntry> files = await downloader.GetFilesFromWebPageAsync();

      Assert.Empty(files);
    }

    [Fact]
    public void GetFileListFromNodes_ShouldReturnValidFileList() {
      var nodes = new List<HtmlNode> {
        HtmlNode.CreateNode("<a href='/file1.gh'>file1.gh</a>"),
        HtmlNode.CreateNode("<a href='/file2.3dm'>file2.3dm</a>"),
      };

      var downloader = new HttpsFileDownloader(_mockHttpClient.Object, _httpExampleComSamples);

      MethodInfo method = downloader.GetType()
       .GetMethod("GetFileListFromNodes", BindingFlags.NonPublic | BindingFlags.Static);

      Assert.NotNull(method);
      var collection = new HtmlNodeCollection(null) {
        nodes[0],
        nodes[1],
      };
      var fileList = method.Invoke(downloader, new object[] {
        _httpExampleComSamples,
        collection,
      }) as List<FileEntry>;

      Assert.NotNull(fileList);
      Assert.Equal(2, fileList.Count);
      Assert.Equal("file1.gh", fileList[0].Name);
      Assert.Equal("file2.3dm", fileList[1].Name);
      Assert.Equal("http://example.com/file1.gh", fileList[0].Url);
      Assert.Equal("http://example.com/file2.3dm", fileList[1].Url);
    }

    private static FileEntry GetSampleFileEntry() {
      var file = new FileEntry {
        Name = "testfile.gh",
        Url = "http://example.com/testfile.gh",
      };
      return file;
    }
  }
}
