using System.Net.Http;
using System.Threading.Tasks;

namespace GsaGH.UI.Helpers {
  public interface IHttpClientWrapper {
    Task<string> GetStringAsync(string url);
    Task<HttpResponseMessage> GetAsync(string url);
  }

  public class HttpClientWrapper : IHttpClientWrapper {
    private readonly HttpClient _httpClient;

    public HttpClientWrapper(HttpClient httpClient) {
      _httpClient = httpClient;
    }

    public async Task<string> GetStringAsync(string url) {
      return await _httpClient.GetStringAsync(url);
    }

    public async Task<HttpResponseMessage> GetAsync(string url) {
      return await _httpClient.GetAsync(url);
    }
  }
}
