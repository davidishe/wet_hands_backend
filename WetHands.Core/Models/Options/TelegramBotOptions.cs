namespace WetHands.Core.Models.Options
{
  public class TelegramBotOptions
  {
    public bool Enabled { get; set; } = false;

    public string BotToken { get; set; } = string.Empty;

    public string LoginUrl { get; set; } = string.Empty;
  }
}
