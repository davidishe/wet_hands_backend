// using AutoMapper;
// using Microsoft.Extensions.Configuration;
// using WetHands.Core.Models;

// namespace WebAPI.Helpers
// {

//   public class UrlFileResolver : IValueResolver<Plot, PlotDto, string>
//   {
//     public UrlFileResolver()
//     {
//     }

//     private readonly IConfiguration _config;

//     public UrlFileResolver(IConfiguration config)
//     {
//       _config = config;
//     }

//     public string Resolve(Plot source, PlotDto destination, string destMember, ResolutionContext context)
//     {
//       if (!string.IsNullOrEmpty(source.InvestPassportLink))
//       {
//         return _config.GetSection("AppSettings:ApiUrl").Value + "Assets/Files/Passports/" + source.Pas;
//       }
//       return null;
//     }
//   }

// }