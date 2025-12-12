using System;
using Core.Dtos;

namespace WetHands.Core.Models.Messages
{
    public class ConnectionItemDto
    {
        public UserToReturnShortDto User { get; set; }
        public string ConnectionId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}