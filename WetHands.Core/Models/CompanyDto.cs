using Core.Models;

namespace WetHands.Core.Models
{
    public class CompanyDto : BaseEntity
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public string? Mail { get; set; }
        public string? FacebookUserName { get; set; }
        public string? InstagramUserName { get; set; }
        public string? TelegramCompanyName { get; set; }
        public string? Address { get; set; }
        public byte[]? CompanyAvatar { get; set; }
        public decimal? LaundryPricePerKg { get; set; }
        public decimal? DryCleaningPricePerKg { get; set; }
        public decimal? RestaurantPricePerKg { get; set; }



    }
}
