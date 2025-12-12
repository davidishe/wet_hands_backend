using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class NPTokenProvider<TUser> : DataProtectorTokenProvider<TUser>
where TUser : IdentityUser
{
  public NPTokenProvider(
      IDataProtectionProvider dataProtectionProvider,
      IOptions<NPTokenProviderOptions> options, ILogger<NPTokenProvider<TUser>> logger)
      : base(dataProtectionProvider, options, logger)
  { }

  // public override Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
  // {
  //   return Task.FromResult<string>("AABBSS");
  // }

}

public static class CustomIdentityBuilderExtensions
{
  public static IdentityBuilder AddNPTokenProvider(this IdentityBuilder builder)
  {
    var userType = builder.UserType;
    var provider = typeof(NPTokenProvider<>).MakeGenericType(userType);
    return builder.AddTokenProvider("NPTokenProvider", provider);
  }
}

