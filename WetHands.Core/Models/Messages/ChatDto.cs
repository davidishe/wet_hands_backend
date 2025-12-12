using System;
using Core.Dtos;
using Core.Models;

namespace WetHands.Core.Models.Messages
{
    public class ChatDto : BaseEntity
    {
        // public ICollection<MessageDto>? Messages { get; set; }
        public int RecepientId { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public UserToReturnDto DestinationUser { get; set; }
        public UserToReturnDto AuthorUser { get; set; }

    }
}