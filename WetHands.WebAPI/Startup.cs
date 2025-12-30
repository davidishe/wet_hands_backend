using System;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using WebAPI.Middleware;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WetHands.Infrastructure.Database;
using Core.Models;
using WetHands.Identity.Database;
using WetHands.Identity.Extensions;
using WetHands.Infrastructure;
using WetHands.Core.Models.Items;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using WetHands.Core.Models;
using WetHands.Email.EmailService;
using WetHands.Core.TonModels;
using WetHands.Core;
using WetHands.Auth.AuthService;
using Mailjet.Client;
using WetHands.Core.Basic;
using WetHands.Core.Models.Options;
using WetHands.Infrastructure.Services.Security;
using WetHands.Infrastructure.Services.Sms;
using WetHands.Infrastructure.Documents;
using WetHands.Infrastructure.Services.TelegramBot;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      _config = configuration;
    }

    public IConfiguration _config { get; }


    public void ConfigureDevelopmentServices(IServiceCollection services)
    {
      ConfigureServices(services);
    }

    public void ConfigureProductionServices(IServiceCollection services)
    {
      ConfigureServices(services);
    }

    public void ConfigureServices(IServiceCollection services)
    {

      services.AddDbContext<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("DefaultConnection")));
      services.AddDbContext<IdentityContext>(options => options.UseSqlServer(_config.GetConnectionString("IdentityConnection")));


      services.AddIdentityServices(_config);
      services.AddApplicationServices();
      services.Configure<MailJetCredentialsOptions>(_config.GetSection("MailJetCredentials"));
      services.Configure<MailJetSmsOptions>(_config.GetSection("MailJetSms"));
      services.Configure<TelegramBotOptions>(_config.GetSection("TelegramBot"));


      services.AddControllers(options =>
      {
        var policy = new AuthorizationPolicyBuilder()
          .RequireAuthenticatedUser()
          .Build();

        options.Filters.Add(new AuthorizeFilter(policy));
      })
        .AddNewtonsoftJson(opt =>
        {
          opt.SerializerSettings.ReferenceLoopHandling =
          Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        });

      services.AddCors();
      services.AddAutoMapper(typeof(Startup));

      services.AddMemoryCache();

      services.AddScoped<IDbRepository<PaymentRequest>, DbRepository<PaymentRequest>>();
      services.AddScoped<IDbRepository<Favour>, DbRepository<Favour>>();
      services.AddScoped<IDbRepository<Grade>, DbRepository<Grade>>();
      services.AddScoped<IDbRepository<PaymentRequestStatus>, DbRepository<PaymentRequestStatus>>();
      services.AddScoped<IDbRepository<ProposalProfileStatus>, DbRepository<ProposalProfileStatus>>();
      services.AddScoped<IDbRepository<TonLocalTransaction>, DbRepository<TonLocalTransaction>>();



      services.AddScoped<IDbRepository<NftPlot>, DbRepository<NftPlot>>();
      services.AddScoped<IDbRepository<Picture>, DbRepository<Picture>>();
      services.AddScoped<IDbRepository<WetHands.Core.Models.Items.File>, DbRepository<WetHands.Core.Models.Items.File>>();


      services.AddScoped<IDbRepository<Company>, DbRepository<Company>>();
      services.AddScoped<IDbRepository<OrderStatus>, DbRepository<OrderStatus>>();
      services.AddScoped<IDbRepository<Region>, DbRepository<Region>>();
      services.AddScoped<IDbRepository<Order>, DbRepository<Order>>();
      services.AddScoped<IDbRepository<OrderItem>, DbRepository<OrderItem>>();
      services.AddScoped<IDbRepository<OrderItemType>, DbRepository<OrderItemType>>();
      services.AddScoped<IDbRepository<MassagePlace>, DbRepository<MassagePlace>>();
      services.AddScoped<IDbRepository<MassagePlaceImage>, DbRepository<MassagePlaceImage>>();
      services.AddScoped<IDbRepository<MassageCategory>, DbRepository<MassageCategory>>();
      services.AddScoped<IDbRepository<Country>, DbRepository<Country>>();
      services.AddScoped<IDbRepository<City>, DbRepository<City>>();
      services.AddScoped<IAuthService, AuthService>();


      services.AddSignalR();

      // services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));
      // services.AddScoped<IUnitOfWork, UnitOfWork>();

      services.AddScoped<IDbRepository<ProposalDescriptionTranaltaion>, DbRepository<ProposalDescriptionTranaltaion>>();
      services.AddScoped<IEmailService, EmailService>();
      services.AddSingleton<IOneTimeCodeService, OneTimeCodeService>();
      services.AddSingleton<IOneTimeCodeStore, MemoryOneTimeCodeStore>();
      services.AddScoped<ISmsSenderService, MailjetSmsService>();
      services.AddTransient<IOrderDocumentService, OrderDocumentService>();
      // Telegram bot is optional; keep it disabled by default to avoid noisy logs when token is invalid.
      if (_config.GetValue<bool>("TelegramBot:Enabled"))
      {
        services.AddHostedService<TelegramBotBackgroundService>();
      }


      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "NotificationService", Version = "v1" });
      });
      services.AddDirectoryBrowser();

      var apiKey = _config.GetSection("MailJetCredentials:MailJetApiKey").Value;
      var apiSecret = _config.GetSection("MailJetCredentials:MailJetApiSecret").Value;

      services.AddHttpClient("MailjetSms", client =>
      {
        client.BaseAddress = new Uri("https://api.mailjet.com/");
      });

      services.AddHttpClient<IMailjetClient, MailjetClient>(client =>
      {
        client.SetDefaultSettings();
        client.UseBasicAuthentication(apiKey, apiSecret);
      });



    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chtole bot v1"));
      }


      app.UseMiddleware<ExceptionMiddleware>();
      app.UseStatusCodePagesWithReExecute("/errors/{0}");
      app.UseRouting();
      app
        .UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
        .WithOrigins(
                      "https://localhost:4200",
                      "https://localhost:4300",
                      "https://localhost:4400",
                      "https://localhost:4500",
                      "https://api.telecost.pro",
                      "https://propertybook.space",
                      "https://api.propertybook.space",
                      "https://telecost.pro",
                      "https://laundr.rwaplace.io",
                      "https://rwaplace.io",
                      "https://property.telecost.pro",
                      "https://terraplatform.space",
                      "*")
        .AllowCredentials());


      app.UseAuthentication();
      app.UseAuthorization();
      app.UseDefaultFiles();
      // app.UseStaticFiles();
      app.UseStaticFiles(new StaticFileOptions
      {
        FileProvider = new PhysicalFileProvider(
          Path.Combine(Directory.GetCurrentDirectory(), "Assets")
        ),
        RequestPath = "/assets"
      });




      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        // endpoints.MapHub<ChatHub>("/chathub");
        // endpoints.MapFallbackToController("Index", "Fallback");
      });


      IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en"), //English US
                new CultureInfo("ar"), //Arabic SY
            };
      var localizationOptions = new RequestLocalizationOptions
      {
        DefaultRequestCulture = new RequestCulture("en"), //English US will be the default culture (for new visitors)
        SupportedCultures = supportedCultures,
        SupportedUICultures = supportedCultures
      };

      app.UseRequestLocalization(localizationOptions);
    }
  }
}
