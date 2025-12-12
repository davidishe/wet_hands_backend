using System.Linq;
using AutoMapper;
using Core.Dtos;
using Core.Identity;
using Microsoft.AspNetCore.Identity;
using WetHands.Core.Models.Messages;
using WetHands.Identity.Database;

namespace WebAPI.Helpers
{

  public class RecepientUserForChatResolver : IValueResolver<Chat, ChatDto, UserToReturnDto>
  {
    public RecepientUserForChatResolver()
    {
    }

    private readonly UserManager<AppUser> _userManager;
    private readonly IdentityContext _identityContext;
    private readonly IMapper _mapper;

    public RecepientUserForChatResolver(
      UserManager<AppUser> userManager,
      IdentityContext identityContext,
      IMapper mapper
      )
    {
      _userManager = userManager;
      _identityContext = identityContext;
      _mapper = mapper;
    }

    public UserToReturnDto Resolve(Chat source, ChatDto destination, UserToReturnDto destMember, ResolutionContext context)
    {

      var userId = source.RecepientId;
      var user = _identityContext.Users.Where(x => x.Id == userId).FirstOrDefault();

      var userToReturn = _mapper.Map<AppUser, UserToReturnDto>(user);

      if (userToReturn is not null)
        return userToReturn;

      return null;
    }
  }

}