using System;
using Core.Models;

namespace WetHands.Core.Models.Messages
{
    public class Chat : BaseEntity
    {
        // public ICollection<Message>? Messages { get; set; }
        public int RecepientId { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}