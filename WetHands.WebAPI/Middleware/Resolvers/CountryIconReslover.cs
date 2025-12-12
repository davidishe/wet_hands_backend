// using System.Threading.Tasks;
// using AutoMapper;
// using Core.Dtos;
// using Core.Identity;
// using Core.Models;
// using Infrastructure.Database;
// using Microsoft.Extensions.Configuration;
// using WetHands.Core.Models.Items;
// using WetHands.Core.Models;

// namespace WebAPI.Helpers
// {
//   public class CountryIconReslover : IValueResolver<Proposal, ProposalDto, string>
//   {
//     public CountryIconReslover()
//     {
//     }

//     private readonly IGenericRepository<Country> _context;

//     public CountryIconReslover(IGenericRepository<Country> context)
//     {
//       _context = context;
//     }


//     public string Resolve(Proposal source, ProposalDto destination, string destMember, ResolutionContext context)
//     {
//       var entity = _context.GetByIdAsync((int)source.City.CountryId);
//       return entity.Result.CountryIconPath;
//     }
//   }

// }