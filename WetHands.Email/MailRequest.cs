namespace WetHands.Email.Models;


public class MailRequest
{

  public string MailFrom { get; set; }
  public string MailTo { get; set; }
  public string Subject { get; set; }
  public string Body { get; set; }

}
