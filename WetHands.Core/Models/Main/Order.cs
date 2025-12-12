using System;
using System.Collections.Generic;
using Core.Models;
using WetHands.Core.Basic;

namespace WetHands.Core.Models
{
    public class Order : BaseEntity
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int? RegionId { get; set; }
        public Region? Region { get; set; }

        public int? CompanyId { get; set; }
        public Company? Company { get; set; }

        public int? OrderStatusId { get; set; }
        public OrderStatus? OrderStatus { get; set; }

        // Поля из JSON заявки
        public ServiceType? ServiceType { get; set; }
        public decimal? WeightKg { get; set; }
        public int? BagsCount { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? ContactPhoneText { get; set; }
        public string? ContactNameText { get; set; }
        // Навигация
        public ICollection<OrderItem>? Items { get; set; } = new List<OrderItem>();

    }











}

