using System.Collections.Generic;

namespace WetHands.Core.Responses
{
  public class ApiValidationErrorResponse : ApiResponse
  {
    public ApiValidationErrorResponse() : base(400)
    {

    }

    public IList<string> Errors { get; set; }
  }
}