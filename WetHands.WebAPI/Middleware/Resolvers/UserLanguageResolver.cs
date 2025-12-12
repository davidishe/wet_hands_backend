using AutoMapper;
using Core.Dtos;
using Core.Identity;
using Microsoft.AspNetCore.Identity;


namespace WebAPI.Helpers
{
  public class UserLanguageResolver : IValueResolver<AppUser, UserToReturnDto, string>
  {
    public UserLanguageResolver()
    {
    }

    private readonly UserManager<AppUser> _userManager;


    public UserLanguageResolver(UserManager<AppUser> userManager)
    {
      _userManager = userManager;
    }

    public string Resolve(AppUser source, UserToReturnDto destination, string destMember, ResolutionContext context)
    {
      return source.CurrentLanguage;
    }
  }

}