using Mailjet.Client.TransactionalEmails.Response;
using WetHands.Email.Models;

namespace WetHands.Email.EmailService
{
  public interface IEmailService
  {
    Task<TransactionalEmailResponse> SendEmailMessage(MailRequest body);

  }
}