using Newtonsoft.Json;

namespace Core.Models.Contracts
{
  public class AccesssTokenResponse
  {
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
  }
}