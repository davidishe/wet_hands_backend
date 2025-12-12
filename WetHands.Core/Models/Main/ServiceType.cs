// ========== Справочник ServiceType ==========
using System.Collections.Generic;
using WetHands.Core.Models;

public class ServiceType
{
  public int Id { get; set; }
  public string Code { get; set; } = null!; // например: "laundry", "dryclean"
  public string Name { get; set; } = null!; // Человекочитаемое название
  public ICollection<Order> Orders { get; set; } = new List<Order>();
}