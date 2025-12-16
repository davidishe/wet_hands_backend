using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using WetHands.Core.Models.Identity;

namespace WetHands.Infrastructure.Services.Security
{
  public class OneTimeCodeService : IOneTimeCodeService
  {
    private const int DefaultCodeLength = 4;
    private readonly ILogger<OneTimeCodeService> _logger;
    private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(30);

    public OneTimeCodeService(ILogger<OneTimeCodeService> logger)
    {
      _logger = logger;
    }

    public OneTimeCode CreateCode(int length = DefaultCodeLength)
    {
      if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length), length, "Length must be greater than zero.");

      var codeBuilder = new StringBuilder(length);
      Span<byte> buffer = stackalloc byte[length];
      RandomNumberGenerator.Fill(buffer);

      for (var i = 0; i < length; i++)
      {
        var digit = buffer[i] % 10;
        codeBuilder.Append(digit);
      }

      var codeValue = codeBuilder.ToString();
      _logger.LogDebug("Generated one-time code of length {Length}.", length);

      return new OneTimeCode
      {
        Code = codeValue,
        ExpiresAtUtc = DateTime.UtcNow.Add(CodeLifetime)
      };
    }
  }
}
