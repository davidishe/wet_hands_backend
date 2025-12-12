using System;

namespace WetHands.Core.Models.Identity
{
  public class OneTimeCode
  {
    public string Code { get; init; } = string.Empty;

    public DateTime ExpiresAtUtc { get; init; }
  }
}
