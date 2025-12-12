using System.Threading.Tasks;

namespace WetHands.Identity.Services
{
  public interface IRoleManagerService
  {

    Task<bool> ChangeUserRoles(string[] roles, string userId);

  }
}