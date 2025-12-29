using System.ComponentModel.DataAnnotations;
using Core.Models;

namespace WetHands.Core.Basic
{
  public class Country : BaseEntity
  {
    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(128)]
    public string? CountryIconPath { get; set; }
  }
}


