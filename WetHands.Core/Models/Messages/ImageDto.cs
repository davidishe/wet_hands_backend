using Core.Models;

namespace WetHands.Core.Models.Messages
{
    public class ImageDto : BaseEntity
    {
        public int MessageId { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public int Size { get; set; }
    }
}