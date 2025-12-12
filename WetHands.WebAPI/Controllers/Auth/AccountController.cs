using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Dtos;
using Core.Identity;
using WetHands.Core.Responses;
using WetHands.Identity;
using WetHands.Identity.Extensions;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System.Web;
using System;
using Microsoft.Extensions.Configuration;
using WetHands.Core.Models;
using WetHands.Auth.AuthService;
using RandomNameGeneratorLibrary;
using System.Threading;
using WetHands.Infrastructure.Services.Security;
using WetHands.Infrastructure.Services.Sms;
using System.Text.RegularExpressions;


namespace WebAPI.Controllers.Auth
{

  [Authorize]
  public class AccountController : BaseApiController
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;
    private readonly ILogger<AccountController> _logger;
    private readonly IOneTimeCodeService _oneTimeCodeService;
    private readonly ISmsSenderService _smsSenderService;


    private readonly string _emailDomainMailJetSender;

    public AccountController(
      UserManager<AppUser> userManager,
      SignInManager<AppUser> signInManager,
      ITokenService tokenService,
      IConfiguration config,
      IMapper mapper,
      IAuthService authService,
      IOneTimeCodeService oneTimeCodeService,
      ISmsSenderService smsSenderService,
      ILogger<AccountController> logger
       )
    {
      _signInManager = signInManager;
      _tokenService = tokenService;
      _mapper = mapper;
      _userManager = userManager;
      _emailDomainMailJetSender = config.GetSection("AppSettings:EmailDomain").Value;
      _authService = authService;
      _oneTimeCodeService = oneTimeCodeService;
      _smsSenderService = smsSenderService;
      _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("health_check")]
    public ActionResult HealthCheck()
    {
      return Ok("Все хорошо");
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("request_sms_code")]
    public async Task<IActionResult> RequestSmsCode([FromQuery] string phoneNumber, CancellationToken cancellationToken)
    {
      var normalizedPhone = NormalizePhoneNumber(phoneNumber);
      if (string.IsNullOrEmpty(normalizedPhone) || !IsPhoneNumberValid(normalizedPhone))
      {
        return BadRequest(new ApiResponse(400, "Укажите корректный номер телефона."));
      }

      try
      {
        var code = _oneTimeCodeService.CreateCode();
        await _smsSenderService.SendOneTimeCodeAsync(normalizedPhone, code, cancellationToken: cancellationToken);
        return Ok(new { expiresAtUtc = code.ExpiresAtUtc });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to send SMS code to {PhoneNumber}", normalizedPhone);
        return StatusCode(500, new ApiResponse(500, "Не удалось отправить SMS-код."));
      }
    }



    /// <summary>
    /// данный метод получает на входе эл.почту, создает пользователя если его нет
    /// отправляет токен на почту через модуль nopassword
    /// firstName указывается в случае если запрос идет со страницы регистрации, а не входа
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("register_email")]
    public async Task<ActionResult<UserToReturnDto>> RegisterEmail([FromQuery] string email, [FromQuery] string? firstName, [FromQuery] bool isAgency, [FromQuery] string langCode)
    {

      var user = await _userManager.FindByEmailAsync(email);

      if (user == null)
      {

        // var personGenerator = new PersonNameGenerator();
        var personGenerator = new PersonNameGenerator();
        var nickName = personGenerator.GenerateRandomFirstAndLastName();


        //TODO: merge Compant and AppUser
        var userToCreate = new AppUser()
        {
          Email = email,
          FirstName = firstName,
          UserName = email,
          CurrentLanguage = "ru-Ru",
          IsAdmin = false,
          IsAgency = isAgency,
          Currency = Currency.RUB,
          WasOnline = DateTime.Now,
          Nickname = nickName
        };

        await _userManager.CreateAsync(userToCreate);
      }

      // send code via email
      var token = await _authService.Login(email, _emailDomainMailJetSender, langCode);
      if (token is not null)
        return Ok(new { token = token });

      return BadRequest("Что-то пошло не так");
    }



    /// <summary>
    /// Получает на входе почту и токен (при регистрации - имя пользователя)
    /// Верифицирует почту
    /// </summary>
    /// <param name="email"></param>
    /// <param name="token"></param>
    /// <param name="firstName"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("login_with_email_code")]
    public async Task<ActionResult<UserToReturnDto>> LoginWithEmailCode([FromQuery] string email, [FromQuery] string code)
    {

      var decodedToken = HttpUtility.UrlDecode(code);
      var userForToken = await _userManager.FindByNameAsync(email);

      if (userForToken == null) return Unauthorized(new ApiResponse(404));

      // verify code for email here

      // var verifyUrl = "https://localhost:5090/nopassword/verify?email=" + email + "&token=" + decodedToken;
      // var res = await RetriveNoPasswordAuth(verifyUrl);

      // var authRes = await _authService.Verify(email: email, token: decodedToken);
      // var jwtSettings = new JwtSettings();
      // var tokenHandler = new JwtSecurityTokenHandler();
      // var tokenValidationParameters = new TokenValidationParameters
      // {
      //   ValidateIssuerSigningKey = true,
      //   IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
      //   ValidateIssuer = false,
      //   ValidateAudience = false,
      //   RequireExpirationTime = false,
      //   ValidateLifetime = true
      // };

      // SecurityToken validatedToken;
      // var principal = tokenHandler.ValidateToken(decodedToken, tokenValidationParameters, out validatedToken);

      // var isValid = await _userManager.VerifyUserTokenAsync(userForToken, "NPTokenProvider", "nopassword-for-the-win", decodedToken);

      // TODO: как-то научиться проверять токен _authService.Verify
      // if (isValid)
      // {
      var userToReturn = new UserToReturnDto
      {
        Email = userForToken.Email,
        FirstName = userForToken.FirstName,
        SecondName = userForToken.SecondName,
        Token = decodedToken,
        UserRoles = await _userManager.GetRolesAsync(userForToken),
        IsVerified = true
      };
      return Ok(userToReturn);
      // }


    }



    /// <summary>
    /// Получает на входе почту и токен (при регистрации - имя пользователя)
    /// Верифицирует почту
    /// </summary>
    /// <param name="email"></param>
    /// <param name="token"></param>
    /// <param name="firstName"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("login_with_email_code_demo")]
    public async Task<ActionResult<UserToReturnDto>> LoginWithEmailCodeDemo()
    {



      var email = "johnny@mnemonic.com";

      // var userToCreate = new AppUser()
      // {
      //   Email = email,
      //   FirstName = "Johnny",
      //   UserName = email,
      //   CurrentLanguage = "ru-Ru",
      //   IsAdmin = false,
      //   IsAgency = false,
      //   Currency = Currency.RUB,
      //   WasOnline = DateTime.Now,
      //   Nickname = "Johnny Mnemonic"
      // };

      // await _userManager.CreateAsync(userToCreate);

      // send code via email
      var token = await _authService.Login(email, _emailDomainMailJetSender, "ru-Ru");
      if (token is null)
        return Ok(new ApiResponse(405));


      var decodedToken = HttpUtility.UrlDecode(token);
      var userForToken = await _userManager.FindByNameAsync(email);

      if (userForToken == null) return Unauthorized(new ApiResponse(404));


      var userToReturn = new UserToReturnDto
      {
        Email = userForToken.Email,
        FirstName = userForToken.FirstName,
        SecondName = userForToken.SecondName,
        Token = decodedToken,
        UserRoles = await _userManager.GetRolesAsync(userForToken)
      };
      return Ok(userToReturn);


    }






    [Authorize]
    [HttpPut]
    [Route("update/profile_contacts")]
    public async Task<ActionResult<AppUser>> UpdateProduct([FromBody] UserToReturnDto userDto)
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      user.TelegramUserName = userDto.TelegramUserName;
      user.InstagramUserName = userDto.InstagramUserName;
      user.FacebookUserName = userDto.FacebookUserName;
      user.PhoneNumber = userDto.PhoneNumber;

      var result = await _userManager.UpdateAsync(user);
      return Ok(result);
    }



    [Authorize]
    [HttpPut]
    [Route("update/profile_info")]
    public async Task<ActionResult<AppUser>> UpdateProfileInfo([FromBody] UserInfoDto userDto)
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      user.FirstName = userDto.FirstName;
      user.SecondName = userDto.SecondName;
      user.UserDescription = userDto.UserDescription;
      await _userManager.UpdateAsync(user);
      var result = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      return Ok(result);
    }





    /// <summary>
    /// Меняет статус пользователя на onboarded = true
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPut]
    [Route("onboarded")]
    public async Task<IActionResult> SetOnboardedStatus()
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (user == null)
        return BadRequest("Пользователь не найден");

      var result = await _userManager.UpdateAsync(user);
      if (!result.Succeeded)
        return BadRequest("Произошла ошибка при обновлении пользователя");

      return Ok(true);

    }


    /// <summary>
    /// получает по токену информацию о логине и текущем пользователе
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    [Route("current")]
    public async Task<ActionResult<UserToReturnDto>> GetCurrentUser()
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);

      if (user == null)
        return NotFound("Пользователь не найден");

      var token = await _tokenService.CreateToken(user);
      if (token is null)
        return BadRequest("Не удалось создать токен");



      var roles = await _userManager.GetRolesAsync(user);

      var userToReturn = _mapper.Map<AppUser, UserToReturnDto>(user);
      userToReturn.Token = token;
      userToReturn.UserRoles = roles;
      userToReturn.CreatedAt = RandomDay();
      // userToReturn.FakeAvatar = GetFakeAvatar();

      return Ok(userToReturn);
    }






    /// <summary>
    /// Получаем пользователя по его id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    [Route("get_user_by_id")]
    public async Task<ActionResult<UserToReturnDto>> GetUserProfileById([FromQuery] string userId)
    {
      Thread.Sleep(1000);
      var user = await _userManager.FindByIdAsync(userId);

      if (user == null)
        return NotFound("Пользователь не найден");

      var roles = await _userManager.GetRolesAsync(user);
      var userToReturn = _mapper.Map<AppUser, UserToReturnDto>(user);

      userToReturn.UserRoles = roles;
      userToReturn.CreatedAt = RandomDay();
      return Ok(userToReturn);
    }




    private Random gen = new Random();
    DateTime RandomDay()
    {
      DateTime start = new DateTime(1995, 1, 1);
      int range = (DateTime.Today - start).Days;
      return start.AddDays(gen.Next(range));
    }

    private static string NormalizePhoneNumber(string phoneNumber)
    {
      if (string.IsNullOrWhiteSpace(phoneNumber))
      {
        return string.Empty;
      }

      var trimmed = phoneNumber.Trim();
      var digitsOnly = Regex.Replace(trimmed, @"\D", string.Empty);

      if (string.IsNullOrEmpty(digitsOnly))
      {
        return string.Empty;
      }

      return trimmed.StartsWith("+", StringComparison.Ordinal) ? $"+{digitsOnly}" : digitsOnly;
    }

    private static bool IsPhoneNumberValid(string normalizedPhone)
    {
      if (string.IsNullOrEmpty(normalizedPhone))
      {
        return false;
      }

      var digits = normalizedPhone.StartsWith("+", StringComparison.Ordinal) ? normalizedPhone[1..] : normalizedPhone;
      return Regex.IsMatch(digits, @"^\d{10,15}$");
    }



    // private async Task<bool> RetriveNoPasswordAuth(string requestUrl)
    // {
    //   var client = new HttpClientModule();
    //   var result = await client.MakeHttpCallWithStream(requestUrl);
    //   return true;
    // }


  }
}
