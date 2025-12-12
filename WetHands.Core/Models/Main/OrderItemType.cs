using System.Collections.Generic;
using Core.Models;

// Справочник типов элементов заказа
public class OrderItemType : BaseEntity
{
  public int Weight { get; set; }
  public string? Name { get; set; }

  // Обратная навигация: многие OrderItem могут ссылаться на один тип
  public ICollection<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();
}
