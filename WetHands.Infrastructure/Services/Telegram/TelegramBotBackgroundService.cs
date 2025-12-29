using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Core.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WetHands.Core.Models;
using WetHands.Core.Models.Options;
using WetHands.Identity;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace WetHands.Infrastructure.Services.TelegramBot
{
  public class TelegramBotBackgroundService : BackgroundService
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly TelegramBotOptions _options;
    private readonly ILogger<TelegramBotBackgroundService> _logger;
    private TelegramBotClient? _botClient;

    public TelegramBotBackgroundService(
      IServiceProvider serviceProvider,
      IOptions<TelegramBotOptions> options,
      ILogger<TelegramBotBackgroundService> logger)
    {
      _serviceProvider = serviceProvider;
      _options = options.Value;
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      // Hard-disable when the feature is turned off in config.
      if (!_options.Enabled)
      {
        return;
      }

      if (string.IsNullOrWhiteSpace(_options.BotToken))
      {
        _logger.LogWarning("Telegram bot token is not configured. Telegram login will be disabled.");
        return;
      }

      _botClient = new TelegramBotClient(_options.BotToken);

      var receiverOptions = new ReceiverOptions
      {
        AllowedUpdates = Array.Empty<UpdateType>()
      };

      _botClient.StartReceiving(
        HandleUpdateAsync,
        HandleErrorAsync,
        receiverOptions,
        cancellationToken: stoppingToken);

      try
      {
        var me = await _botClient.GetMeAsync(stoppingToken);
        _logger.LogInformation("Telegram bot started. Username: @{Username}", me.Username);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to initialize Telegram bot.");
      }

      try
      {
        await Task.Delay(Timeout.Infinite, stoppingToken);
      }
      catch (TaskCanceledException)
      {
        _logger.LogInformation("Telegram bot background service is stopping.");
      }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
      _logger.LogInformation("Received update of type {UpdateType}", update.Type);

      if (update.Message is not { } message)
      {
        _logger.LogInformation("Update skipped because it does not contain a message payload.");
        return;
      }

      _logger.LogInformation("Handling message update. Chat: {ChatId}, Type: {MessageType}", message.Chat.Id, message.Type);

      if (message.Type == MessageType.Contact && message.Contact is not null)
      {
        _logger.LogInformation("Processing contact from user {UserId}", message.From?.Id);
        await HandleContactAsync(botClient, message, cancellationToken);
        return;
      }

      if (message.Type == MessageType.Text)
      {
        var text = message.Text?.Trim();
        _logger.LogInformation("Processing text message: {Text}", text);

        if (string.Equals(text, "/start", StringComparison.OrdinalIgnoreCase))
        {
          _logger.LogInformation("Handling /start command for chat {ChatId}", message.Chat.Id);
          await PromptForPhoneAsync(botClient, message.Chat.Id, cancellationToken);
          return;
        }

        await botClient.SendTextMessageAsync(
          chatId: message.Chat.Id,
          text: "Чтобы войти в приложение, нажмите кнопку и поделитесь номером телефона.",
          replyMarkup: BuildContactRequestKeyboard(),
          cancellationToken: cancellationToken);
      }
    }

    private async Task HandleContactAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Message message, CancellationToken cancellationToken)
    {
      var contact = message.Contact;
      if (contact is null)
      {
        _logger.LogWarning("Received contact message without contact payload.");
        return;
      }

      var normalizedPhone = NormalizePhone(contact.PhoneNumber);
      if (string.IsNullOrEmpty(normalizedPhone))
      {
        await botClient.SendTextMessageAsync(
          chatId: message.Chat.Id,
          text: "Не удалось распознать номер телефона. Попробуйте еще раз.",
          replyMarkup: BuildContactRequestKeyboard(),
          cancellationToken: cancellationToken);
        return;
      }

      using var scope = _serviceProvider.CreateScope();
      var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
      var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

      var email = BuildTelegramEmail(normalizedPhone);

      var user = await userManager.Users.FirstOrDefaultAsync(
        u => u.PhoneNumber == normalizedPhone,
        cancellationToken);

      if (user is null) user = await userManager.FindByNameAsync(email);

      if (user is null) user = await userManager.FindByEmailAsync(email);

      var existingUser = user is not null;

      if (!existingUser)
      {
        var newUser = new AppUser
        {
          Email = email,
          UserName = email,
          PhoneNumber = normalizedPhone,
          PhoneNumberConfirmed = true,
          TelegramId = message.From?.Id,
          TelegramUserName = message.From?.Username,
          FirstName = contact.FirstName,
          SecondName = contact.LastName,
          CurrentLanguage = "ru-RU",
          Currency = Currency.RUB,
          WasOnline = DateTime.UtcNow,
          IsAgency = false
        };

        var creationResult = await userManager.CreateAsync(newUser);
        if (!creationResult.Succeeded)
        {
          var duplicateUser = await userManager.FindByNameAsync(email) ??
            await userManager.FindByEmailAsync(email);
          if (duplicateUser is null)
          {
            var errors = string.Join(", ", creationResult.Errors.Select(e => e.Description));
            _logger.LogError("Failed to create user for phone {Phone}. Errors: {Errors}", normalizedPhone, errors);
            await botClient.SendTextMessageAsync(
              chatId: message.Chat.Id,
              text: "Не удалось создать учетную запись. Попробуйте позже.",
              cancellationToken: cancellationToken,
              replyMarkup: new ReplyKeyboardRemove());
            return;
          }

          user = duplicateUser;
          existingUser = true;
          _logger.LogInformation("Reusing existing Telegram user {UserId} for phone {Phone} after creation conflict.", duplicateUser.Id, normalizedPhone);
        }
        else
        {
          user = newUser;
        }
      }

      if (existingUser && user is not null)
      {
        var updated = false;
        if (user.TelegramId != message.From?.Id)
        {
          user.TelegramId = message.From?.Id;
          updated = true;
        }
        if (!string.IsNullOrEmpty(message.From?.Username) && user.TelegramUserName != message.From?.Username)
        {
          user.TelegramUserName = message.From?.Username;
          updated = true;
        }
        if (!string.IsNullOrEmpty(contact.FirstName) && user.FirstName != contact.FirstName)
        {
          user.FirstName = contact.FirstName;
          updated = true;
        }
        if (!string.IsNullOrEmpty(contact.LastName) && user.SecondName != contact.LastName)
        {
          user.SecondName = contact.LastName;
          updated = true;
        }
        if (!string.Equals(user.PhoneNumber, normalizedPhone, StringComparison.Ordinal))
        {
          user.PhoneNumber = normalizedPhone;
          updated = true;
        }
        if (!user.PhoneNumberConfirmed)
        {
          user.PhoneNumberConfirmed = true;
          updated = true;
        }
        if (updated)
        {
          user.WasOnline = DateTime.UtcNow;
          await userManager.UpdateAsync(user);
        }
      }

      var token = await tokenService.CreateToken(user);
      var loginUrl = BuildLoginUrl(token, user.Email);

      await botClient.SendTextMessageAsync(
        chatId: message.Chat.Id,
        text: "Готово! Сформировали ссылку для входа.",
        replyMarkup: new ReplyKeyboardRemove(),
        cancellationToken: cancellationToken);

      var inlineKeyboard = new InlineKeyboardMarkup(new[]
      {
        new[]
        {
          InlineKeyboardButton.WithUrl("Перейти в приложение", loginUrl)
        }
      });

      await botClient.SendTextMessageAsync(
        chatId: message.Chat.Id,
        text: "Нажмите кнопку ниже, чтобы войти в приложение.",
        replyMarkup: inlineKeyboard,
        disableWebPagePreview: true,
        cancellationToken: cancellationToken);
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
      switch (exception)
      {
        case RequestException requestException when IsTimeoutException(requestException):
          _logger.LogWarning("Telegram bot request timed out. The client will retry automatically. Details: {Message}",
            requestException.Message);
          break;

        case ApiRequestException apiRequestException:
          _logger.LogError("Telegram API error {ErrorCode}: {Message}",
            apiRequestException.ErrorCode,
            apiRequestException.Message);
          break;

        case RequestException requestException:
          _logger.LogError(requestException, "Telegram bot request failed. Message: {Message}",
            requestException.Message);
          break;

        case OperationCanceledException when cancellationToken.IsCancellationRequested:
          _logger.LogInformation("Telegram bot polling cancelled due to shutdown.");
          break;

        default:
          _logger.LogError(exception, "Unexpected Telegram bot error.");
          break;
      }

      return Task.CompletedTask;
    }

    private static bool IsTimeoutException(RequestException requestException)
    {
      if (requestException.InnerException is TimeoutException) return true;

      var current = requestException.InnerException;
      while (current is not null)
      {
        if (current is TimeoutException) return true;

        current = current.InnerException;
      }

      return requestException.Message.Contains("timed out", StringComparison.OrdinalIgnoreCase);
    }

    private async Task PromptForPhoneAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
      await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "Привет! Нажмите кнопку ниже и поделитесь номером телефона, чтобы войти в приложение.",
        replyMarkup: BuildContactRequestKeyboard(),
        cancellationToken: cancellationToken);
      _logger.LogInformation("Prompted chat {ChatId} to share phone number.", chatId);
    }

    private static ReplyKeyboardMarkup BuildContactRequestKeyboard()
    {
      return new ReplyKeyboardMarkup(new[]
        {
          new KeyboardButton[]
          {
            new KeyboardButton("Поделиться номером телефона") { RequestContact = true }
          }
        })
      {
        ResizeKeyboard = true,
        OneTimeKeyboard = true
      };
    }

    private static string BuildTelegramEmail(string normalizedPhone)
    {
      var digits = Regex.Replace(normalizedPhone, @"\D", string.Empty);
      return $"tg_{digits}@telegram-login.local";
    }

    private string BuildLoginUrl(string token, string email)
    {
      var encodedToken = Uri.EscapeDataString(token);
      var encodedEmail = Uri.EscapeDataString(email);
      var separator = _options.LoginUrl.Contains('?') ? "&" : "?";
      return $"{_options.LoginUrl}{separator}token={encodedToken}&email={encodedEmail}";
    }

    private static string NormalizePhone(string? rawPhone)
    {
      if (string.IsNullOrWhiteSpace(rawPhone))
      {
        return string.Empty;
      }

      var trimmed = rawPhone.Trim();
      var digitsOnly = Regex.Replace(trimmed, @"\D", string.Empty);

      if (string.IsNullOrEmpty(digitsOnly))
      {
        return string.Empty;
      }

      return trimmed.StartsWith("+", StringComparison.Ordinal) ? $"+{digitsOnly}" : digitsOnly;
    }
  }
}
