using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WetHands.Core.Models;
using WetHands.Core.Responses;

namespace WebAPI.Controllers
{
  [AllowAnonymous]
  public class MassagePlacesController : BaseApiController
  {
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<MassagePlacesController> _logger;
    private readonly string _catalogFilePath;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
      PropertyNameCaseInsensitive = true,
      ReadCommentHandling = JsonCommentHandling.Skip
    };

    private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> MassageCategories =
      new Dictionary<string, IReadOnlyList<string>>
      {
        ["Классические и лечебные"] = new[]
        {
          "Классический массаж",
          "Лечебный массаж",
          "Профилактический массаж",
          "Общеукрепляющий массаж",
          "Локальный (зональный) массаж",
          "Сегментарный массаж",
          "Периостальный массаж",
          "Рефлекторно-сегментарный массаж",
          "Соединительнотканный массаж",
          "Миофасциальный релиз",
          "Триггерный массаж",
          "Посттравматический массаж",
          "Реабилитационный массаж"
        },
        ["Медицинские (по показаниям)"] = new[]
        {
          "Ортопедический массаж",
          "Неврологический массаж",
          "Кардиологический массаж",
          "Пульмонологический массаж",
          "Висцеральный массаж",
          "Урологический массаж",
          "Гинекологический массаж",
          "Логопедический массаж",
          "Детский лечебный массаж",
          "Массаж для беременных (медицинский протокол)"
        },
        ["Спортивные"] = new[]
        {
          "Спортивный массаж",
          "Предтренировочный",
          "Послетренировочный",
          "Восстановительный",
          "Тонизирующий",
          "Расслабляющий спортивный",
          "Массаж при мышечных спазмах",
          "Массаж для бегунов",
          "Массаж для силовых тренировок"
        },
        ["Восточные и традиционные практики"] = new[]
        {
          "Тайский массаж",
          "Балийский массаж",
          "Китайский массаж (Туина)",
          "Шиацу",
          "Японский массаж Амма",
          "Индийский массаж (Абхьянга)",
          "Аюрведический массаж",
          "Ломи-ломи (гавайский)",
          "Тибетский массаж",
          "Кореанский массаж",
          "Бирманский массаж"
        },
        ["Расслабляющие и SPA"] = new[]
        {
          "Релакс-массаж",
          "Антистресс-массаж",
          "Аромамассаж",
          "Масляный массаж",
          "Свечной массаж",
          "Шоколадный массаж",
          "Медовый массаж",
          "Винный массаж",
          "Массаж с камнями (стоун-терапия)",
          "Тепловой массаж",
          "Холодный массаж (крио-элементы)"
        },
        ["Косметические и эстетические"] = new[]
        {
          "Косметический массаж лица",
          "Буккальный массаж",
          "Скульптурный массаж лица",
          "Пластический массаж",
          "Лифтинг-массаж",
          "Антивозрастной массаж",
          "Лимфодренажный массаж",
          "Антицеллюлитный массаж",
          "Моделирующий массаж",
          "Массаж шеи и декольте"
        },
        ["Лимфодренаж и коррекция фигуры"] = new[]
        {
          "Ручной лимфодренаж",
          "Аппаратный лимфодренаж",
          "Вакуумный массаж",
          "LPG-массаж",
          "Баночный массаж",
          "Массаж для похудения",
          "Коррекционный массаж",
          "Детокс-массаж"
        },
        ["Рефлексотерапия"] = new[]
        {
          "Рефлекторный массаж",
          "Точечный массаж",
          "Акупрессура",
          "Су-джок",
          "Массаж стоп",
          "Плантарный массаж",
          "Аурикулярный массаж"
        },
        ["Аппаратные виды"] = new[]
        {
          "Вибромассаж",
          "Ультразвуковой массаж",
          "Вакуумно-роликовый массаж",
          "Гидромассаж",
          "Подводный душ-массаж",
          "Прессотерапия",
          "Электромиостимуляция",
          "Пневмомассаж"
        },
        ["Специальные категории"] = new[]
        {
          "Детский массаж",
          "Массаж для новорождённых",
          "Массаж для пожилых",
          "Офисный массаж",
          "Экспресс-массаж",
          "Корпоративный массаж",
          "Домашний массаж",
          "Психосоматический массаж",
          "Энергетический массаж"
        },
        ["Альтернативные и энергетические"] = new[]
        {
          "Энергетический массаж",
          "Рэйки-массаж",
          "Биоэнергетический массаж",
          "Чакральный массаж",
          "Краниосакральная терапия",
          "Остеопатические техники"
        }
      };

    public MassagePlacesController(
      IWebHostEnvironment environment,
      ILogger<MassagePlacesController> logger)
    {
      _environment = environment;
      _logger = logger;
      _catalogFilePath = Path.Combine(_environment.ContentRootPath, "Data", "massage_places.json");
    }

    [HttpGet]
    [Route("")]
    [Route("catalog")]
    public async Task<ActionResult<IReadOnlyList<MassagePlaceDto>>> GetCatalog(
      [FromQuery] string? q = null,
      [FromQuery] string? categories = null,
      [FromQuery] string? country = null,
      [FromQuery] string? city = null,
      [FromQuery] int? minRating = null,
      [FromQuery] int? maxRating = null,
      [FromQuery] int offset = 0,
      [FromQuery] int limit = 200,
      [FromQuery] bool includeMainImage = true,
      [FromQuery] bool includeGallery = true,
      CancellationToken cancellationToken = default)
    {
      if (!System.IO.File.Exists(_catalogFilePath))
      {
        _logger.LogWarning("Massage catalog file {FilePath} not found", _catalogFilePath);
        return Ok(Array.Empty<MassagePlaceDto>());
      }

      try
      {
        await using var stream = System.IO.File.OpenRead(_catalogFilePath);
        var places = await JsonSerializer.DeserializeAsync<List<MassagePlaceDto>>(stream, SerializerOptions, cancellationToken)
          ?? new List<MassagePlaceDto>();

        // Default country for current dataset (no explicit country field in JSON).
        for (var i = 0; i < places.Count; i++)
        {
          var p = places[i];
          if (p != null && string.IsNullOrWhiteSpace(p.Country))
          {
            // All seed data cities are in Russia for now.
            places[i] = new MassagePlaceDto
            {
              Name = p.Name,
              Country = "Russia",
              City = p.City,
              Description = p.Description,
              Rating = p.Rating,
              MainImage = p.MainImage,
              Gallery = p.Gallery,
              Attributes = p.Attributes
            };
          }
        }

        // Server-side filters.
        var selectedCategories = ParseCsv(categories);
        var countryFilter = Normalize(country);
        var cityFilter = Normalize(city);

        if (minRating.HasValue)
        {
          minRating = Math.Clamp(minRating.Value, 0, 100);
        }
        if (maxRating.HasValue)
        {
          maxRating = Math.Clamp(maxRating.Value, 0, 100);
        }

        if (selectedCategories.Count > 0 ||
            !string.IsNullOrWhiteSpace(countryFilter) ||
            !string.IsNullOrWhiteSpace(cityFilter) ||
            minRating.HasValue ||
            maxRating.HasValue)
        {
          places = places
            .Where(p => MatchesFilters(
              p,
              selectedCategories,
              countryFilter,
              cityFilter,
              minRating,
              maxRating))
            .ToList();
        }

        // Server-side search.
        var query = (q ?? string.Empty).Trim();
        if (query.Length > 0)
        {
          places = places
            .Where(p => MatchesQuery(p, query))
            .OrderByDescending(p => p.Rating)
            .ToList();
        }

        // Server-side paging (useful for search).
        if (offset < 0) offset = 0;
        if (limit <= 0) limit = 200;
        limit = Math.Min(limit, 500);
        if (offset > 0 || limit < places.Count)
        {
          places = places.Skip(offset).Take(limit).ToList();
        }

        if (includeMainImage && includeGallery)
        {
          return Ok(places);
        }

        var projected = new List<MassagePlaceDto>(places.Count);
        foreach (var place in places)
        {
          projected.Add(new MassagePlaceDto
          {
            Name = place.Name ?? string.Empty,
            Country = place.Country,
            City = place.City,
            Description = place.Description ?? string.Empty,
            Rating = place.Rating,
            MainImage = includeMainImage ? (place.MainImage ?? string.Empty) : string.Empty,
            Gallery = includeGallery ? (place.Gallery ?? Array.Empty<string>()) : Array.Empty<string>(),
            Attributes = place.Attributes ?? Array.Empty<string>()
          });
        }

        return Ok(projected);
      }
      catch (JsonException ex)
      {
        _logger.LogError(ex, "Invalid massage catalog JSON in {FilePath}", _catalogFilePath);
        return StatusCode(500, new ApiResponse(500, "Massage catalog JSON is invalid."));
      }
      catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
      {
        _logger.LogError(ex, "Unable to read massage catalog file {FilePath}", _catalogFilePath);
        return StatusCode(500, new ApiResponse(500, "Unable to read massage catalog file."));
      }
    }

    /// <summary>
    /// Returns a single massage place by name (case-insensitive match).
    /// Useful to lazy-load heavy fields (gallery/images) only when needed.
    /// </summary>
    [HttpGet("details")]
    public async Task<ActionResult<MassagePlaceDto>> GetDetails(
      [FromQuery] string name,
      [FromQuery] bool includeMainImage = true,
      [FromQuery] bool includeGallery = true,
      CancellationToken cancellationToken = default)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return BadRequest(new ApiResponse(400, "Parameter 'name' is required."));
      }

      if (!System.IO.File.Exists(_catalogFilePath))
      {
        _logger.LogWarning("Massage catalog file {FilePath} not found", _catalogFilePath);
        return NotFound(new ApiResponse(404, "Not found."));
      }

      try
      {
        await using var stream = System.IO.File.OpenRead(_catalogFilePath);
        var places = await JsonSerializer.DeserializeAsync<List<MassagePlaceDto>>(stream, SerializerOptions, cancellationToken)
          ?? new List<MassagePlaceDto>();

        var found = places.Find(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
        if (found == null)
        {
          return NotFound(new ApiResponse(404, "Not found."));
        }

        if (includeMainImage && includeGallery)
        {
          if (string.IsNullOrWhiteSpace(found.Country))
          {
            found = new MassagePlaceDto
            {
              Name = found.Name,
              Country = "Russia",
              City = found.City,
              Description = found.Description,
              Rating = found.Rating,
              MainImage = found.MainImage,
              Gallery = found.Gallery,
              Attributes = found.Attributes
            };
          }
          return Ok(found);
        }

        return Ok(new MassagePlaceDto
        {
          Name = found.Name,
          Country = string.IsNullOrWhiteSpace(found.Country) ? "Russia" : found.Country,
          City = found.City,
          Description = found.Description,
          Rating = found.Rating,
          MainImage = includeMainImage ? found.MainImage : string.Empty,
          Gallery = includeGallery ? found.Gallery : Array.Empty<string>(),
          Attributes = found.Attributes
        });
      }
      catch (JsonException ex)
      {
        _logger.LogError(ex, "Invalid massage catalog JSON in {FilePath}", _catalogFilePath);
        return StatusCode(500, new ApiResponse(500, "Massage catalog JSON is invalid."));
      }
      catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
      {
        _logger.LogError(ex, "Unable to read massage catalog file {FilePath}", _catalogFilePath);
        return StatusCode(500, new ApiResponse(500, "Unable to read massage catalog file."));
      }
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>> GetCategories(
      CancellationToken cancellationToken = default)
    {
      var places = await LoadPlacesOrEmpty(cancellationToken);
      var availableTypes = ExtractAvailableAttributes(places);

      if (availableTypes.Count == 0)
      {
        return Ok(new Dictionary<string, IReadOnlyList<string>>
        {
          ["Другое"] = Array.Empty<string>()
        });
      }

      // Filter predefined groups to only types that are actually present in the catalog.
      var covered = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      var result = new Dictionary<string, IReadOnlyList<string>>();

      foreach (var group in MassageCategories)
      {
        var filtered = group.Value
          .Where(t => availableTypes.ContainsKey(NormalizeKey(t)))
          .Select(NormalizeLabel)
          .Where(t => !string.IsNullOrWhiteSpace(t))
          .Distinct(StringComparer.OrdinalIgnoreCase)
          .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
          .ToArray();

        if (filtered.Length == 0) continue;

        foreach (var t in filtered) covered.Add(NormalizeKey(t));
        result[group.Key] = filtered;
      }

      // Everything else goes to "Other".
      var other = availableTypes
        .Keys
        .Where(k => !covered.Contains(k))
        .Select(k => availableTypes[k])
        .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
        .ToArray();

      if (other.Length > 0 || result.Count == 0)
      {
        result["Другое"] = other;
      }

      return Ok(result);
    }

    [HttpGet("filterOptions")]
    public async Task<ActionResult<MassagePlaceFilterOptionsResponse>> GetFilterOptions(
      CancellationToken cancellationToken = default)
    {
      if (!System.IO.File.Exists(_catalogFilePath))
      {
        _logger.LogWarning("Massage catalog file {FilePath} not found", _catalogFilePath);
        return Ok(new MassagePlaceFilterOptionsResponse
        {
          Countries = new[] { "Russia" },
          Cities = Array.Empty<string>(),
          Categories = Array.Empty<string>(),
          MinRating = 0,
          MaxRating = 100
        });
      }

      try
      {
        var places = await LoadPlacesOrEmpty(cancellationToken);

        var countries = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var cities = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var min = 100;
        var max = 0;

        foreach (var p in places)
        {
          if (p == null) continue;
          var ctry = string.IsNullOrWhiteSpace(p.Country) ? "Russia" : p.Country.Trim();
          if (ctry.Length > 0) countries.Add(ctry);

          if (!string.IsNullOrWhiteSpace(p.City)) cities.Add(p.City.Trim());

          min = Math.Min(min, p.Rating);
          max = Math.Max(max, p.Rating);

          var attrs = p.Attributes;
          if (attrs != null)
          {
            foreach (var a in attrs)
            {
              var label = NormalizeLabel(a);
              if (!string.IsNullOrWhiteSpace(label)) categories.Add(label);
            }
          }
        }

        var countriesArr = countries.Count == 0 ? new[] { "Russia" } : countries.OrderBy(x => x).ToArray();
        var citiesArr = cities.OrderBy(x => x).ToArray();
        var categoriesArr = categories.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();

        return Ok(new MassagePlaceFilterOptionsResponse
        {
          Countries = countriesArr,
          Cities = citiesArr,
          Categories = categoriesArr,
          MinRating = Math.Clamp(min, 0, 100),
          MaxRating = Math.Clamp(max, 0, 100)
        });
      }
      catch (JsonException ex)
      {
        _logger.LogError(ex, "Invalid massage catalog JSON in {FilePath}", _catalogFilePath);
        return StatusCode(500, new ApiResponse(500, "Massage catalog JSON is invalid."));
      }
      catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
      {
        _logger.LogError(ex, "Unable to read massage catalog file {FilePath}", _catalogFilePath);
        return StatusCode(500, new ApiResponse(500, "Unable to read massage catalog file."));
      }
    }

    private async Task<List<MassagePlaceDto>> LoadPlacesOrEmpty(CancellationToken cancellationToken)
    {
      if (!System.IO.File.Exists(_catalogFilePath)) return new List<MassagePlaceDto>();

      await using var stream = System.IO.File.OpenRead(_catalogFilePath);
      return await JsonSerializer.DeserializeAsync<List<MassagePlaceDto>>(stream, SerializerOptions, cancellationToken)
        ?? new List<MassagePlaceDto>();
    }

    private static string NormalizeKey(string? value)
    {
      return (value ?? string.Empty).Trim();
    }

    private static string NormalizeLabel(string? value)
    {
      return (value ?? string.Empty).Trim();
    }

    private static Dictionary<string, string> ExtractAvailableAttributes(IEnumerable<MassagePlaceDto> places)
    {
      // key => canonical label
      var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
      foreach (var p in places)
      {
        if (p?.Attributes == null) continue;
        foreach (var raw in p.Attributes)
        {
          var label = NormalizeLabel(raw);
          if (string.IsNullOrWhiteSpace(label)) continue;
          var key = NormalizeKey(label);
          map.TryAdd(key, label);
        }
      }
      return map;
    }

    private static bool MatchesQuery(MassagePlaceDto place, string q)
    {
      if (place == null) return false;
      if (string.IsNullOrWhiteSpace(q)) return true;

      if (!string.IsNullOrWhiteSpace(place.Name) &&
          place.Name.Contains(q, StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }

      if (!string.IsNullOrWhiteSpace(place.City) &&
          place.City.Contains(q, StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }

      if (!string.IsNullOrWhiteSpace(place.Country) &&
          place.Country.Contains(q, StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }

      if (!string.IsNullOrWhiteSpace(place.Description) &&
          place.Description.Contains(q, StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }

      var attrs = place.Attributes;
      if (attrs != null)
      {
        foreach (var attr in attrs)
        {
          if (string.IsNullOrWhiteSpace(attr)) continue;
          if (attr.Contains(q, StringComparison.OrdinalIgnoreCase)) return true;
        }
      }

      return false;
    }

    private static bool MatchesFilters(
      MassagePlaceDto place,
      IReadOnlyList<string> selectedCategories,
      string? country,
      string? city,
      int? minRating,
      int? maxRating)
    {
      if (place == null) return false;

      if (!string.IsNullOrWhiteSpace(country))
      {
        var effectiveCountry = string.IsNullOrWhiteSpace(place.Country) ? "Russia" : place.Country;
        if (effectiveCountry == null || !effectiveCountry.Contains(country, StringComparison.OrdinalIgnoreCase))
        {
          return false;
        }
      }

      if (!string.IsNullOrWhiteSpace(city))
      {
        if (string.IsNullOrWhiteSpace(place.City) ||
            !place.City.Contains(city, StringComparison.OrdinalIgnoreCase))
        {
          return false;
        }
      }

      if (minRating.HasValue && place.Rating < minRating.Value) return false;
      if (maxRating.HasValue && place.Rating > maxRating.Value) return false;

      if (selectedCategories.Count > 0)
      {
        // Flexible matching:
        // - If an item matches a category name (from `MassageCategories`), treat it as a category filter.
        // - Otherwise treat it as a concrete service/type (i.e. a place attribute).
        var attrs = place.Attributes ?? Array.Empty<string>();
        var hasAny = false;

        foreach (var item in selectedCategories)
        {
          if (MassageCategories.TryGetValue(item, out var types) && types != null)
          {
            foreach (var attr in attrs)
            {
              if (string.IsNullOrWhiteSpace(attr)) continue;
              // Exact match (trim + ignore case) to avoid accidental substring matches.
              if (types.Any(t => string.Equals(NormalizeLabel(t), NormalizeLabel(attr), StringComparison.OrdinalIgnoreCase)))
              {
                hasAny = true;
                break;
              }
            }
          }
          else
          {
            // Exact match against attributes (service/type).
            foreach (var attr in attrs)
            {
              if (string.IsNullOrWhiteSpace(attr)) continue;
              if (string.Equals(NormalizeLabel(attr), NormalizeLabel(item), StringComparison.OrdinalIgnoreCase))
              {
                hasAny = true;
                break;
              }
            }
          }

          if (hasAny) break;
        }

        if (!hasAny) return false;
      }

      return true;
    }

    private static List<string> ParseCsv(string? value)
    {
      if (string.IsNullOrWhiteSpace(value)) return new List<string>();
      return value
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToList();
    }

    private static string? Normalize(string? value)
    {
      var trimmed = value?.Trim();
      return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    public class MassagePlaceFilterOptionsResponse
    {
      public required IReadOnlyList<string> Countries { get; init; }
      public required IReadOnlyList<string> Cities { get; init; }
      public required IReadOnlyList<string> Categories { get; init; }
      public required int MinRating { get; init; }
      public required int MaxRating { get; init; }
    }
  }
}
