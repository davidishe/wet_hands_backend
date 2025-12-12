// using System.Linq;
// using System.Security.Claims;
// using AutoMapper;
// using WetHands.Identity;
// using WetHands.Identity.Database;
// using Core.Dtos;
// using Core.Identity;
// using Core.Models;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Infrastructure.Database;
// using WetHands.Core.Models.Items;
// using System.Threading.Tasks;
// using System.Collections.Generic;
// using WetHands.Infrastructure.Specifications;

// namespace WebAPI.Helpers
// {
//   public class InvestorResolver : IValueResolver<Proposal, ProposalDto, ICollection<InvestorOrderDto>>
//   {
//     public InvestorResolver()
//     {
//     }

//     private readonly UserManager<AppUser> _userManager;
//     private readonly IGenericRepository<InvestorOrder> _context;
//     private readonly IMapper _mapper;



//     public InvestorResolver(
//       UserManager<AppUser> userManager,
//       IGenericRepository<InvestorOrder> context,
//       IMapper mapper
//     )
//     {
//       _userManager = userManager;
//       _context = context;
//       _mapper = mapper;
//     }

//     // public async Task<ICollection<InvestorOrder>> Resolve(Proposal source, ProposalDto destination, InvestorOrder destMember, ResolutionContext context)
//     // {
//     //   // var userId = _userManager.FindByIdAsync(source.AuthorId.ToString()).Result.Id;
//     //   // var user = _identityContext.Users.Where(x => x.Id == userId).FirstOrDefault();
//     //   var investorsToReturn = await _context.GetAllAsync();
//     //   return investorsToReturn;Ghjnjrjk1!
//     // }

//     // public ICollection<InvestorOrder> Resolve(Proposal source, ProposalDto destination, ICollection<InvestorOrderDto> destMember, ResolutionContext context)
//     // {

//     // }

//     ICollection<InvestorOrderDto> IValueResolver<Proposal, ProposalDto, ICollection<InvestorOrderDto>>.Resolve(Proposal source, ProposalDto destination, ICollection<InvestorOrderDto> destMember, ResolutionContext context)
//     {
//       var spec = new InvestorOrderSpecification();
//       var investors = _context.ListAsync(spec).Result;
//       var investorsToReturn = _mapper.Map<IReadOnlyList<InvestorOrder>, IReadOnlyList<InvestorOrderDto>>(investors);
//       var investorsCollection = (ICollection<InvestorOrderDto>)investorsToReturn;
//       return investorsCollection;
//     }

//     // Cannot implicitly convert type 'System.Collections.Generic.IReadOnlyList<Core.Models.InvestorOrder>' to 'System.Collections.Generic.ICollection<Core.Models.InvestorOrder>'. An explicit conversion exists (are you missing a cast?) [WebAPI] csh

//   }
// }