// using System;
// using System.Globalization;
// using System.Threading;
// using System.Threading.Tasks;
// using AutoMapper;
// using Core.Dtos;
// using Core.Identity;
// using Core.Models;
// using Infrastructure.Database;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.Extensions.Configuration;
// using WetHands.Core.Models.Items;

// namespace WebAPI.Helpers
// {
//   public class ProposalDescriptionResolver : IValueResolver<Proposal, ProposalDto, string>
//   {
//     public ProposalDescriptionResolver()
//     {
//     }

//     private readonly IGenericRepository<ProposalDescriptionTranaltaion> _context;

//     public ProposalDescriptionResolver(
//       IGenericRepository<ProposalDescriptionTranaltaion> context
//       )
//     {
//       _context = context;
//     }


//     public string Resolve(Proposal source, ProposalDto destination, string destMember, ResolutionContext context)
//     {
//       var entity = _context.GetByIdAsync((int)source.ProposalDescriptionTranaltaionId);

//       CultureInfo cultureInfo = Thread.CurrentThread.CurrentUICulture;
//       var culture = cultureInfo.TwoLetterISOLanguageName;

//       Console.WriteLine("culture");
//       Console.WriteLine("culture");
//       Console.WriteLine("culture");
//       Console.WriteLine(culture);
//       Console.WriteLine(culture);
//       Console.WriteLine(culture);


//       if (culture.Contains("geo"))
//         return entity.Result.Georgian;


//       if (culture.Contains("ru"))
//         return entity.Result.Russian;


//       return entity.Result.English;

//     }
//   }

// }