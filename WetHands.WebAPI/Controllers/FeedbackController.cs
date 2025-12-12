using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Identity;
using WetHands.Identity;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using WebAPI.Controllers;
using WetHands.Email.EmailService;
using WetHands.Email.Models;

namespace WetHands.WebAPI.Controllers
{

    [Authorize]
    public class FeedbackController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<FeedbackController> _logger;
        private readonly IEmailService _emailService;
        private readonly string _emailDomainMailJetSender;
        private readonly string _baseUrl;

        public FeedbackController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            IConfiguration config,
            IMapper mapper,
            IEmailService emailService
        )
        {
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _mapper = mapper;
            _userManager = userManager;
            _emailDomainMailJetSender = config.GetSection("AppSettings:EmailDomain").Value;
            _baseUrl = config.GetSection("AppSettings:FrontendUrl").Value;
        }


        /// <summary>
        /// данный метод отправляет на почту запрос по обратной связи
        /// firstName указывается в случае если запрос идет со страницы регистрации, а не входа
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("send")]
        public async Task<ActionResult> SendGetFeedbackEmail([FromQuery] string phone)
        {
            var body = new MailRequest()
            {
                MailFrom = "noreplay@telecost.pro",
                MailTo = "david.akobiya@gmail.com",
                Subject = "Запрос на созвон",
                Body = $"<html>Поступил запрос на созвон с сайта PropertyBook. Номер телефона: {phone} </html>"
            };


            var result = await _emailService.SendEmailMessage(body);

            if (result is null)
                return BadRequest("Что-то пошло не так");

            else
                return Ok(result);

        }





    }
}

