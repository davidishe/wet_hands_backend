using Core.Models;
using WetHands.Core.Models;

public class OrderItem : BaseEntity
{
  public int OrderId { get; set; }
  public Order? Order { get; set; } = null!;
  public string? Name { get; set; } = null!;
  public int? OrderItemTypeId { get; set; }
  public OrderItemType? OrderItemType { get; set; }
  public int? QtyDirty { get; set; }
  public int? Bags { get; set; }
  public int? QtyClean { get; set; }
  public int? BagsClean { get; set; }
  public string? CommentText { get; set; }
}

