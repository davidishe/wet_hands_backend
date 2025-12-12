using System;
using Core.Models;

namespace WetHands.Core.Models
{

  /// <summary>
  /// здесь избранное
  /// </summary>
  public class Favour : BaseEntity
  {
    public int PlotId { get; set; }
    public int AppUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Order? Order { get; set; }

  }
}