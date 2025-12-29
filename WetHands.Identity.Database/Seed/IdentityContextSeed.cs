using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Core.Identity;
using Core.Models.Identity;
using WetHands.Identity.Database;

namespace Identity.Database.SeedData
{
  public class IdentityContextSeed
  {
    public static async Task SeedUsersAsync(UserManager<AppUser> userManager, RoleManager<Role> roleManager, ILoggerFactory loggerFactory, IdentityContext context)
    {
      var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

      try
      {



        // if (!userManager.Users.Any())
        if (!context.Users.Any() && false)
        {

          var itemsData = File.ReadAllText(path + @"/Seed/SeedData/users.json");
          var users = JsonSerializer.Deserialize<List<AppUser>>(itemsData);
          foreach (var user in users)
          {
            if (user.Email is null)
            {
              user.UserName = user.TelegramUserName;
              user.NormalizedUserName = user.TelegramUserName.ToUpper();
            }


            if (user.Email is not null)
            {
              user.UserName = user.Email;
              user.NormalizedEmail = user.Email.ToUpper();
            }

            user.CurrentLanguage = "ru-Ru";

            var result = userManager.CreateAsync(user, "Pa$$w0rd").Result;
          }

        }
        //

        if (!roleManager.Roles.Any())
        {
          var roles = new List<Role>()
          {
            new Role {Name = "Admin", RoleName = "Администратор"},
            new Role {Name = "User", RoleName = "Заказчик"},
            new Role {Name = "Moderator", RoleName = "Исполнитель"},
          };

          foreach (var role in roles)
          {
            await roleManager.CreateAsync(role);
          }

          // Optional bootstrap: if an admin user exists, grant roles.
          var admin = await userManager.FindByNameAsync("david.akobiya@gmail.com");
          if (admin != null)
          {
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "User", "Moderator" });
          }

        }



      }
      catch (Exception ex)
      {
        var logger = loggerFactory.CreateLogger<IdentityContextSeed>();
        logger.LogError(ex.Message);
      }
    }
  }
}