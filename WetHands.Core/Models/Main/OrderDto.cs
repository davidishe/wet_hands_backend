using System;
using System.Collections.Generic;
using WetHands.Core.Basic;


namespace WetHands.Core.Models
{

    public class OrderDto
    {
        public int? Id { get; set; }

        public int? AuthorId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? RegionId { get; set; }
        public Region? Region { get; set; }
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
        public int? OrderStatusId { get; set; }
        public string? StatusName { get; set; }
        public string? StatusDescription { get; set; }
        public string? StatusIcon { get; set; }
        public int? ServiceTypeId { get; set; }
        public decimal? WeightKg { get; set; }
        public int? BagsCount { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? ContactPhoneText { get; set; }
        public string? ContactNameText { get; set; }

        // Навигация
        public ICollection<OrderItemDto>? Items { get; set; }

    }






}

