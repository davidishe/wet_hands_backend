using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Models;

namespace WetHands.Core.Models
{
  public class MassagePlace : BaseEntity
  {
    [Required]
    [MaxLength(256)]
    public string Name { get; set; }

    [MaxLength(128)]
    public string? Country { get; set; }

    [MaxLength(128)]
    public string? City { get; set; }

    /// <summary>
    /// Optional link to a catalog country row.
    /// </summary>
    public int? CountryId { get; set; }

    /// <summary>
    /// Optional link to a catalog city row.
    /// </summary>
    public int? CityId { get; set; }

    [Required]
    public string Description { get; set; }

    [Range(0, 100)]
    public int Rating { get; set; }

    /// <summary>
    /// Legacy field (was base64). Prefer <see cref="MassagePlaceImage"/> records.
    /// </summary>
    public string? MainImage { get; set; }

    public List<string> Gallery { get; set; } = new();

    public List<string> Attributes { get; set; } = new();
  }
}
