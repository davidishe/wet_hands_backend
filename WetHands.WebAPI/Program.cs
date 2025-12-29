using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Core.Identity;
using Core.Models.Identity;
using WetHands.Infrastructure.Database;
using WetHands.Identity.Database;
using Identity.Database.SeedData;
using Infrastructure.Database.SeedData;

namespace WebAPI
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = CreateHostBuilder(args);
      var host = builder.Build();

      using (var scope = host.Services.CreateScope())
      {


        var services = scope.ServiceProvider;
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        try
        {
          var userManager = services.GetRequiredService<UserManager<AppUser>>();
          var roleManager = services.GetRequiredService<RoleManager<Role>>();
          var identityContext = services.GetRequiredService<IdentityContext>();

          // This solution ships without EF Core migrations. MigrateAsync() would create an empty database,
          // but it would NOT create Identity tables (AspNetUsers, AspNetRoles, ...), leading to 500 errors.
          // EnsureCreated creates the schema from the current model when it is missing.
          await identityContext.Database.EnsureCreatedAsync();
          await IdentityContextSeed.SeedUsersAsync(userManager, roleManager, loggerFactory, identityContext);

          var context = services.GetRequiredService<AppDbContext>();
          await context.Database.EnsureCreatedAsync();
          await DataContextSeed.SeedDataAsync(context, loggerFactory);

        }
        catch (Exception ex)
        {
          var logger = services.GetRequiredService<ILogger<Program>>();
          logger.LogError(ex, "An error occured during migrtation");
        }
      }



      host.Run();

    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>().
                UseUrls("https://localhost:6155");
            });


  }
}
