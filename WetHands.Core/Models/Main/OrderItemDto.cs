using WetHands.Core.Models;

public class OrderItemDto
{
  public int? Id { get; set; }
  public int OrderId { get; set; }
  public Order Order { get; set; } = null!;
  public string Name { get; set; } = null!;
  public string? OrderItemTypeName { get; set; }
  public string? OrderItemTypeId { get; set; }
  public int? OrderItemTypeWeight { get; set; }
  public int QtyDirty { get; set; }
  public int Bags { get; set; }
  public int? QtyClean { get; set; }
  public int? BagsClean { get; set; }
  public string? CommentText { get; set; }
}


