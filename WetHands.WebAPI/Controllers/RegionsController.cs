using System.Threading.Tasks;
using AutoMapper;
using Core.Identity;
using WetHands.Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using Microsoft.Extensions.Logging;
using WetHands.Core.Models.Items;
using WetHands.Core.Basic;

namespace WetHands.WebAPI.Controllers
{


  [Authorize]
  public class RegionsController : BaseApiController
  {

    private readonly IDbRepository<Region> _regionRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<RegionsController> _logger;
    private readonly IDbRepository<Picture> _pictureRepo;


    public RegionsController(
      UserManager<AppUser> userManager,
      IDbRepository<Region> regionRepository,
      ILogger<RegionsController> logger,
      IDbRepository<Picture> pictureRepo,
      IMapper mapper)
    {
      _logger = logger;
      _regionRepository = regionRepository;
      _userManager = userManager;
      _pictureRepo = pictureRepo;
    }



    [AllowAnonymous]
    [HttpGet]
    [Route("all")]
    public ActionResult GetAll()
    {
      var result = _regionRepository.GetAll();
      return Ok(result);
    }


    [AllowAnonymous]
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> Create()
    {
      return Ok(200);
    }





    [AllowAnonymous]
    [HttpPost]
    [Route("update")]
    public async Task<ActionResult> Update([FromBody] Region dto)
    {

      var plotForUpdate = await _regionRepository.GetByIdAsync((int)dto.Id);
      plotForUpdate.Name = dto.Name;
      await _regionRepository.UpdateAsync(plotForUpdate);
      return Ok(200);
    }




  }
}