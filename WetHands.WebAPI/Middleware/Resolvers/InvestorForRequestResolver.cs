// using System.Linq;
// using AutoMapper;
// using Core.Dtos;
// using Core.Identity;
// using Microsoft.AspNetCore.Identity;
// using WetHands.Core.Models;
// using WetHands.Identity.Database;

// namespace WebAPI.Helpers
// {

//   public class InvestorForRequestResolver : IValueResolver<Request, RequestDto, UserToReturnDto>
//   {
//     public InvestorForRequestResolver()
//     {
//     }

//     private readonly UserManager<AppUser> _userManager;
//     private readonly IdentityContext _identityContext;
//     private readonly IMapper _mapper;

//     public InvestorForRequestResolver(
//       UserManager<AppUser> userManager,
//       IdentityContext identityContext,
//       IMapper mapper
//       )
//     {
//       _userManager = userManager;
//       _identityContext = identityContext;
//       _mapper = mapper;
//     }

//     public UserToReturnDto Resolve(Request source, RequestDto destination, UserToReturnDto destMember, ResolutionContext context)
//     {

//       var userId = source.InvestorUserId;
//       var user = _identityContext.Users.Where(x => x.Id == userId).FirstOrDefault();

//       var userToReturn = _mapper.Map<AppUser, UserToReturnDto>(user);

//       if (userToReturn is not null)
//         return userToReturn;

//       return null;
//     }
//   }

// }