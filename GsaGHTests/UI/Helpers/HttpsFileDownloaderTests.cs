using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using GsaGH.UI;
using GsaGH.UI.Helpers;

using HtmlAgilityPack;

using Moq;

using Xunit;

namespace GsaGHTests.UI.Helpers {
  public class HttpsFileDownloaderTests : IDisposable {
    private readonly Mock<IHttpClientWrapper> _mockHttpClient;
    private const string HttpExampleComSamples = "http://example.com/samples";
    private const string HttpsExampleComSamples = "https://example.com/samples";
    private const string SampleFileUrl = "http://example.com/testfile.gh";
    private const string SampleFileName = "testfile.gh";
    private readonly string _tempDir;
    private readonly HttpsFileDownloader _downloader;

    public HttpsFileDownloaderTests() {
      _mockHttpClient = new Mock<IHttpClientWrapper>();
      _tempDir = Path.Combine(Path.GetTempPath(), $"HttpsFileDownloaderTests_{Guid.NewGuid()}");
      Directory.CreateDirectory(_tempDir);
      _downloader = new HttpsFileDownloader(_mockHttpClient.Object, HttpExampleComSamples, _tempDir);
    }

    public void Dispose() {
      try {
        if (Directory.Exists(_tempDir)) {
          Directory.Delete(_tempDir, true);
        }
      } catch {
        // Ignore cleanup errors
      }
    }

    #region Helper Methods

    private static FileEntry CreateSampleFileEntry(string name = SampleFileName, string url = SampleFileUrl) {
      return new FileEntry {
        Name = name,
        Url = url,
      };
    }

    private HttpsFileDownloader CreateDownloader(
      Mock<IHttpClientWrapper> httpClientWrapper = null, string urlToSamples = HttpExampleComSamples) {
      return new HttpsFileDownloader(httpClientWrapper?.Object ?? _mockHttpClient.Object, urlToSamples, _tempDir);
    }

    private static Mock<IHttpClientWrapper> CreateMockHttpClientWithResponse(
      HttpStatusCode statusCode, string content = "") {
      var mock = new Mock<IHttpClientWrapper>();
      var response = new HttpResponseMessage(statusCode) {
        Content = new StringContent(content),
      };
      mock.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(response);
      return mock;
    }

    private static Mock<IHttpClientWrapper> CreateMockHttpClientWithStringResponse(string htmlContent) {
      var mock = new Mock<IHttpClientWrapper>();
      mock.Setup(c => c.GetStringAsync(It.IsAny<string>())).ReturnsAsync(htmlContent);
      return mock;
    }

    private static Mock<IHttpClientWrapper> CreateMockHttpClientWithException(Exception exception) {
      var mock = new Mock<IHttpClientWrapper>();
      mock.Setup(c => c.GetAsync(It.IsAny<string>())).ThrowsAsync(exception);
      return mock;
    }

    private static List<HtmlNode> CreateHtmlNodes(params string[] htmlStrings) {
      return htmlStrings.Select(HtmlNode.CreateNode).ToList();
    }

    private List<FileEntry> InvokeGetFileListFromNodes(string url, HtmlNodeCollection nodes) {
      MethodInfo method = _downloader.GetType()
       .GetMethod("GetFileListFromNodes", BindingFlags.NonPublic | BindingFlags.Static);

      Assert.NotNull(method);
      return method.Invoke(_downloader, new object[] {
        url,
        nodes,
      }) as List<FileEntry>;
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenHttpClientWrapperIsNull() {
      Assert.Throws<ArgumentNullException>(() => new HttpsFileDownloader(null, HttpExampleComSamples, _tempDir));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ThrowsArgumentException_WhenUrlToSamplesIsNullOrWhitespace(string url) {
      Assert.Throws<ArgumentException>(() => CreateDownloader(urlToSamples: url));
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com")]
    [InlineData("file:///C:/file.txt")]
    [InlineData("example.com")]
    public void Constructor_ThrowsArgumentException_WhenUrlToSamplesIsInvalid(string url) {
      Assert.Throws<ArgumentException>(() => CreateDownloader(urlToSamples: url));
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://example.com/path")]
    [InlineData("https://example.com/samples?query=value")]
    public void Constructor_SucceedsWithValidUrl(string url) {
      HttpsFileDownloader downloader = CreateDownloader(urlToSamples: url);

      Assert.NotNull(downloader);
      Assert.Equal(url, downloader.UrlToSamples);
    }

    [Fact]
    public void Constructor_SetsUrlToSamples() {
      HttpsFileDownloader downloader = CreateDownloader(urlToSamples: HttpsExampleComSamples);

      Assert.Equal(HttpsExampleComSamples, downloader.UrlToSamples);
    }

    [Theory]
    [InlineData("http://example.com")]
    [InlineData("http://example.com/")]
    [InlineData("http://example.com/path")]
    public void Constructor_AcceptsValidHttpUrl(string url) {
      HttpsFileDownloader downloader = CreateDownloader(urlToSamples: url);

      Assert.Equal(url, downloader.UrlToSamples);
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("https://example.com/")]
    [InlineData("https://example.com/path")]
    public void Constructor_AcceptsValidHttpsUrl(string url) {
      HttpsFileDownloader downloader = CreateDownloader(urlToSamples: url);

      Assert.Equal(url, downloader.UrlToSamples);
    }

    #endregion

    #region SetCustomDownloadsPath Tests

    [Fact]
    public void SetCustomDownloadsPath_ReturnsDefaultPath_WhenCustomPathIsEmpty() {
      string expectedDefault = HttpsFileDownloader.DefaultDownloadPath;

      string result = _downloader.SetCustomDownloadsPath("");

      Assert.Equal(expectedDefault, result);
    }

    [Fact]
    public void SetCustomDownloadsPath_ReturnsDefaultPath_WhenCustomPathIsNull() {
      string expectedDefault = HttpsFileDownloader.DefaultDownloadPath;

      string result = _downloader.SetCustomDownloadsPath(null);

      Assert.Equal(expectedDefault, result);
    }

    [Fact]
    public void SetCustomDownloadsPath_ReturnsDefaultPath_WhenCustomPathIsWhitespace() {
      string expectedDefault = HttpsFileDownloader.DefaultDownloadPath;

      string result = _downloader.SetCustomDownloadsPath("   ");

      Assert.Equal(expectedDefault, result);
    }

    [Fact]
    public void SetCustomDownloadsPath_ReturnsCustomPath_WhenValidRootedPathProvided() {
      string result = _downloader.SetCustomDownloadsPath(_tempDir);

      Assert.Equal(_tempDir, result);
    }

    [Fact]
    public void SetCustomDownloadsPath_ThrowsArgumentException_WhenPathNotRooted() {
      Assert.Throws<ArgumentException>(() => _downloader.SetCustomDownloadsPath("relative/path"));
    }

    [Fact]
    public void SetCustomDownloadsPath_ThrowsArgumentException_WhenPathDoesNotExist() {
      string nonExistentPath = Path.Combine(_tempDir, "nonexistent");

      Assert.Throws<ArgumentException>(() => _downloader.SetCustomDownloadsPath(nonExistentPath));
    }

    #endregion

    #region DefaultDownloadPath Tests

    [Fact]
    public void DefaultDownloadPath_ReturnsUserDownloadsFolder() {
      string result = HttpsFileDownloader.DefaultDownloadPath;

      Assert.NotNull(result);
      Assert.NotEmpty(result);
      Assert.True(Directory.Exists(result));
      Assert.Contains("Downloads", result);
    }

    #endregion

    #region GetFullDownloadPath Tests

    [Fact]
    public void GetFullDownloadPath_ThrowsArgumentNullException_WhenFileIsNull() {
      Assert.Throws<ArgumentNullException>(() => _downloader.GetFullDownloadPath(null));
    }

    [Fact]
    public void GetFullDownloadPath_ReturnsCorrectPath() {
      FileEntry file = CreateSampleFileEntry();
      string expectedPath = Path.Combine(_tempDir, SampleFileName);

      string fullPath = _downloader.GetFullDownloadPath(file);

      Assert.Equal(expectedPath, fullPath);
    }

    [Fact]
    public void GetFullDownloadPath_ThrowsException_WhenFileNameWithPath() {
      FileEntry file = CreateSampleFileEntry("testfile/gh");

      Assert.Throws<ArgumentException>(() => _downloader.GetFullDownloadPath(file));
    }

    [Fact]
    public void GetFullDownloadPath_SanitizesFileName() {
      FileEntry file = CreateSampleFileEntry("test file.gh");
      string result = _downloader.GetFullDownloadPath(file);

      Assert.DoesNotContain("?", result);
      Assert.DoesNotContain("*", result);
      Assert.True(result.EndsWith("test file.gh"));
    }

    #endregion

    #region DownloadFileAsync Tests

    [Fact]
    public async Task DownloadFileAsync_ThrowsArgumentNullException_WhenFileIsNull() {
      await Assert.ThrowsAsync<ArgumentNullException>(() => _downloader.DownloadFileAsync(null));
    }

    [Fact]
    public async Task DownloadFileAsync_DownloadsFileCorrectly() {
      Mock<IHttpClientWrapper> mockHttpClientWrapper
        = CreateMockHttpClientWithResponse(HttpStatusCode.OK, "sample file content");
      HttpsFileDownloader downloader = CreateDownloader(mockHttpClientWrapper);
      FileEntry file = CreateSampleFileEntry();

      await downloader.DownloadFileAsync(file);

      mockHttpClientWrapper.Verify(c => c.GetAsync(file.Url), Times.Once);
      string filePath = downloader.GetFullDownloadPath(file);
      Assert.True(File.Exists(filePath));
    }

    [Fact]
    public async Task DownloadFileAsync_ThrowsException_WhenHttpResponseIsNotSuccess() {
      Mock<IHttpClientWrapper> mockHttpClientWrapper = CreateMockHttpClientWithResponse(HttpStatusCode.NotFound);
      HttpsFileDownloader downloader = CreateDownloader(mockHttpClientWrapper);
      FileEntry file = CreateSampleFileEntry();

      await Assert.ThrowsAsync<HttpRequestException>(() => downloader.DownloadFileAsync(file));

      mockHttpClientWrapper.Verify(c => c.GetAsync(file.Url), Times.Once);
    }

    [Fact]
    public async Task DownloadFileAsync_ThrowsException_WhenHttpRequestExceptionOccurs() {
      Mock<IHttpClientWrapper> mockHttpClientWrapper
        = CreateMockHttpClientWithException(new HttpRequestException("Request failed"));
      HttpsFileDownloader downloader = CreateDownloader(mockHttpClientWrapper);
      FileEntry file = CreateSampleFileEntry();

      await Assert.ThrowsAsync<HttpRequestException>(() => downloader.DownloadFileAsync(file));

      mockHttpClientWrapper.Verify(c => c.GetAsync(file.Url), Times.Once);
    }

    #endregion

    #region SaveFileAsync Tests

    [Fact]
    public async Task SaveFileAsync_ThrowsArgumentNullException_WhenResponseIsNull() {
      FileEntry file = CreateSampleFileEntry();

      await Assert.ThrowsAsync<ArgumentNullException>(() => _downloader.SaveFileAsync(null, file));
    }

    [Fact]
    public async Task SaveFileAsync_ThrowsInvalidOperationException_WhenContentIsNull() {
      var response = new HttpResponseMessage(HttpStatusCode.OK) {
        Content = null,
      };
      FileEntry file = CreateSampleFileEntry();

      await Assert.ThrowsAsync<InvalidOperationException>(() => _downloader.SaveFileAsync(response, file));
    }

    [Fact]
    public async Task SaveFileAsync_SavesFileOnDisk_WhenValidResponse() {
      var mockHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent("sample file content"),
      };
      FileEntry file = CreateSampleFileEntry();
      string filePath = _downloader.GetFullDownloadPath(file);

      await _downloader.SaveFileAsync(mockHttpResponseMessage, file);

      Assert.True(File.Exists(filePath));
      string content = File.ReadAllText(filePath);
      Assert.Equal("sample file content", content);
    }

    #endregion

    #region GetFilesFromWebPageAsync Tests

    [Fact]
    public async Task GetFilesFromWebPageAsync_ParsesHtmlAndReturnsFileList() {
      string htmlContent
        = "<html><body><a href='/file1.gh'>file1.gh</a><a href='/file2.3dm'>file2.3dm</a></body></html>";
      Mock<IHttpClientWrapper> mockHttpClientWrapper = CreateMockHttpClientWithStringResponse(htmlContent);
      HttpsFileDownloader downloader = CreateDownloader(mockHttpClientWrapper);

      List<FileEntry> files = await downloader.GetFilesFromWebPageAsync();

      Assert.Equal(2, files.Count);
      Assert.Equal("file1.gh", files[0].Name);
      Assert.Equal("file2.3dm", files[1].Name);
    }

    [Fact]
    public async Task GetFilesFromWebPageAsync_ReturnsEmptyList_WhenNoFilesFound() {
      string htmlContent
        = "<html><body><a href='/file1.txt'>file1.txt</a><a href='/file2.txt'>file2.txt</a></body></html>";
      Mock<IHttpClientWrapper> mockHttpClientWrapper = CreateMockHttpClientWithStringResponse(htmlContent);
      HttpsFileDownloader downloader = CreateDownloader(mockHttpClientWrapper);

      List<FileEntry> files = await downloader.GetFilesFromWebPageAsync();

      Assert.Empty(files);
    }

    [Fact]
    public async Task GetFilesFromWebPageAsync_HandlesEmptyHtmlGracefully() {
      string htmlContent = "<html><body></body></html>";
      Mock<IHttpClientWrapper> mockHttpClientWrapper = CreateMockHttpClientWithStringResponse(htmlContent);
      HttpsFileDownloader downloader = CreateDownloader(mockHttpClientWrapper);

      List<FileEntry> files = await downloader.GetFilesFromWebPageAsync();

      Assert.Empty(files);
    }

    [Fact]
    public async Task GetFilesFromWebPageAsync_HandlesMalformedHtmlGracefully() {
      string htmlContent = "<html><body><a href='/file1.gh'>file1.gh"; // tag not closed
      Mock<IHttpClientWrapper> mockHttpClientWrapper = CreateMockHttpClientWithStringResponse(htmlContent);
      HttpsFileDownloader downloader = CreateDownloader(mockHttpClientWrapper);

      List<FileEntry> files = await downloader.GetFilesFromWebPageAsync();

      Assert.Single(files);
      Assert.Equal("file1.gh", files[0].Name);
    }

    [Fact]
    public async Task GetFilesFromWebPageAsync_ThrowsException_WhenHttpClientThrows() {
      Mock<IHttpClientWrapper> mockHttpClientWrapper
        = CreateMockHttpClientWithException(new HttpRequestException("Network error"));
      mockHttpClientWrapper.Setup(c => c.GetStringAsync(It.IsAny<string>()))
       .ThrowsAsync(new HttpRequestException("Network error"));
      HttpsFileDownloader downloader = CreateDownloader(mockHttpClientWrapper);

      await Assert.ThrowsAsync<HttpRequestException>(() => downloader.GetFilesFromWebPageAsync());
    }

    #endregion

    #region GetFileListFromNodes Tests

    [Fact]
    public void GetFileListFromNodes_ReturnsValidFileList() {
      List<HtmlNode> nodes = CreateHtmlNodes("<a href='/file1.gh'>file1.gh</a>", "<a href='/file2.3dm'>file2.3dm</a>");

      var collection = new HtmlNodeCollection(null);
      foreach (HtmlNode node in nodes) {
        collection.Add(node);
      }

      List<FileEntry> fileList = InvokeGetFileListFromNodes(HttpExampleComSamples, collection);

      Assert.NotNull(fileList);
      Assert.Equal(2, fileList.Count);
      Assert.Equal("file1.gh", fileList[0].Name);
      Assert.Equal("file2.3dm", fileList[1].Name);
      Assert.Equal("http://example.com/file1.gh", fileList[0].Url);
      Assert.Equal("http://example.com/file2.3dm", fileList[1].Url);
    }

    [Fact]
    public void GetFileListFromNodes_ReturnsEmptyList_WhenNodesIsNull() {
      List<FileEntry> fileList = InvokeGetFileListFromNodes(HttpExampleComSamples, null);

      Assert.NotNull(fileList);
      Assert.Empty(fileList);
    }

    #endregion

  }
}
