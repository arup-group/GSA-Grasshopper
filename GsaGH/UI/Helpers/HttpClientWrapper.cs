using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GsaGH.UI.Helpers {
  public interface IHttpClientWrapper {
    Task<string> GetStringAsync(string url);
    Task<HttpResponseMessage> GetAsync(string url);
  }

  public class HttpClientWrapper : IHttpClientWrapper {
    private readonly HttpClient _httpClient;

    public HttpClientWrapper(HttpClient httpClient, TimeSpan? timeout = null) {
      _httpClient = httpClient;
      httpClient.Timeout = timeout ?? TimeSpan.FromSeconds(60); // Default timeout
    }

    public async Task<string> GetStringAsync(string url) {
      return await _httpClient.GetStringAsync(url);
    }

    public async Task<HttpResponseMessage> GetAsync(string url) {
      return await _httpClient.GetAsync(url);
    }
  }
}
