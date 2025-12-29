using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using WetHands.Core.Models.Identity;

namespace WetHands.Infrastructure.Services.Security
{
  public class MemoryOneTimeCodeStore : IOneTimeCodeStore
  {
    private readonly IMemoryCache _cache;

    public MemoryOneTimeCodeStore(IMemoryCache cache)
    {
      _cache = cache;
    }

    private static string EmailKey(string email) => $"otp:email:{(email ?? string.Empty).Trim().ToLowerInvariant()}";
    private static string SmsKey(string phone) => $"otp:sms:{(phone ?? string.Empty).Trim()}";

    public Task StoreEmailCodeAsync(string email, OneTimeCode code, CancellationToken cancellationToken = default)
    {
      Store(EmailKey(email), code);
      return Task.CompletedTask;
    }

    public Task<bool> ValidateAndConsumeEmailCodeAsync(string email, string code, CancellationToken cancellationToken = default)
    {
      return Task.FromResult(ValidateAndConsume(EmailKey(email), code));
    }

    public Task StoreSmsCodeAsync(string phoneNumber, OneTimeCode code, CancellationToken cancellationToken = default)
    {
      Store(SmsKey(phoneNumber), code);
      return Task.CompletedTask;
    }

    public Task<bool> ValidateAndConsumeSmsCodeAsync(string phoneNumber, string code, CancellationToken cancellationToken = default)
    {
      return Task.FromResult(ValidateAndConsume(SmsKey(phoneNumber), code));
    }

    private void Store(string key, OneTimeCode code)
    {
      if (string.IsNullOrWhiteSpace(key)) return;
      if (code == null) return;
      if (string.IsNullOrWhiteSpace(code.Code)) return;

      var nowUtc = DateTime.UtcNow;
      var ttl = code.ExpiresAtUtc <= nowUtc ? TimeSpan.FromSeconds(1) : (code.ExpiresAtUtc - nowUtc);

      _cache.Set(
        key,
        code.Code.Trim(),
        new MemoryCacheEntryOptions
        {
          AbsoluteExpirationRelativeToNow = ttl
        }
      );
    }

    private bool ValidateAndConsume(string key, string providedCode)
    {
      if (string.IsNullOrWhiteSpace(key)) return false;
      if (string.IsNullOrWhiteSpace(providedCode)) return false;

      if (!_cache.TryGetValue(key, out string expected)) return false;

      // One-time: remove on first successful match.
      if (!string.Equals(expected, providedCode.Trim(), StringComparison.Ordinal))
      {
        return false;
      }

      _cache.Remove(key);
      return true;
    }
  }
}


