using System.Threading;
using System.Threading.Tasks;
using WetHands.Core.Models.Identity;

namespace WetHands.Infrastructure.Services.Security
{
  public interface IOneTimeCodeStore
  {
    Task StoreEmailCodeAsync(string email, OneTimeCode code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates code and removes it on success (one-time).
    /// </summary>
    Task<bool> ValidateAndConsumeEmailCodeAsync(string email, string code, CancellationToken cancellationToken = default);

    Task StoreSmsCodeAsync(string phoneNumber, OneTimeCode code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates code and removes it on success (one-time).
    /// </summary>
    Task<bool> ValidateAndConsumeSmsCodeAsync(string phoneNumber, string code, CancellationToken cancellationToken = default);
  }
}


