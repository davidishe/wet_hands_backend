using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WetHands.Core.Models.Identity;
using WetHands.Core.Models.Options;

namespace WetHands.Infrastructure.Services.Sms
{
  public class MailjetSmsService : ISmsSenderService
  {

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly MailJetSmsOptions _options;
    private readonly MailJetCredentialsOptions _credentials;
    private readonly string _basicAuthorizationValue;
    private readonly ILogger<MailjetSmsService> _logger;

    public MailjetSmsService(
      IHttpClientFactory httpClientFactory,
      IOptions<MailJetSmsOptions> options,
      IOptions<MailJetCredentialsOptions> credentials,
      ILogger<MailjetSmsService> logger)
    {
      _httpClientFactory = httpClientFactory;
      _logger = logger;
      _options = options.Value;
      _credentials = credentials.Value;

      if (string.IsNullOrWhiteSpace(_options.ApiToken) || string.IsNullOrWhiteSpace(_options.From))
      {
        throw new InvalidOperationException("MailJet SMS options must be configured.");
      }

      if (string.IsNullOrWhiteSpace(_credentials.MailJetApiKey) || string.IsNullOrWhiteSpace(_credentials.MailJetApiSecret))
      {
        throw new InvalidOperationException("MailJet credentials must be configured.");
      }

      _basicAuthorizationValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_credentials.MailJetApiKey}:{_credentials.MailJetApiSecret}"));
    }

    public async Task SendOneTimeCodeAsync(string phoneNumber, OneTimeCode code, string? template = null, CancellationToken cancellationToken = default)
    {
      if (string.IsNullOrWhiteSpace(phoneNumber))
        throw new ArgumentException("Phone number is required.", nameof(phoneNumber));

      if (code is null)
        throw new ArgumentNullException(nameof(code));

      var message = template ?? $"Ваш код: {code.Code}. Действителен до {code.ExpiresAtUtc:yyyy-MM-dd HH:mm} UTC.";

      var payload = new
      {
        From = _options.From,
        To = phoneNumber,
        Text = message
      };

      var httpClient = _httpClientFactory.CreateClient("MailjetSms");
      using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mailjet.com/v4/sms-send")
      {
        Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
      };

      Console.WriteLine(request.RequestUri);
      Console.WriteLine(request.RequestUri);
      Console.WriteLine(request.RequestUri);
      Console.WriteLine(request.RequestUri);

      request.Headers.TryAddWithoutValidation("Authorization", $"Basic {_basicAuthorizationValue}");
      request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_options.ApiToken}");

      _logger.LogDebug("Sending SMS with one-time code to {Phone}.", phoneNumber);

      var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
      if (!response.IsSuccessStatusCode)
      {
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogError("Failed to send SMS via Mailjet. Status: {StatusCode}. Body: {Body}", response.StatusCode, responseBody);
        throw new InvalidOperationException($"Failed to send SMS via Mailjet. Status code: {response.StatusCode}.");
      }

      _logger.LogInformation("SMS with one-time code sent to {Phone}. Expires at {ExpiresAt}.", phoneNumber, code.ExpiresAtUtc.ToString("o", CultureInfo.InvariantCulture));
    }
  }
}
