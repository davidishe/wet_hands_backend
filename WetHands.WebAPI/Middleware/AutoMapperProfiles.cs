using Core.Dtos;
using Core.Identity;
using Core.Models;
using WetHands.Core.Models;
using WetHands.Core.Models.Items;
using WetHands.Core.Models.Messages;

namespace WebAPI.Helpers
{
  public class AutoMapperProfiles : AutoMapper.Profile
  {

    public AutoMapperProfiles()
    {
      CreateMap<AppUser, UserToReturnDto>()
      .ForMember(d => d.PictureUrl, m => m.MapFrom<UrlPictureForUserReslover>())
      .ForMember(d => d.UserRoles, m => m.MapFrom<UserRolesReslover>());

      CreateMap<AppUser, UserToReturnShortDto>();

      CreateMap<Order, OrderDto>()
        .ForMember(d => d.StatusName, m => m.MapFrom(s => s.OrderStatus.Name))
        .ForMember(d => d.StatusIcon, m => m.MapFrom(s => s.OrderStatus.Icon))
        .ForMember(d => d.StatusDescription, m => m.MapFrom(s => s.OrderStatus.Description));


      CreateMap<OrderItem, OrderItemDto>()
        .ForMember(d => d.OrderItemTypeId, m => m.MapFrom(s => s.OrderItemType.Id))
        .ForMember(d => d.OrderItemTypeName, m => m.MapFrom(s => s.OrderItemType.Name))
        .ForMember(d => d.OrderItemTypeWeight, m => m.MapFrom(s => s.OrderItemType.Weight));


      CreateMap<OrderItemType, OrderItemTypeDto>();

      CreateMap<File, FileDto>();


      CreateMap<Correspondent, CorrespondentDto>()
          .ForMember(d => d.AppUser, m => m.MapFrom<UserForCorrespondentResolver>());


      CreateMap<Chat, ChatDto>()
        .ForMember(d => d.DestinationUser, m => m.MapFrom<RecepientUserForChatResolver>())
        .ForMember(d => d.AuthorUser, m => m.MapFrom<AuthorUserForChatResolver>());


      CreateMap<PaymentRequestType, PaymentRequestTypeDto>();
      CreateMap<PaymentRequestStatus, PaymentRequestStatusDto>();

      CreateMap<PaymentRequest, PaymentRequestDto>()
      .ForMember(d => d.Status, m => m.MapFrom(s => s.Status.Name))
      .ForMember(d => d.PaymentRequestType, m => m.MapFrom(s => s.PaymentRequestType.Name));




    }



  }
}
