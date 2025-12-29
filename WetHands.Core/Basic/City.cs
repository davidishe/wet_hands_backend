using System.ComponentModel.DataAnnotations;
using Core.Models;

namespace WetHands.Core.Basic
{
  public class City : BaseEntity
  {
    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    public int CountryId { get; set; }
  }
}


