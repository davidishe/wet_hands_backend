using System;
using System.Data;
using System.Data.Common;
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
        var startupLogger = loggerFactory.CreateLogger<Program>();
        try
        {
          var userManager = services.GetRequiredService<UserManager<AppUser>>();
          var roleManager = services.GetRequiredService<RoleManager<Role>>();
          var identityContext = services.GetRequiredService<IdentityContext>();

          // This solution ships without EF Core migrations. MigrateAsync() would create an empty database,
          // but it would NOT create Identity tables (AspNetUsers, AspNetRoles, ...), leading to 500 errors.
          // EnsureCreated creates the schema from the current model when it is missing.
          await identityContext.Database.EnsureCreatedAsync();
          await EnsureIdentitySchemaAsync(identityContext, startupLogger);
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

    private static async Task EnsureIdentitySchemaAsync(IdentityContext identityContext, ILogger logger)
    {
      var conn = identityContext.Database.GetDbConnection();
      logger.LogInformation("Identity DB connection: dataSource={DataSource}, database={Database}", conn.DataSource, conn.Database);

      // EnsureCreated() can be a no-op if the database exists but is not empty.
      // We hard-check the presence of AspNetUsers and bootstrap tables if missing.
      if (await TableExistsAsync(conn, "AspNetUsers"))
      {
        return;
      }

      logger.LogWarning("Identity table AspNetUsers is missing. Bootstrapping Identity schema via SQL...");

      await identityContext.Database.ExecuteSqlRawAsync(GetIdentityBootstrapSql());

      if (!await TableExistsAsync(conn, "AspNetUsers"))
      {
        throw new InvalidOperationException("Identity schema bootstrap failed: AspNetUsers still does not exist. Check DB permissions and connection string.");
      }
    }

    private static async Task<bool> TableExistsAsync(DbConnection connection, string tableName)
    {
      if (connection.State != ConnectionState.Open)
      {
        await connection.OpenAsync();
      }

      using var cmd = connection.CreateCommand();
      cmd.CommandText = "SELECT 1 WHERE OBJECT_ID(N'[dbo].[" + tableName + "]', 'U') IS NOT NULL";
      var result = await cmd.ExecuteScalarAsync();
      return result != null && result != DBNull.Value;
    }

    private static string GetIdentityBootstrapSql()
    {
      // Idempotent SQL Server bootstrap for ASP.NET Identity (int keys).
      // This avoids relying on EF Core migrations (which are not present in this repo).
      return @"
IF OBJECT_ID(N'[dbo].[AspNetRoles]', 'U') IS NULL
BEGIN
  CREATE TABLE [dbo].[AspNetRoles] (
      [Id] int NOT NULL IDENTITY(1,1),
      [RoleName] nvarchar(max) NOT NULL,
      [Name] nvarchar(256) NULL,
      [NormalizedName] nvarchar(256) NULL,
      [ConcurrencyStamp] nvarchar(max) NULL,
      CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
  );
END;

IF OBJECT_ID(N'[dbo].[AspNetUsers]', 'U') IS NULL
BEGIN
  CREATE TABLE [dbo].[AspNetUsers] (
      [Id] int NOT NULL IDENTITY(1,1),
      [FirstName] nvarchar(max) NULL,
      [SecondName] nvarchar(max) NULL,
      [PictureUrl] nvarchar(max) NULL,
      [PictureByte] varbinary(max) NULL,
      [PictureType] nvarchar(max) NULL,
      [UserDescription] nvarchar(max) NULL,
      [CurrentLanguage] nvarchar(max) NOT NULL,
      [TelegramId] bigint NULL,
      [TelegramUserName] nvarchar(max) NULL,
      [InstagramUserName] nvarchar(max) NULL,
      [FacebookUserName] nvarchar(max) NULL,
      [PnoneNumber] nvarchar(max) NULL,
      [CompanyId] int NULL,
      [IsAgency] bit NULL,
      [WasOnline] datetime2 NULL,
      [Currency] int NOT NULL,
      [IsAdmin] bit NOT NULL,
      [JoinCode] nvarchar(max) NOT NULL,
      [Nickname] nvarchar(max) NULL,
      [FakeAvatar] nvarchar(max) NULL,
      [IsVerified] bit NULL,
      [UserName] nvarchar(256) NULL,
      [NormalizedUserName] nvarchar(256) NULL,
      [Email] nvarchar(256) NULL,
      [NormalizedEmail] nvarchar(256) NULL,
      [EmailConfirmed] bit NOT NULL,
      [PasswordHash] nvarchar(max) NULL,
      [SecurityStamp] nvarchar(max) NULL,
      [ConcurrencyStamp] nvarchar(max) NULL,
      [PhoneNumber] nvarchar(max) NULL,
      [PhoneNumberConfirmed] bit NOT NULL,
      [TwoFactorEnabled] bit NOT NULL,
      [LockoutEnd] datetimeoffset NULL,
      [LockoutEnabled] bit NOT NULL,
      [AccessFailedCount] int NOT NULL,
      CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
  );
END;

IF OBJECT_ID(N'[dbo].[AspNetRoleClaims]', 'U') IS NULL
BEGIN
  CREATE TABLE [dbo].[AspNetRoleClaims] (
      [Id] int NOT NULL IDENTITY(1,1),
      [RoleId] int NOT NULL,
      [ClaimType] nvarchar(max) NULL,
      [ClaimValue] nvarchar(max) NULL,
      CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
      CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
  );
END;

IF OBJECT_ID(N'[dbo].[AspNetUserClaims]', 'U') IS NULL
BEGIN
  CREATE TABLE [dbo].[AspNetUserClaims] (
      [Id] int NOT NULL IDENTITY(1,1),
      [UserId] int NOT NULL,
      [ClaimType] nvarchar(max) NULL,
      [ClaimValue] nvarchar(max) NULL,
      CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
      CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
  );
END;

IF OBJECT_ID(N'[dbo].[AspNetUserLogins]', 'U') IS NULL
BEGIN
  CREATE TABLE [dbo].[AspNetUserLogins] (
      [LoginProvider] nvarchar(450) NOT NULL,
      [ProviderKey] nvarchar(450) NOT NULL,
      [ProviderDisplayName] nvarchar(max) NULL,
      [UserId] int NOT NULL,
      CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
      CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
  );
END;

IF OBJECT_ID(N'[dbo].[AspNetUserRoles]', 'U') IS NULL
BEGIN
  CREATE TABLE [dbo].[AspNetUserRoles] (
      [UserId] int NOT NULL,
      [RoleId] int NOT NULL,
      CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
      CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
      CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
  );
END;

IF OBJECT_ID(N'[dbo].[AspNetUserTokens]', 'U') IS NULL
BEGIN
  CREATE TABLE [dbo].[AspNetUserTokens] (
      [UserId] int NOT NULL,
      [LoginProvider] nvarchar(450) NOT NULL,
      [Name] nvarchar(450) NOT NULL,
      [Value] nvarchar(max) NULL,
      CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
      CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
  );
END;

IF OBJECT_ID(N'[dbo].[RefreshTokens]', 'U') IS NULL
BEGIN
  CREATE TABLE [dbo].[RefreshTokens] (
      [Token] nvarchar(450) NOT NULL,
      [JwtId] nvarchar(max) NOT NULL,
      [CreationDate] datetime2 NOT NULL,
      [ExpiryDate] datetime2 NOT NULL,
      [Used] bit NOT NULL,
      [Invalidated] bit NOT NULL,
      [UserId] int NOT NULL,
      CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Token]),
      CONSTRAINT [FK_RefreshTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
  );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AspNetRoleClaims_RoleId' AND object_id = OBJECT_ID(N'[dbo].[AspNetRoleClaims]'))
  CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims] ([RoleId]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'RoleNameIndex' AND object_id = OBJECT_ID(N'[dbo].[AspNetRoles]'))
  CREATE UNIQUE INDEX [RoleNameIndex] ON [dbo].[AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AspNetUserClaims_UserId' AND object_id = OBJECT_ID(N'[dbo].[AspNetUserClaims]'))
  CREATE INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims] ([UserId]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AspNetUserLogins_UserId' AND object_id = OBJECT_ID(N'[dbo].[AspNetUserLogins]'))
  CREATE INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins] ([UserId]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AspNetUserRoles_RoleId' AND object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]'))
  CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles] ([RoleId]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'EmailIndex' AND object_id = OBJECT_ID(N'[dbo].[AspNetUsers]'))
  CREATE INDEX [EmailIndex] ON [dbo].[AspNetUsers] ([NormalizedEmail]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UserNameIndex' AND object_id = OBJECT_ID(N'[dbo].[AspNetUsers]'))
  CREATE UNIQUE INDEX [UserNameIndex] ON [dbo].[AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RefreshTokens_UserId' AND object_id = OBJECT_ID(N'[dbo].[RefreshTokens]'))
  CREATE INDEX [IX_RefreshTokens_UserId] ON [dbo].[RefreshTokens] ([UserId]);
";
    }

  }
}
