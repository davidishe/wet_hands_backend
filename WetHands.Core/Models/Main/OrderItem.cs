using Core.Models;
using WetHands.Core.Models;

/// <summary>
/// Order item entity used by Orders (laundry-like line items).
/// Note: Some legacy models in this project live in the global namespace,
/// so we keep this class un-namespaced to match existing references.
/// </summary>
public class OrderItem : BaseEntity
{
  public int OrderId { get; set; }
  public Order Order { get; set; } = null!;

  public string Name { get; set; } = string.Empty;

  public int QtyDirty { get; set; }
  public int Bags { get; set; }

  public int? QtyClean { get; set; }
  public int? BagsClean { get; set; }

  public string? CommentText { get; set; }

  public int? OrderItemTypeId { get; set; }
  public OrderItemType? OrderItemType { get; set; }
}


