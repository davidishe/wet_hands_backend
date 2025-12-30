using System.ComponentModel.DataAnnotations;
using Core.Models;

namespace WetHands.Core.Models
{
  /// <summary>
  /// Server-administered catalog of selectable categories for massage places.
  /// </summary>
  public class MassageCategory : BaseEntity
  {
    [Required]
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional group name used by UI to render grouped lists.
    /// </summary>
    [MaxLength(256)]
    public string? GroupName { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; } = 0;
  }
}


