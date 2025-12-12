using System.Threading.Tasks;
using AutoMapper;
using Core.Identity;
using WetHands.Core.Models;
using WetHands.Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using System;
using WetHands.Core.TonModels;
using Microsoft.Extensions.Logging;
using System.Linq;
using WetHands.Identity.Extensions;



namespace WetHands.WebAPI.Controllers
{


  [Authorize]
  public class FavourController : BaseApiController
  {

    private readonly UserManager<AppUser> _userManager;
    private readonly IDbRepository<Favour> _favourRepo;
    private readonly IDbRepository<TonLocalTransaction> _tonLocalTransactionRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<FavourController> _logger;
    private readonly IDbRepository<Order> _plotRepository;


    public FavourController(
      UserManager<AppUser> userManager,
      // IDbRepository<ProposalProfile> projectsManager,
      ILogger<FavourController> logger,
      IDbRepository<TonLocalTransaction> tonLocalTransactionRepo,
      IDbRepository<Order> plotRepository,
      IDbRepository<Favour> favourRepo,
      IMapper mapper)
    {
      _mapper = mapper;
      _logger = logger;
      _favourRepo = favourRepo;
      _plotRepository = plotRepository;
      _tonLocalTransactionRepo = tonLocalTransactionRepo;
      _userManager = userManager;
    }


    [Authorize]
    [HttpGet]
    [Route("get_for_user")]
    public async Task<ActionResult> GetByUserId()
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      var favours = _favourRepo.GetAll().Where(z => z.AppUserId == user.Id);
      return Ok(favours);
    }


    [Authorize]
    [HttpPost]
    [Route("{plotId}")]
    public async Task<ActionResult> CreateFavour([FromRoute] int plotId)
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      var favour = new Favour()
      {
        AppUserId = user.Id,
        PlotId = plotId,
        CreatedAt = DateTime.Now
      };
      var newFavour = await _favourRepo.AddAsync(favour);
      return Ok(newFavour);
    }


    [Authorize]
    [HttpDelete]
    [Route("{favourId}")]
    public async Task<ActionResult> DeleteFavour([FromRoute] int favourId)
    {
      var favour = await _favourRepo.GetByIdAsync(favourId);
      await _favourRepo.DeleteAsync(favour);
      return Ok(200);
    }






  }
}