using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using WetHands.Email.Models;
using WetHands.Email.EmailService;
using WetHands.Identity;
using Core.Identity;
using Microsoft.Extensions.Configuration;

namespace WetHands.Auth.AuthService
{
    public class AuthService : IAuthService
    {
        private IHostEnvironment _hostingEnv;
        private readonly IEmailService _emailService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AuthService> _logger;
        private readonly ITokenService _tokenService;
        private readonly string _frontendUrl;
        private readonly string _appName;


        public AuthService(
                UserManager<AppUser> userManager,
                ILogger<AuthService> logger,
                IEmailService emailService,
                IHostEnvironment hostingEnv,
                ITokenService tokenService,
                IConfiguration config
        )
        {
            _userManager = userManager;
            _logger = logger;
            _emailService = emailService;
            _hostingEnv = hostingEnv;
            _tokenService = tokenService;
            _frontendUrl = config.GetSection("AppSettings:FrontendUrl").Value!;
            _appName = config.GetSection("AppSettings:AppName").Value!;
        }

        public async Task<string> Login(string email, string mailFrom, string langCode)
        {
            var User = await _userManager.FindByNameAsync(email);
            if (User == null)
            {
                var newUser = new AppUser
                {
                    Email = email,
                    UserName = email
                };

                var identityResult = await _userManager.CreateAsync(newUser);
                if (identityResult.Succeeded == false)
                    return null;
            }

            // var token = await _userManager.GenerateUserTokenAsync(User, "NPTokenProvider", "nopassword-for-the-win");
            var token = await _tokenService.CreateToken(User);

            // send token to email


            string validationUrl = "";
            if (_hostingEnv.IsDevelopment())
                validationUrl += $"{_frontendUrl}/auth/verify?token={token}&email={email}";

            if (_hostingEnv.IsProduction())
                validationUrl += $"{_frontendUrl}/auth/verify?token={token}&email={email}";


            var localizedBody = "";
            var subject = "";
            if (langCode.Equals("ru") || langCode.Equals("ru-RU"))
            {
                localizedBody = $"<html>Для входа в приложение <a href='{validationUrl}'>перейдите по ссылке</a> </html>";
                subject = $"Линк для входа в {_appName}";
            }

            else
            {
                localizedBody = $"<html>To enter the application, follow <a href='{validationUrl}'>the link</a> </html>";
                subject = "Link to log in to the application";
            }


            var body = new MailRequest()
            {
                MailFrom = mailFrom,
                MailTo = email,
                Subject = subject,
                Body = localizedBody
            };


            await _emailService.SendEmailMessage(body);
            return token;

        }


        public async Task<bool> Verify(string token, string email)
        {
            // Fetch your user from the database
            var User = await _userManager.FindByNameAsync(email);
            if (User == null)
                return false;

            var decodedToken = HttpUtility.UrlDecode(token);

            _logger.LogInformation("Verifying...");
            _logger.LogInformation(decodedToken);

            var IsValid = await _userManager.VerifyUserTokenAsync(User, "NPTokenProvider", "nopassword-for-the-win", token);
            if (IsValid) return true;

            return false;

        }
    }
}