using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Core.Identity;
using System.Collections.Generic;

namespace WetHands.Identity.Services
{
  public class RoleManagerService : IRoleManagerService
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public RoleManagerService(
      UserManager<AppUser> userManager,
      IMapper mapper)
    {
      _mapper = mapper;
      _userManager = userManager;
    }

    public async Task<bool> ChangeUserRoles(string[] roles, string userId)
    {
      var user = await _userManager.FindByIdAsync(userId);
      var userRoles = await _userManager.GetRolesAsync(user);
      await _userManager.RemoveFromRolesAsync(user, userRoles);
      IEnumerable<string> enumRoles = roles;
      await _userManager.AddToRolesAsync(user, enumRoles);
      return true;
    }
  }
}