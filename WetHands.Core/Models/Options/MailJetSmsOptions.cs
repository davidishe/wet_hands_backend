using System.ComponentModel.DataAnnotations;

namespace WetHands.Core.Models.Options
{
  public class MailJetSmsOptions
  {
    [Required]
    public string ApiToken { get; set; } = null!;

    [Required]
    public string From { get; set; } = null!;
  }
}
