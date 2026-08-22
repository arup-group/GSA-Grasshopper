using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using GsaGH.UI.Helpers;

using Xunit;

namespace GsaGHTests.UI.Helpers {
  public class HttpClientWrapperTests : IDisposable {
    private readonly HttpClient _httpClient;

    public HttpClientWrapperTests() {
      _httpClient = new HttpClient();
    }

    public void Dispose() {
      _httpClient?.Dispose();
    }

    [Fact]
    public void Constructor_SetsDefaultTimeout_WhenTimeoutIsNull() {
      var wrapper = new HttpClientWrapper(_httpClient, null);

      Assert.Equal(TimeSpan.FromSeconds(60), _httpClient.Timeout);
    }

    [Fact]
    public void Constructor_SetsCustomTimeout_WhenTimeoutIsProvided() {
      var customTimeout = TimeSpan.FromSeconds(30);

      var wrapper = new HttpClientWrapper(_httpClient, customTimeout);

      Assert.Equal(customTimeout, _httpClient.Timeout);
    }

    [Fact]
    public void Constructor_ThrowsNullException_WhenHttpClientIsNull() {
      Assert.Throws<NullReferenceException>(() => new HttpClientWrapper(null));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetStringAsync_ThrowsInvalidOperationException_WhenUrlIsNullOrEmpty(string url) {
      var handler = new MockHttpMessageHandler((request)
        => new HttpResponseMessage(HttpStatusCode.OK) {
          Content = new StringContent("response"),
        });
      using (var httpClient = new HttpClient(handler)) {
        var wrapper = new HttpClientWrapper(httpClient);

        await Assert.ThrowsAsync<InvalidOperationException>(() => wrapper.GetStringAsync(url));
      }
    }

    [Fact]
    public async Task GetStringAsync_ReturnsString_WhenRequestSucceeds() {
      const string expectedContent = "Test Content";
      var handler = new MockHttpMessageHandler((request) => new HttpResponseMessage(HttpStatusCode.OK) {
        Content = new StringContent(expectedContent),
      });
      using (var httpClient = new HttpClient(handler)) {
        var wrapper = new HttpClientWrapper(httpClient);

        string result = await wrapper.GetStringAsync("https://example.com");

        Assert.Equal(expectedContent, result);
      }
    }

    [Fact]
    public async Task GetStringAsync_ThrowsHttpRequestException_WhenRequestFails() {
      var handler = new MockHttpMessageHandler((request) => throw new HttpRequestException("Connection failed"));
      using (var httpClient = new HttpClient(handler)) {
        var wrapper = new HttpClientWrapper(httpClient);

        await Assert.ThrowsAsync<HttpRequestException>(() => wrapper.GetStringAsync("https://example.com"));
      }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetAsync_ThrowsInvalidOperationException_WhenUrlIsNullOrEmpty(string url) {
      var handler = new MockHttpMessageHandler((request) => new HttpResponseMessage(HttpStatusCode.OK));
      using (var httpClient = new HttpClient(handler)) {
        var wrapper = new HttpClientWrapper(httpClient);

        await Assert.ThrowsAsync<InvalidOperationException>(() => wrapper.GetAsync(url));
      }
    }

    [Fact]
    public async Task GetAsync_ReturnsHttpResponseMessage_WhenRequestSucceeds() {
      var handler = new MockHttpMessageHandler((request)
        => new HttpResponseMessage(HttpStatusCode.OK) {
          Content = new StringContent("response"),
        });
      using (var httpClient = new HttpClient(handler)) {
        var wrapper = new HttpClientWrapper(httpClient);

        HttpResponseMessage result = await wrapper.GetAsync("https://example.com");

        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
      }
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.Forbidden)]
    public async Task GetAsync_ReturnsHttpResponseMessage_WithErrorStatusCode(HttpStatusCode statusCode) {
      var handler = new MockHttpMessageHandler((request) => new HttpResponseMessage(statusCode));
      using (var httpClient = new HttpClient(handler)) {
        var wrapper = new HttpClientWrapper(httpClient);

        HttpResponseMessage result = await wrapper.GetAsync("https://example.com");

        Assert.NotNull(result);
        Assert.Equal(statusCode, result.StatusCode);
      }
    }

    [Fact]
    public async Task GetAsync_ThrowsHttpRequestException_WhenRequestFails() {
      var handler = new MockHttpMessageHandler((request) => throw new HttpRequestException("Connection failed"));
      using (var httpClient = new HttpClient(handler)) {
        var wrapper = new HttpClientWrapper(httpClient);

        await Assert.ThrowsAsync<HttpRequestException>(() => wrapper.GetAsync("https://example.com"));
      }
    }

    private class MockHttpMessageHandler : HttpMessageHandler {
      private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

      public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler) {
        _handler = handler;
      }

      protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken) {
        try {
          return Task.FromResult(_handler(request));
        } catch (Exception ex) {
          return Task.FromException<HttpResponseMessage>(ex);
        }
      }
    }
  }
}
