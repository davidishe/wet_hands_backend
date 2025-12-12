// using AutoMapper;
// using Core.Identity;
// using Microsoft.AspNetCore.Identity;
// using WetHands.Core.Models;
// using WetHands.Identity.Database;

// namespace WebAPI.Helpers
// {

//   public class ProposalProfileReadyToDealResolver : IValueResolver<ProposalProfile, ProposalProfileDto, bool?>
//   {
//     public ProposalProfileReadyToDealResolver()
//     {
//     }

//     private readonly UserManager<AppUser> _userManager;
//     private readonly IdentityContext _identityContext;
//     private readonly IMapper _mapper;

//     public ProposalProfileReadyToDealResolver(
//       UserManager<AppUser> userManager,
//       IdentityContext identityContext,
//       IMapper mapper
//       )
//     {
//       _userManager = userManager;
//       _identityContext = identityContext;
//       _mapper = mapper;
//     }

//     public bool? Resolve(ProposalProfile source, ProposalProfileDto destination, bool? destMember, ResolutionContext context)
//     {

//       if (source.ProposalProfileStatusId == 3)
//         return false;

//       double currentPledgeAmmount = 0;
//       foreach (var request in source.Requests)
//       {
//         if (request.RequestStatusId != 5)
//           currentPledgeAmmount += request.ShareValue;
//       }

//       if (currentPledgeAmmount > source.OveralInvestSum)
//         return true;

//       if (currentPledgeAmmount == source.OveralInvestSum)
//         return true;

//       else
//         return false;

//     }

//   }

// }