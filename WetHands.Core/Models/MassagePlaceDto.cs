using System;
using System.Collections.Generic;

namespace WetHands.Core.Models
{
  /// <summary>
  /// DTO that describes a massage place card for the Flutter catalog.
  /// </summary>
  public class MassagePlaceDto
  {
    public MassagePlaceDto()
    {
      Gallery = Array.Empty<string>();
      Attributes = Array.Empty<string>();
    }

    public required int Id { get; init; }
    public required string Name { get; init; }
    public int? CountryId { get; init; }
    public int? CityId { get; init; }
    public string? Country { get; init; }
    public string? City { get; init; }
    public required string Description { get; init; }
    /// <summary>Rating from 0 to 100.</summary>
    public int Rating { get; init; }
    /// <summary>
    /// Main image reference (relative/absolute URL).
    /// </summary>
    public required string MainImage { get; init; }
    public required IReadOnlyList<string> Gallery { get; init; }
    public required IReadOnlyList<string> Attributes { get; init; }
  }
}
