using System.Collections.Generic;

namespace Core.Models.Contracts
{
  public class AuthFailedResponse
  {
    public IEnumerable<string> Errors { get; set; }
  }
}