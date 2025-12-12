using System;
using Core.Models;

namespace WetHands.Core.Models.Items
{
  public class Investor : BaseEntity
  {
    public string Name { get; set; }
    // public ICollection<InvestorOrder> InvestorOrders { get; set; }
    public string PictureUrl { get; set; }
    // public City? City { get; set; }
    // public int? CityId { get; set; }
    public DateTime EnrolledDate { get; set; } = DateTime.Now;
  }
}