using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Dtos;
using Microsoft.AspNetCore.Identity;
using Core.Identity;
using WetHands.Identity.Extensions;

namespace WebAPI.Controllers
{



  [AllowAnonymous]
  public class PhotoController : BaseApiController
  {


    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;


    public PhotoController(
      UserManager<AppUser> userManager,
      IMapper mapper)
    {
      _mapper = mapper;
      _userManager = userManager;
    }


    [Authorize]
    [HttpPost]
    [Route("user")]
    public async Task<ActionResult<UserToReturnDto>> AddPhotoUser()
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      var file = Request.Form.Files[0];

      using (var memoryStream = new MemoryStream())
      {
        await file.CopyToAsync(memoryStream);
        var docByte = memoryStream.ToArray();
        user.PictureByte = docByte;
        user.PictureType = file.ContentType;
        await _userManager.UpdateAsync(user);
      }

      user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (user is null)
        return BadRequest("Такой пользователь не найден...");

      var userToReturn = _mapper.Map<AppUser, UserToReturnDto>(user);
      return Ok(userToReturn);
    }



  }
}