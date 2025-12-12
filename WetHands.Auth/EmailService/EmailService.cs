// using Microsoft.Extensions.Logging;
// using WetHands.Email.Models;
// using RestSharp;

// namespace WetHands.Auth.EmailService
// {

//     public class EmailService : IEmailService
//   {


//     private readonly ILogger<EmailService> _logger;


//     public EmailService(
//       // IConfiguration config,
//       ILogger<EmailService> logger
//     )
//     {
//       _logger = logger;
//     }


//     public RestResponse SendEmailMessage(MailRequest body)
//     {

//       var client = new RestClient("https://localhost:6047/api/mail/feedback/");
//       var restRequest = new RestRequest();
//       restRequest.AddParameter("application/json", body, ParameterType.RequestBody);
//       restRequest.Method = Method.Post;
//       Console.WriteLine("Sending email..");
//       return client.Execute(restRequest);
//     }


//   }

// }