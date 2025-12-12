using Newtonsoft.Json;

namespace Core.External.Contracts
{
  public class GoogleTokenValidationResult
  {

    [JsonProperty("azp")]
    public string ClientId { get; set; }

    [JsonProperty("aud")]
    public string ClientIdAud { get; set; }

    [JsonProperty("sub")]
    public string UserId { get; set; }

    [JsonProperty("scope")]
    public string Scope { get; set; }

    [JsonProperty("exp")]
    public long ExpiresAt { get; set; }

    [JsonProperty("expires_in")]
    public long ExpiresIn { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("email_verified")]
    public string EmailVerified { get; set; }

    [JsonProperty("access_type")]
    public string AccessType { get; set; }

  }

}



// "{\n  
// \"azp\": \"263769089352-26v7o8794c42njl3u591acssfsuo0th3.apps.googleusercontent.com\",\n  
// \"aud\": \"263769089352-26v7o8794c42njl3u591acssfsuo0th3.apps.googleusercontent.com\",\n  
// \"sub\": \"113602998486841610587\",\n  
// \"scope\": \"openid https://www.googleapis.com/auth/userinfo.email\",\n  
// \"exp\": \"1602185896\",\n  
// \"expires_in\": \"3556\",\n  
// \"email\": \"akobiya.david@gmail.com\",\n  
// \"email_verified\": \"true\",\n  
// \"access_type\": \"offline\"\n
// }\n"
