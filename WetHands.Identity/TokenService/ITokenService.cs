using System.Threading.Tasks;
using Core.Identity;

namespace WetHands.Identity
{
  public interface ITokenService
  {
    Task<string> CreateToken(AppUser user);
  }
}