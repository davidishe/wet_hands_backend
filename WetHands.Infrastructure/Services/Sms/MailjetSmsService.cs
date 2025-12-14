using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WetHands.Core.Models.Identity;
using WetHands.Email.EmailService;
using WetHands.Email.Models;

namespace WetHands.Infrastructure.Services.Sms
{
  public class MailjetSmsService : ISmsSenderService
  {
    private readonly IEmailService _emailService;
    private readonly ILogger<MailjetSmsService> _logger;
    private readonly string _mailFrom;
    private readonly string _appName;

    public MailjetSmsService(
      IEmailService emailService,
      IConfiguration configuration,
      ILogger<MailjetSmsService> logger)
    {
      _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      _mailFrom = configuration.GetValue<string>("AppSettings:EmailDomain")
                 ?? throw new InvalidOperationException("AppSettings:EmailDomain must be configured.");

      var configuredAppName = configuration.GetValue<string>("AppSettings:AppName");
      _appName = string.IsNullOrWhiteSpace(configuredAppName)
        ? "WetHands"
        : configuredAppName.Trim().Trim('\'');
    }

    public async Task SendOneTimeCodeAsync(string email, OneTimeCode code, string? langCode = null, CancellationToken cancellationToken = default)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (string.IsNullOrWhiteSpace(email))
        throw new ArgumentException("Email is required.", nameof(email));

      if (code is null)
        throw new ArgumentNullException(nameof(code));

      var (subject, body) = BuildEmailContent(langCode, code);

      var mailRequest = new MailRequest
      {
        MailFrom = _mailFrom,
        MailTo = email,
        Subject = subject,
        Body = body
      };

      _logger.LogDebug("Sending email with one-time code to {Email}.", email);
      await _emailService.SendEmailMessage(mailRequest).ConfigureAwait(false);

      _logger.LogInformation(
        "Email with one-time code sent to {Email}. Expires at {ExpiresAt}.",
        email,
        code.ExpiresAtUtc.ToString("o", CultureInfo.InvariantCulture));
    }

    private (string Subject, string Body) BuildEmailContent(string? langCode, OneTimeCode code)
    {
      var isRussian = string.IsNullOrWhiteSpace(langCode) ||
                      langCode.StartsWith("ru", StringComparison.OrdinalIgnoreCase);

      if (isRussian)
      {
        var subject = $"Код для входа в {_appName}";
        var body = $"<html><p>Ваш одноразовый код: <strong>{code.Code}</strong>.</p><p>Он действителен до {code.ExpiresAtUtc:dd.MM.yyyy HH:mm} (UTC).</p></html>";
        return (subject, body);
      }

      var subjectEn = $"Your {_appName} login code";
      var bodyEn = $"<html><p>Your one-time code is <strong>{code.Code}</strong>.</p><p>The code expires at {code.ExpiresAtUtc:dd.MM.yyyy HH:mm} (UTC).</p></html>";
      return (subjectEn, bodyEn);
    }
  }
}
