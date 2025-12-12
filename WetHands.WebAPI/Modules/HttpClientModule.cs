using System.Net.Http;
using System.Threading.Tasks;


namespace WetHands.WebAPI.Modules
{
  public class HttpClientModule
  {


    public async Task<object> MakeHttpCall(string requestUrl)
    {
      using var httpClient = new HttpClient();
      using var response = await httpClient.GetAsync(requestUrl);
      return response;
    }


    // public async Task<string> MakeHttpCallWithStream(string requestUrl)
    // {
    //   using var httpClient = new HttpClient();
    //   using var response = await httpClient.GetAsync(requestUrl);
    //   var content = await response.Content.ReadAsStringAsync();
    //   return content;

    // }



  }

}