using Newtonsoft.Json;

namespace Core.Models.Contracts.Google
{
  public class GoogleUserInfoResult
  {
    [JsonProperty("sub")]
    public string Id { get; set; }

    [JsonProperty("given_name")]
    public string FirstName { get; set; }

    [JsonProperty("family_name")]
    public string LastName { get; set; }

    [JsonProperty("picture")]
    public string PictureUrl { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

  }


  // "{\n  
  // \"sub\": \"113602998486841610587\",\n  

  // \"name\": \"Давид Акобия\",\n  

  // \"given_name\": \"Давид\",\n  
  // \"family_name\": \"Акобия\",\n  

  // \"picture\": \"https://lh3.googleusercontent.com/a-/AOh14Gic3BbYRTVGFIRpz7t3tJSV8jXUAaY709yh6GTd\\u003ds96-c\",\n  
  // \"email\": \"akobiya.david@gmail.com\",\n  
  // \"email_verified\": true,\n  
  // \"locale\": \"ru\"\n}"


}