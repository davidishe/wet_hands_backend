namespace WetHands.Core.Responses
{
  public class ApiResponse
  {

    public int StatusCode { get; set; }
    public string Message { get; set; }

    public ApiResponse(int statusCode, string message = null)
    {
      StatusCode = statusCode;
      Message = message ?? GetDefaultMessageForStatusCode(StatusCode);
    }

    private string GetDefaultMessageForStatusCode(int statusCode)
    {
      return statusCode switch
      {
        400 => "Плохой запрос ты сделал здесь",
        401 => "Не авторизовался ты",
        404 => "Не найдено ничего такого",
        405 => "Токен не создался",
        500 => "На стороне сервера произошла ошибка",
        _ => null
      };
    }
  }
}