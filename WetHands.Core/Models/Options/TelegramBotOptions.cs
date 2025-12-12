using System.ComponentModel.DataAnnotations;

namespace WetHands.Core.Models.Options
{
  public class TelegramBotOptions
  {
    [Required]
    public string BotToken { get; set; } = null!;

    [Required]
    public string LoginUrl { get; set; } = null!;
  }
}
