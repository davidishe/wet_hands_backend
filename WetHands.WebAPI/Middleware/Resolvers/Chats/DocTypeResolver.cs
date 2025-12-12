// using System.Linq;
// using AutoMapper;
// using Core.Dtos;
// using Core.Identity;
// using Core.Models;
// using Humanizer;
// using Microsoft.AspNet.SignalR.Messaging;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.Extensions.Configuration;
// using WetHands.Core.Models;
// using WetHands.Core.Models.Messages;
// using WetHands.Identity.Database;
// using Message = WetHands.Core.Models.Messages.Message;

// namespace WebAPI.Helpers
// {

//   public class DocTypeResolver : IValueResolver<Message, MessageDto, byte>
//   {
//     public DocTypeResolver()
//     {
//     }

//     private readonly UserManager<AppUser> _userManager;
//     private readonly IdentityContext _identityContext;
//     private readonly IMapper _mapper;

//     public DocTypeResolver(
//       UserManager<AppUser> userManager,
//       IdentityContext identityContext,
//       IMapper mapper
//       )
//     {
//       _userManager = userManager;
//       _identityContext = identityContext;
//       _mapper = mapper;
//     }

//     public byte Resolve(Message source, MessageDto destination, byte destMember, ResolutionContext context)
//     {
//       var result = source.DocByte[0];
//       return result;

//     }
//   }

// }