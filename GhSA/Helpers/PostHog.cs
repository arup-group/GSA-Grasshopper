using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GsaGH.Helpers
{
  public static class PostHog
  {
    private static HttpClient _phClient = new HttpClient();

    public static async Task<HttpResponseMessage> SendToPostHog(string eventName, Dictionary<string, object> additionalProperties = null)
    {
      // posthog ADS plugin requires a user object
      User user = new User();
      user.email = System.DirectoryServices.AccountManagement.UserPrincipal.Current.EmailAddress; // case sensitive

      Dictionary<string, object> properties = new Dictionary<string, object>() {
        { "distinct_id", Environment.UserName },
        { "user", user },
        { "version", GsaGH.GsaGHInfo.Vers },
        { "isBeta", GsaGH.GsaGHInfo.isBeta }
      };

      if(additionalProperties != null)
        properties.Concat(additionalProperties);

      var container = new PhContainer(eventName, properties);
      var body = JsonConvert.SerializeObject(container);
      var content = new StringContent(body, Encoding.UTF8, "application/json");
      var response = await _phClient.PostAsync("https://posthog.insights.arup.com/capture/", content);
      return response;
    }

    private class PhContainer
    {
      [JsonProperty("api_key")]
      string api_key { get; set; } = "phc_alOp3OccDM3D18xJTWDoW44Y1cJvbEScm5LJSX8qnhs";
      [JsonProperty("event")]
      string ph_event { get; set; }
      [JsonProperty("timestamp")]
      DateTime ph_timestamp { get; set; }
      public Dictionary<string, object> properties { get; set; }

      public PhContainer(string eventName, Dictionary<string, object> properties)
      {
        this.ph_event = eventName;
        this.properties = properties;
        this.ph_timestamp = DateTime.UtcNow;
      }
    }

    private class User
    {
      public string email { get; set; }
    }
  }
}
