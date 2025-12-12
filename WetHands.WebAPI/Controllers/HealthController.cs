using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
  [AllowAnonymous]
  public class HealthController : BaseApiController
  {
    [HttpGet]
    [Route("health_check")]
    public ActionResult HealthCheck()
    {
      return Ok("Все хорошо");
    }
  }
}
