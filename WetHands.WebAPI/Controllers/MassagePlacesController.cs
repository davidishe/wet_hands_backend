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
          return Ok(found);
        }

        return Ok(new MassagePlaceDto
        {
          Name = found.Name,
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
    public ActionResult<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetCategories()
    {
      return Ok(MassageCategories);
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
  }
}
