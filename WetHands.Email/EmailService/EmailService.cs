using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Mailjet.Client.TransactionalEmails.Response;
using Microsoft.Extensions.Logging;
using WetHands.Email.Models;

namespace WetHands.Email.EmailService
{

  public class EmailService : IEmailService
  {

    private readonly ILogger<EmailService> _logger;
    private readonly IMailjetClient _mailjetClient;



    public EmailService(
      IMailjetClient mailjetClient,
      ILogger<EmailService> logger
    )
    {
      _logger = logger;
      _mailjetClient = mailjetClient;

    }


    public async Task<TransactionalEmailResponse> SendEmailMessage(MailRequest body)
    {


      var email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact(body.MailFrom))
                .WithSubject(body.Subject)
                .WithHtmlPart(body.Body)
                .WithTo(new SendContact(body.MailTo))
                .Build();

      var response = await _mailjetClient.SendTransactionalEmailAsync(email);



      // var client = new RestClient("http://localhost:6047/api/mail/feedback/");
      // var restRequest = new RestRequest();
      // restRequest.AddParameter("application/json", body, ParameterType.RequestBody);
      // restRequest.Method = Method.Post;
      // Console.WriteLine("Sending email..");
      // var result = await client.ExecuteAsync(restRequest);
      return response;
    }
  }

}