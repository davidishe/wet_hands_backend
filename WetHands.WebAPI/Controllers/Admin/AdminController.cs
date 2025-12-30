using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    private readonly IDbRepository<MassageCategory> _massageCategoryRepo;


    // 
    public AdminController(
      UserManager<AppUser> userManager,
      IMapper mapper,
      IDbRepository<NftPlot> nftPlotRepo,
      IDbRepository<Order> plotRepo,
      IDbRepository<MassageCategory> massageCategoryRepo,
      IRoleManagerService roleManager)
    {
      _mapper = mapper;
      _userManager = userManager;
      _roleManager = roleManager;
      _nftPlotRepo = nftPlotRepo;
      _plotRepo = plotRepo;
      _massageCategoryRepo = massageCategoryRepo;
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

    // -----------------------------
    // Massage categories admin CRUD
    // -----------------------------

    [Authorize]
    [HttpGet]
    [Route("massageCategories")]
    public async Task<ActionResult<IReadOnlyList<MassageCategory>>> GetMassageCategories(
      [FromQuery] bool includeInactive = false)
    {
      var caller = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (caller is null || caller.IsAdmin == false) return Forbid();

      var query = _massageCategoryRepo.GetAll().AsNoTracking();
      if (!includeInactive)
      {
        query = query.Where(x => x.IsActive);
      }

      var items = await query
        .OrderBy(x => x.GroupName)
        .ThenBy(x => x.SortOrder)
        .ThenBy(x => x.Name)
        .ToListAsync();

      return Ok(items);
    }

    [Authorize]
    [HttpPost]
    [Route("massageCategories")]
    public async Task<ActionResult<MassageCategory>> CreateMassageCategory(
      [FromBody] MassageCategoryUpsertRequest request)
    {
      var caller = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (caller is null || caller.IsAdmin == false) return Forbid();

      if (!ModelState.IsValid) return BadRequest(ModelState);

      var name = request.Name.Trim();
      var group = NormalizeOptional(request.GroupName);

      var duplicate = await _massageCategoryRepo
        .GetAll()
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.GroupName == group && x.Name == name);

      if (duplicate != null)
      {
        return Conflict($"Категория '{name}' уже существует в группе '{group ?? "Другое"}'.");
      }

      var entity = new MassageCategory
      {
        Name = name,
        GroupName = group,
        IsActive = request.IsActive,
        SortOrder = request.SortOrder
      };

      var created = await _massageCategoryRepo.AddAsync(entity);
      return Ok(created);
    }

    [Authorize]
    [HttpPut]
    [Route("massageCategories/{id:int}")]
    public async Task<ActionResult<MassageCategory>> UpdateMassageCategory(
      [FromRoute] int id,
      [FromBody] MassageCategoryUpsertRequest request)
    {
      var caller = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (caller is null || caller.IsAdmin == false) return Forbid();

      if (!ModelState.IsValid) return BadRequest(ModelState);

      var existing = await _massageCategoryRepo.GetByIdAsync(id);
      if (existing == null) return NotFound();

      var name = request.Name.Trim();
      var group = NormalizeOptional(request.GroupName);

      var duplicate = await _massageCategoryRepo
        .GetAll()
        .AsNoTracking()
        .AnyAsync(x => x.Id != id && x.GroupName == group && x.Name == name);

      if (duplicate)
      {
        return Conflict($"Категория '{name}' уже существует в группе '{group ?? "Другое"}'.");
      }

      existing.Name = name;
      existing.GroupName = group;
      existing.IsActive = request.IsActive;
      existing.SortOrder = request.SortOrder;

      await _massageCategoryRepo.UpdateAsync(existing);
      return Ok(existing);
    }

    [Authorize]
    [HttpDelete]
    [Route("massageCategories/{id:int}")]
    public async Task<ActionResult> DeleteMassageCategory(
      [FromRoute] int id,
      [FromQuery] bool hard = false)
    {
      var caller = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (caller is null || caller.IsAdmin == false) return Forbid();

      var existing = await _massageCategoryRepo.GetByIdAsync(id);
      if (existing == null) return NotFound();

      if (hard)
      {
        await _massageCategoryRepo.DeleteAsync(existing);
        return Ok();
      }

      if (existing.IsActive)
      {
        existing.IsActive = false;
        await _massageCategoryRepo.UpdateAsync(existing);
      }

      return Ok();
    }

    private static string? NormalizeOptional(string? value)
    {
      var trimmed = value?.Trim();
      return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    public class MassageCategoryUpsertRequest
    {
      [Required]
      [MaxLength(256)]
      public required string Name { get; init; }

      [MaxLength(256)]
      public string? GroupName { get; init; }

      public bool IsActive { get; init; } = true;

      public int SortOrder { get; init; } = 0;
    }




  }

}
