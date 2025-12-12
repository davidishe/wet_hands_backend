using WetHands.Core.Models.Identity;

namespace WetHands.Infrastructure.Services.Security
{
  public interface IOneTimeCodeService
  {
    OneTimeCode CreateCode(int length = 6);
  }
}
