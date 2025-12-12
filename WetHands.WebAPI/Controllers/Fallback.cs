using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{

  [AllowAnonymous]
  public class Fallback : Controller
  {
    public IActionResult Index()
    {
      return NotFound("Не найден такой роут");
    }
  }

}