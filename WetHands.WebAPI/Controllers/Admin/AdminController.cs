using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Dtos;
using Core.Identity;
using WetHands.Identity.Services;
using WetHands.Infrastructure.Database;
using WetHands.Core.Models;
using System.Linq;
using WetHands.Core;
using Core.Dtos.Identity;
using WetHands.Identity.Extensions;

namespace WebAPI.Controllers
{

  [Authorize]
  public class AdminController : BaseApiController
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IRoleManagerService _roleManager;
    private readonly IDbRepository<NftPlot> _nftPlotRepo;
    private readonly IDbRepository<Order> _plotRepo;


    // 
    public AdminController(
      UserManager<AppUser> userManager,
      IMapper mapper,
      IDbRepository<NftPlot> nftPlotRepo,
      IDbRepository<Order> plotRepo,
      IRoleManagerService roleManager)
    {
      _mapper = mapper;
      _userManager = userManager;
      _roleManager = roleManager;
      _nftPlotRepo = nftPlotRepo;
      _plotRepo = plotRepo;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("create")]
    public async Task<ActionResult<List<UserToReturnDto>>> CreateUser()
    {
      var users = await _userManager.Users.Include(x => x.Address).Include(z => z.UserRoles).ToListAsync();
      var usersToReturn = _mapper.Map<List<AppUser>, List<UserToReturnDto>>(users);

      if (usersToReturn != null) return Ok(usersToReturn);
      return BadRequest("Не удалось получить список пользователей");
    }


    [HttpPut]
    [Authorize]
    [Route("users/update")]
    public async Task<ActionResult<UserToReturnDto>> UpdateUser([FromBody] AdminUpdateUserDto dto)
    {
      var caller = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (caller is null || caller.IsAdmin == false)
        return Forbid();

      var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
      if (user is null)
        return NotFound("Пользователь не найден");

      user.CompanyId = dto.CompanyId > 0 ? dto.CompanyId : null;
      user.IsAdmin = dto.IsAdmin;

      var updateResult = await _userManager.UpdateAsync(user);
      if (!updateResult.Succeeded)
        return BadRequest("Не удалось обновить пользователя");

      var userRoles = await _userManager.GetRolesAsync(user);
      if (dto.IsAdmin)
      {
        if (!userRoles.Contains("Admin"))
        {
          await _userManager.AddToRoleAsync(user, "Admin");
        }
      }
      else
      {
        if (userRoles.Contains("Admin"))
        {
          await _userManager.RemoveFromRoleAsync(user, "Admin");
        }
      }

      var updatedUser = await _userManager.FindByIdAsync(dto.UserId.ToString());
      var roles = await _userManager.GetRolesAsync(updatedUser);
      var userToReturn = _mapper.Map<AppUser, UserToReturnDto>(updatedUser);
      userToReturn.UserRoles = roles;
      return Ok(userToReturn);
    }


    [HttpGet]
    // [Authorize(Roles = "Admin")]
    [Authorize]
    [Route("users/all")]
    public async Task<ActionResult<List<UserToReturnDto>>> GetAllUsers()
    {
      var users = await _userManager.Users.Include(x => x.Address).Include(z => z.UserRoles).ToListAsync();
      var usersToReturn = _mapper.Map<List<AppUser>, List<UserToReturnDto>>(users);

      if (usersToReturn != null)
        return Ok(usersToReturn);
      return BadRequest("Не удалось получить список пользователей");
    }




    [AllowAnonymous]
    [HttpGet]
    [Route("plots/all")]
    public async Task<ActionResult> GetAllOrders()
    {
      // Thread.Sleep(4000);

      var plots = _plotRepo.GetAll().Include(x => x.OrderStatus);
      if (plots is null)
        return Ok(plots);

      var plotsToMap = plots.ToList();
      var mappedPlots = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderDto>>(plotsToMap);
      return Ok(mappedPlots);

    }




  }

}
