using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WetHands.Core.Models;
using WetHands.Infrastructure.Database;

namespace WebAPI.Controllers
{
  public class ManagerController : BaseApiController
  {
    private readonly IDbRepository<MassagePlace> _massagePlaceRepository;
    private readonly ILogger<ManagerController> _logger;

    public ManagerController(
      IDbRepository<MassagePlace> massagePlaceRepository,
      ILogger<ManagerController> logger)
    {
      _massagePlaceRepository = massagePlaceRepository;
      _logger = logger;
    }

    [HttpPost("catalog")]
    public async Task<ActionResult<MassagePlaceDto>> CreateMassagePlace(
      [FromBody] MassagePlaceUpsertRequest request,
      CancellationToken cancellationToken = default)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var normalizedName = request.Name.Trim();
      var exists = await _massagePlaceRepository
        .GetAll()
        .AsNoTracking()
        .AnyAsync(x => x.Name == normalizedName, cancellationToken);

      if (exists)
      {
        return Conflict($"Massage place with name '{normalizedName}' already exists.");
      }

      var entity = new MassagePlace
      {
        Name = normalizedName,
        Country = request.Country?.Trim(),
        City = request.City?.Trim(),
        Description = request.Description.Trim(),
        Rating = Math.Clamp(request.Rating, 0, 100),
        MainImage = request.MainImage,
        Gallery = NormalizeList(request.Gallery),
        Attributes = NormalizeList(request.Attributes)
      };

      try
      {
        var created = await _massagePlaceRepository.AddAsync(entity);
        return CreatedAtAction(
          nameof(MassagePlacesController.GetDetails),
          "MassagePlaces",
          new { name = created.Name },
          ToDto(created));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to create massage place");
        return StatusCode(500, "Unable to create massage place.");
      }
    }

    [HttpPut("catalog/{id:int}")]
    public async Task<ActionResult<MassagePlaceDto>> UpdateMassagePlace(
      [FromRoute] int id,
      [FromBody] MassagePlaceUpsertRequest request,
      CancellationToken cancellationToken = default)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var existing = await _massagePlaceRepository.GetByIdAsync(id);
      if (existing == null)
      {
        return NotFound();
      }

      var normalizedName = request.Name.Trim();
      var duplicateExists = await _massagePlaceRepository
        .GetAll()
        .AsNoTracking()
        .AnyAsync(x => x.Id != id && x.Name == normalizedName, cancellationToken);

      if (duplicateExists)
      {
        return Conflict($"Massage place with name '{normalizedName}' already exists.");
      }

      existing.Name = normalizedName;
      existing.Country = request.Country?.Trim();
      existing.City = request.City?.Trim();
      existing.Description = request.Description.Trim();
      existing.Rating = Math.Clamp(request.Rating, 0, 100);
      existing.MainImage = request.MainImage;
      existing.Gallery = NormalizeList(request.Gallery);
      existing.Attributes = NormalizeList(request.Attributes);

      try
      {
        await _massagePlaceRepository.UpdateAsync(existing);
        return Ok(ToDto(existing));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to update massage place with id {Id}", id);
        return StatusCode(500, "Unable to update massage place.");
      }
    }

    private static MassagePlaceDto ToDto(MassagePlace place)
    {
      var effectiveCountry = string.IsNullOrWhiteSpace(place.Country) ? "Russia" : place.Country;
      return new MassagePlaceDto
      {
        Name = place.Name,
        Country = effectiveCountry,
        City = place.City,
        Description = place.Description,
        Rating = place.Rating,
        MainImage = place.MainImage,
        Gallery = place.Gallery ?? new List<string>(),
        Attributes = place.Attributes ?? new List<string>()
      };
    }

    private static List<string> NormalizeList(IEnumerable<string>? source)
    {
      return source?
        .Where(s => !string.IsNullOrWhiteSpace(s))
        .Select(s => s.Trim())
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToList() ?? new List<string>();
    }

    public class MassagePlaceUpsertRequest
    {
      [Required]
      [MaxLength(256)]
      public required string Name { get; init; }

      [MaxLength(128)]
      public string? Country { get; init; }

      [MaxLength(128)]
      public string? City { get; init; }

      [Required]
      public required string Description { get; init; }

      [Range(0, 100)]
      public int Rating { get; init; }

      [Required]
      public required string MainImage { get; init; }

      public IReadOnlyList<string>? Gallery { get; init; }

      public IReadOnlyList<string>? Attributes { get; init; }
    }
  }
}
