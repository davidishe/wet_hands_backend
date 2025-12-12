using System;
using System.Collections.Generic;
using System.IO;
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
    public async Task<ActionResult<IReadOnlyList<MassagePlaceDto>>> GetCatalog(CancellationToken cancellationToken)
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

        return Ok(places);
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
  }
}
