using System.ComponentModel.DataAnnotations;

namespace WetHands.Core.Models.Options
{
  public class MailJetCredentialsOptions
  {
    [Required]
    public string MailJetApiKey { get; set; } = null!;

    [Required]
    public string MailJetApiSecret { get; set; } = null!;
  }
}
