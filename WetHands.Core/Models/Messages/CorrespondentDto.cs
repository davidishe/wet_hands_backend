using System;
using Core.Dtos;
using Core.Models;

namespace WetHands.Core.Models.Messages
{
    public class CorrespondentDto : BaseEntity
    {
        public int CoresspondentId { get; set; }
        public int AnotherCoresspondentId { get; set; }
        public UserToReturnDto? AppUser { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}