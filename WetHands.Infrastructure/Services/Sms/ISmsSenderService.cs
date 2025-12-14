using System.Threading;
using System.Threading.Tasks;
using WetHands.Core.Models.Identity;

namespace WetHands.Infrastructure.Services.Sms
{
  public interface ISmsSenderService
  {
    Task SendOneTimeCodeAsync(string email, OneTimeCode code, string? langCode = null, CancellationToken cancellationToken = default);
  }
}
