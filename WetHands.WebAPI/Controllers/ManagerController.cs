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
using WetHands.Core.Basic;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace WebAPI.Controllers
{
  public class ManagerController : BaseApiController
  {
    private readonly IDbRepository<MassagePlace> _massagePlaceRepository;
    private readonly IDbRepository<MassagePlaceImage> _massagePlaceImageRepository;
    private readonly IDbRepository<MassageCategory> _massageCategoryRepository;
    private readonly IDbRepository<Country> _countryRepository;
    private readonly IDbRepository<City> _cityRepository;
    private readonly ILogger<ManagerController> _logger;

    public ManagerController(
      IDbRepository<MassagePlace> massagePlaceRepository,
      IDbRepository<MassagePlaceImage> massagePlaceImageRepository,
      IDbRepository<MassageCategory> massageCategoryRepository,
      IDbRepository<Country> countryRepository,
      IDbRepository<City> cityRepository,
      ILogger<ManagerController> logger)
    {
      _massagePlaceRepository = massagePlaceRepository;
      _massagePlaceImageRepository = massagePlaceImageRepository;
      _massageCategoryRepository = massageCategoryRepository;
      _countryRepository = countryRepository;
      _cityRepository = cityRepository;
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

      // Reject legacy base64 image payloads. Use multipart endpoints instead.
      if (!string.IsNullOrWhiteSpace(request.MainImage) ||
          (request.Gallery != null && request.Gallery.Any(x => !string.IsNullOrWhiteSpace(x))))
      {
        return BadRequest("Images must be uploaded as files via POST /api/manager/catalog/{id}/images (multipart/form-data).");
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

      var resolved = await ResolveCountryCityAsync(request, cancellationToken);

      var (normalizedAttributes, attributesError) =
        await ValidateAndNormalizeAttributesAsync(request.Attributes, cancellationToken);
      if (!string.IsNullOrWhiteSpace(attributesError))
      {
        return BadRequest(attributesError);
      }

      var entity = new MassagePlace
      {
        Name = normalizedName,
        CountryId = resolved.CountryId,
        CityId = resolved.CityId,
        Country = resolved.CountryName,
        City = resolved.CityName,
        Description = request.Description.Trim(),
        Rating = Math.Clamp(request.Rating, 0, 100),
        // Images are stored in MassagePlaceImages, not in legacy base64 fields.
        MainImage = null,
        Gallery = new List<string>(),
        Attributes = normalizedAttributes
      };

      try
      {
        var created = await _massagePlaceRepository.AddAsync(entity);
        return CreatedAtAction(
          nameof(MassagePlacesController.GetDetails),
          "MassagePlaces",
          new { name = created.Name },
          await ToDtoAsync(created.Id, cancellationToken));
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

      // Reject legacy base64 image payloads. Use multipart endpoints instead.
      if (!string.IsNullOrWhiteSpace(request.MainImage) ||
          (request.Gallery != null && request.Gallery.Any(x => !string.IsNullOrWhiteSpace(x))))
      {
        return BadRequest("Images must be uploaded as files via POST /api/manager/catalog/{id}/images (multipart/form-data).");
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

      var resolved = await ResolveCountryCityAsync(request, cancellationToken);

      var (normalizedAttributes, attributesError) =
        await ValidateAndNormalizeAttributesAsync(request.Attributes, cancellationToken);
      if (!string.IsNullOrWhiteSpace(attributesError))
      {
        return BadRequest(attributesError);
      }

      existing.Name = normalizedName;
      existing.CountryId = resolved.CountryId;
      existing.CityId = resolved.CityId;
      existing.Country = resolved.CountryName;
      existing.City = resolved.CityName;
      existing.Description = request.Description.Trim();
      existing.Rating = Math.Clamp(request.Rating, 0, 100);
      // Images are stored in MassagePlaceImages, not in legacy base64 fields.
      existing.Attributes = normalizedAttributes;

      try
      {
        await _massagePlaceRepository.UpdateAsync(existing);
        return Ok(await ToDtoAsync(existing.Id, cancellationToken));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to update massage place with id {Id}", id);
        return StatusCode(500, "Unable to update massage place.");
      }
    }

    [HttpPost("catalog/{id:int}/images")]
    public async Task<ActionResult<MassagePlaceDto>> UploadImages(
      [FromRoute] int id,
      CancellationToken cancellationToken = default)
    {
      var place = await _massagePlaceRepository.GetByIdAsync(id);
      if (place == null) return NotFound();

      var files = Request.Form.Files;
      if (files == null || files.Count == 0)
      {
        return BadRequest("No files uploaded.");
      }

      var created = new List<MassagePlaceImage>();

      foreach (var file in files)
      {
        if (file.Length <= 0) continue;
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);
        var bytes = memoryStream.ToArray();
        var img = new MassagePlaceImage
        {
          MassagePlaceId = id,
          IsMain = false,
          EnrolledDate = DateTime.UtcNow,
          FileName = file.FileName,
          FileType = file.ContentType,
          DocByte = bytes,
          Size = bytes.Length
        };
        var createdImg = await _massagePlaceImageRepository.AddAsync(img);
        created.Add(createdImg);
      }

      // Ensure there is a main image.
      var existingImages = await _massagePlaceImageRepository
        .GetAll()
        .Where(x => x.MassagePlaceId == id)
        .ToListAsync(cancellationToken);

      var hasMain = existingImages.Any(x => x.IsMain);

      int? mainIndex = null;
      if (Request.Form.TryGetValue("mainIndex", out var mainIndexRaw))
      {
        if (int.TryParse(mainIndexRaw.ToString(), out var idx))
        {
          mainIndex = idx;
        }
      }

      if (mainIndex.HasValue && mainIndex.Value >= 0 && mainIndex.Value < created.Count)
      {
        await SetMainInternalAsync(id, created[mainIndex.Value].Id, cancellationToken);
      }
      else if (!hasMain && existingImages.Count > 0)
      {
        await SetMainInternalAsync(id, existingImages[0].Id, cancellationToken);
      }

      return Ok(await ToDtoAsync(id, cancellationToken));
    }

    [HttpPut("catalog/{placeId:int}/images/{imageId:int}/main")]
    public async Task<ActionResult<MassagePlaceDto>> SetMainImage(
      [FromRoute] int placeId,
      [FromRoute] int imageId,
      CancellationToken cancellationToken = default)
    {
      var place = await _massagePlaceRepository.GetByIdAsync(placeId);
      if (place == null) return NotFound();

      var img = await _massagePlaceImageRepository
        .GetAll()
        .FirstOrDefaultAsync(x => x.Id == imageId && x.MassagePlaceId == placeId, cancellationToken);
      if (img == null) return NotFound();

      await SetMainInternalAsync(placeId, imageId, cancellationToken);
      return Ok(await ToDtoAsync(placeId, cancellationToken));
    }

    [HttpDelete("catalog/{placeId:int}/images/{imageId:int}")]
    public async Task<ActionResult<MassagePlaceDto>> DeleteImage(
      [FromRoute] int placeId,
      [FromRoute] int imageId,
      CancellationToken cancellationToken = default)
    {
      var img = await _massagePlaceImageRepository
        .GetAll()
        .FirstOrDefaultAsync(x => x.Id == imageId && x.MassagePlaceId == placeId, cancellationToken);
      if (img == null) return NotFound();

      await _massagePlaceImageRepository.DeleteAsync(img);

      // If main deleted, pick another as main.
      var remaining = await _massagePlaceImageRepository
        .GetAll()
        .Where(x => x.MassagePlaceId == placeId)
        .OrderByDescending(x => x.EnrolledDate)
        .ToListAsync(cancellationToken);
      if (remaining.Count > 0 && !remaining.Any(x => x.IsMain))
      {
        await SetMainInternalAsync(placeId, remaining[0].Id, cancellationToken);
      }

      return Ok(await ToDtoAsync(placeId, cancellationToken));
    }

    private async Task SetMainInternalAsync(int placeId, int imageId, CancellationToken cancellationToken)
    {
      var imgs = await _massagePlaceImageRepository
        .GetAll()
        .Where(x => x.MassagePlaceId == placeId)
        .ToListAsync(cancellationToken);

      foreach (var img in imgs)
      {
        var desired = img.Id == imageId;
        if (img.IsMain != desired)
        {
          img.IsMain = desired;
          await _massagePlaceImageRepository.UpdateAsync(img);
        }
      }
    }

    private static string BuildImageUrl(int imageId) => $"/api/MassagePlaces/images/{imageId}";

    private async Task<MassagePlaceDto> ToDtoAsync(int placeId, CancellationToken cancellationToken)
    {
      var place = await _massagePlaceRepository
        .GetAll()
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == placeId, cancellationToken);

      if (place == null)
      {
        return new MassagePlaceDto
        {
          Id = 0,
          Name = string.Empty,
          CountryId = null,
          CityId = null,
          Country = "Russia",
          City = string.Empty,
          Description = string.Empty,
          Rating = 0,
          MainImage = string.Empty,
          Gallery = Array.Empty<string>(),
          Attributes = Array.Empty<string>()
        };
      }

      var countries = await _countryRepository.GetAll().AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name, cancellationToken);
      var cities = await _cityRepository.GetAll().AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name, cancellationToken);

      var effectiveCountry = place.CountryId.HasValue && countries.TryGetValue(place.CountryId.Value, out var ctryName)
        ? ctryName
        : (string.IsNullOrWhiteSpace(place.Country) ? "Russia" : place.Country);

      var effectiveCity = place.CityId.HasValue && cities.TryGetValue(place.CityId.Value, out var cityName)
        ? cityName
        : place.City;

      var imgs = await _massagePlaceImageRepository
        .GetAll()
        .AsNoTracking()
        .Where(x => x.MassagePlaceId == placeId)
        .OrderByDescending(x => x.IsMain)
        .ThenByDescending(x => x.EnrolledDate)
        .ToListAsync(cancellationToken);

      var mainId = imgs.FirstOrDefault(x => x.IsMain)?.Id;
      var galleryUrls = imgs.Where(x => !x.IsMain).Select(x => BuildImageUrl(x.Id)).ToArray();

      return new MassagePlaceDto
      {
        Id = place.Id,
        Name = place.Name,
        CountryId = place.CountryId,
        CityId = place.CityId,
        Country = effectiveCountry,
        City = effectiveCity,
        Description = place.Description,
        Rating = place.Rating,
        MainImage = mainId.HasValue ? BuildImageUrl(mainId.Value) : string.Empty,
        Gallery = galleryUrls,
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

    private async Task<(List<string> Normalized, string? Error)> ValidateAndNormalizeAttributesAsync(
      IEnumerable<string>? requested,
      CancellationToken cancellationToken)
    {
      var normalized = NormalizeList(requested);
      if (normalized.Count == 0) return (normalized, null);

      var allowed = await _massageCategoryRepository
        .GetAll()
        .AsNoTracking()
        .Where(x => x.IsActive)
        .Select(x => x.Name)
        .ToListAsync(cancellationToken);

      var allowedSet = new HashSet<string>(
        allowed
          .Where(x => !string.IsNullOrWhiteSpace(x))
          .Select(x => x.Trim()),
        StringComparer.OrdinalIgnoreCase);

      var unknown = normalized.Where(x => !allowedSet.Contains(x)).ToList();
      if (unknown.Count == 0) return (normalized, null);

      // Return a human friendly message so the client can show it to user.
      var msg =
        "Некоторые категории отсутствуют в справочнике и не могут быть сохранены: " +
        string.Join(", ", unknown) +
        ". Обратитесь к администратору, чтобы добавить их в справочник (MassageCategories).";

      return (normalized, msg);
    }

    public class MassagePlaceUpsertRequest
    {
      [Required]
      [MaxLength(256)]
      public required string Name { get; init; }

      public int? CountryId { get; init; }
      public int? CityId { get; init; }

      [MaxLength(128)]
      public string? Country { get; init; } // legacy

      [MaxLength(128)]
      public string? City { get; init; } // legacy

      [Required]
      public required string Description { get; init; }

      [Range(0, 100)]
      public int Rating { get; init; }

      // Legacy fields (base64) are rejected; upload images via multipart endpoints.
      public string? MainImage { get; init; }

      // Legacy fields (base64) are rejected; upload images via multipart endpoints.
      public IReadOnlyList<string>? Gallery { get; init; }

      public IReadOnlyList<string>? Attributes { get; init; }
    }

    private async Task<(int? CountryId, int? CityId, string? CountryName, string? CityName)> ResolveCountryCityAsync(
      MassagePlaceUpsertRequest request,
      CancellationToken cancellationToken)
    {
      int? countryId = request.CountryId;
      int? cityId = request.CityId;
      string? countryName = request.Country?.Trim();
      string? cityName = request.City?.Trim();

      if (countryId.HasValue)
      {
        var c = await _countryRepository.GetByIdAsync(countryId.Value);
        if (c != null)
        {
          countryName = c.Name;
        }
      }
      else if (!string.IsNullOrWhiteSpace(countryName))
      {
        var c = await _countryRepository
          .GetAll()
          .AsNoTracking()
          .FirstOrDefaultAsync(x => x.Name == countryName, cancellationToken);
        if (c != null)
        {
          countryId = c.Id;
          countryName = c.Name;
        }
      }

      if (cityId.HasValue)
      {
        var ct = await _cityRepository.GetByIdAsync(cityId.Value);
        if (ct != null)
        {
          cityName = ct.Name;
          // Align country if not provided.
          if (!countryId.HasValue)
          {
            countryId = ct.CountryId;
            var c = await _countryRepository.GetByIdAsync(countryId.Value);
            if (c != null) countryName = c.Name;
          }
        }
      }
      else if (!string.IsNullOrWhiteSpace(cityName))
      {
        var q = _cityRepository.GetAll().AsNoTracking().Where(x => x.Name == cityName);
        if (countryId.HasValue) q = q.Where(x => x.CountryId == countryId.Value);
        var ct = await q.FirstOrDefaultAsync(cancellationToken);
        if (ct != null)
        {
          cityId = ct.Id;
          cityName = ct.Name;
        }
      }

      return (countryId, cityId, countryName, cityName);
    }
  }
}
