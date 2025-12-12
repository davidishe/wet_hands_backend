using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Core.Identity;
using Core.Models.Identity;
using Core.Options;
using WetHands.Identity.Services;
using WetHands.Identity.Database;
using System.Threading.Tasks;

namespace WetHands.Identity.Extensions
{
  public static class IdentityServicesExtensions

  {
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {


      var builder = services.AddIdentityCore<AppUser>(opt =>
      {
        opt.Password.RequireDigit = false;
        opt.Password.RequiredLength = 4;
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireLowercase = false;
      });

      var b = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
      b.AddEntityFrameworkStores<IdentityContext>();
      b.AddRoleValidator<RoleValidator<Role>>();
      b.AddRoleManager<RoleManager<Role>>();
      b.AddSignInManager<SignInManager<AppUser>>();


      // jwt id service
      var jwtSettings = new JwtSettings();
      config.Bind(nameof(jwtSettings), jwtSettings);
      services.AddSingleton(jwtSettings);

      services.AddScoped<ITokenService, TokenService>();
      services.AddScoped<IRoleManagerService, RoleManagerService>();

      // facebook settings
      var facebookAuthSettings = new FacebookAuthSettings();
      config.Bind(nameof(FacebookAuthSettings), facebookAuthSettings);
      services.AddSingleton(facebookAuthSettings);
      services.AddHttpClient();


      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = false,
        ValidateLifetime = true
      };
      services.AddSingleton(tokenValidationParameters);

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer
        (options =>
        {
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Token:Key"])),
            ValidIssuer = config["Token:Issuer"],
            ValidateIssuer = true,
            ValidateAudience = false
          };
          options.Events = new JwtBearerEvents
          {
            OnMessageReceived = context =>
            {
              var accessToken = context.Request.Query["access_token"];

              // если запрос направлен хабу
              var path = context.HttpContext.Request.Path;
              if (!string.IsNullOrEmpty(accessToken) &&
                  (path.StartsWithSegments("/chathub")))
              {
                // получаем токен из строки запроса
                context.Token = accessToken;
              }
              return Task.CompletedTask;

            }

          };
        });



      services.AddAuthorization(options =>
      {
        options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
        options.AddPolicy("RequireModerator", policy => policy.RequireRole("Admin, Moderator"));
        options.AddPolicy("RequireAuthentication", policy => policy.RequireRole("Admin, Moderator, Client"));
      });

      return services;
    }

  }
}