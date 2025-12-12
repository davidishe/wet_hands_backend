using System;
using Core.Models;

namespace WetHands.Core.Models
{
  public class Grade : BaseEntity
  {
    public DateTime CreatedAt { get; set; }
    public int ProfileId { get; set; }
    public int? Type { get; set; }
    public string Description { get; set; }
    public decimal Rate { get; set; }


  }
}