using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Core.Models;
using WetHands.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WetHands.Core.Basic;
using WetHands.Core.Models;

namespace Infrastructure.Database.SeedData
{
  public class DataContextSeed
  {

    public static async Task SeedDataAsync(AppDbContext context, ILoggerFactory loggerFactory)
    {
      try
      {

        await EnsureMassageCatalogSchemaAsync(context);

        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        // Seed country/city catalogs (used by MassagePlaces).
        var existingCountries = await context.Countries.AsNoTracking().ToListAsync();
        if (existingCountries.Count == 0)
        {
          var itemsData = File.ReadAllText(path + @"/Seed/SeedData/countrys.json");
          var items = JsonSerializer.Deserialize<List<Country>>(itemsData);
          if (items != null)
          {
            foreach (var item in items)
            {
              context.Countries.Add(item);
            }
            await context.SaveChangesAsync();
          }
        }

        var existingCities = await context.Cities.AsNoTracking().ToListAsync();
        if (existingCities.Count == 0)
        {
          var itemsData = File.ReadAllText(path + @"/Seed/SeedData/citys.json");
          var items = JsonSerializer.Deserialize<List<City>>(itemsData);
          if (items != null)
          {
            foreach (var item in items)
            {
              context.Cities.Add(item);
            }
            await context.SaveChangesAsync();
          }
        }

        // Seed selectable massage categories (server-administered list).
        var existingCategories = await context.MassageCategories.AsNoTracking().ToListAsync();
        if (existingCategories.Count == 0)
        {
          var itemsData = File.ReadAllText(path + @"/Seed/SeedData/massage-categories.json");
          var items = JsonSerializer.Deserialize<List<MassageCategory>>(itemsData);
          if (items != null)
          {
            foreach (var item in items)
            {
              if (item == null) continue;
              if (string.IsNullOrWhiteSpace(item.Name)) continue;
              item.Name = item.Name.Trim();
              item.GroupName = string.IsNullOrWhiteSpace(item.GroupName) ? null : item.GroupName.Trim();
              context.MassageCategories.Add(item);
            }
            await context.SaveChangesAsync();
          }
        }

        // /// <summary>
        // /// adding investors
        // /// </summary>
        // /// <returns></returns>
        // if (!context.Investors.Any())
        // {
        //   var itemsData = File.ReadAllText(path + @"/Seed/SeedData/investors.json");
        //   var items = JsonSerializer.Deserialize<List<Investor>>(itemsData);
        //   foreach (var item in items)
        //   {
        //     context.Investors.Add(item);
        //   }
        //   await context.SaveChangesAsync();
        // }

        // /// <summary>
        // /// seeding amenities
        // /// </summary>
        // /// <returns></returns>
        // if (!context.Amenities.Any())
        // {
        //   var itemsData = File.ReadAllText(path + @"/Seed/SeedData/amenities.json");
        //   var items = JsonSerializer.Deserialize<List<Amenitie>>(itemsData);
        //   foreach (var item in items)
        //   {
        //     context.Amenities.Add(item);
        //   }
        //   await context.SaveChangesAsync();
        // }

        // /// <summary>
        // /// proposal types seed
        // /// </summary>
        // /// <returns></returns>
        // if (!context.ProposalTypes.Any())
        // {
        //   var itemsData = File.ReadAllText(path + @"/Seed/SeedData/types.json");
        //   var items = JsonSerializer.Deserialize<List<ProposalType>>(itemsData);
        //   foreach (var item in items)
        //   {
        //     context.ProposalTypes.Add(item);
        //   }
        //   await context.SaveChangesAsync();
        // }


        // /// <summary>
        // /// proposal description translated seeed
        // /// </summary>
        // /// <returns></returns>
        // if (!context.ProposalDescriptionTranaltaions.Any())
        // {
        //   var itemsData = File.ReadAllText(path + @"/Seed/SeedData/proposal-descriptions.json");
        //   var items = JsonSerializer.Deserialize<List<ProposalDescriptionTranaltaion>>(itemsData);
        //   foreach (var item in items)
        //   {
        //     context.ProposalDescriptionTranaltaions.Add(item);
        //   }
        //   await context.SaveChangesAsync();
        // }



        /// <summary>
        /// seeding actions for history log
        /// </summary>
        /// <returns></returns>
        // if (!context.ProposalActions.Any())
        // {
        //   // var itemsData = File.ReadAllText(path + @"/Seed/SeedData/proposals.json");
        //   // var items = JsonSerializer.Deserialize<List<Proposal>>(itemsData);

        //   var action1 = new ProposalAction()
        //   {
        //     ActionName = "Объект готов к оформлению и покупке",
        //     ActionDescription = "Идет оформление сделки риэлтором",
        //     EnrolledDate = DateTime.Now.AddDays(42).AddHours(11).AddMinutes(55),
        //     ProfileId = 1
        //   };
        //   var action2 = new ProposalAction()
        //   {
        //     ActionName = "Пользователь Matew Green готов вложиться",
        //     ActionDescription = "Предполагается к вложению $40000",
        //     EnrolledDate = DateTime.Now.AddDays(40).AddHours(5).AddMinutes(3),
        //     ProfileId = 1
        //   };
        //   var action3 = new ProposalAction()
        //   {
        //     ActionName = "Пользователь Andrey Kamenev увеличил долю",
        //     ActionDescription = "Предполагается к вложению $40000",
        //     EnrolledDate = DateTime.Now.AddDays(33).AddHours(22).AddMinutes(31),
        //     ProfileId = 1
        //   };
        //   var action4 = new ProposalAction()
        //   {
        //     ActionName = "Пользователь Elen Black готов вложиться",
        //     ActionDescription = "Предполагается к вложению $20000",
        //     EnrolledDate = DateTime.Now.AddDays(24).AddHours(22).AddMinutes(12),
        //     ProfileId = 1
        //   };
        //   var action5 = new ProposalAction()
        //   {
        //     ActionName = "Пользователь Andrey Kamenev готов вложиться",
        //     ActionDescription = "Предполагается к вложению $20000",
        //     EnrolledDate = DateTime.Now.AddDays(30).AddHours(13).AddMinutes(41),
        //     ProfileId = 1
        //   };
        //   var action6 = new ProposalAction()
        //   {
        //     ActionName = "Объект доступен для инвестирования",
        //     ActionDescription = "Осталось для покупки 100%",
        //     EnrolledDate = DateTime.Now.AddDays(11).AddHours(10).AddMinutes(12),
        //     ProfileId = 1
        //   };

        //   var items = new List<ProposalAction>
        //   {
        //     action1,
        //     action2,
        //     action3,
        //     action4,
        //     action5,
        //     action6
        //   };


        //   foreach (var item in items)
        //   {
        //     context.ProposalActions.Add(item);
        //   };

        //   await context.SaveChangesAsync();
        // }

        /// <summary>
        /// seedeing orders from investors
        /// </summary>
        /// <returns></returns>
        var serviceTypes = await context.ServiceTypes.ToListAsync();
        var requiredServiceTypes = new[]
        {
          new ServiceType { Code = "laundry", Name = "Прачечная" },
          new ServiceType { Code = "dry_cleaning", Name = "Химчистка" },
          new ServiceType { Code = "restaurant", Name = "Ресторан" }
        };

        foreach (var serviceType in requiredServiceTypes)
        {
          if (!serviceTypes.Any(st => st.Code == serviceType.Code))
          {
            context.ServiceTypes.Add(new ServiceType
            {
              Code = serviceType.Code,
              Name = serviceType.Name
            });
          }
        }

        if (context.ChangeTracker.HasChanges())
        {
          await context.SaveChangesAsync();
        }

        var requestStatuses = await context.OrderStatuses.ToArrayAsync();
        // if (requestStatuses.Length == 0)
        if (requestStatuses.Length == 0)
        {

          var statuses = new[]
          {
            new OrderStatus{ Icon = "tw_doc_black", Name = "Черновик", Description = "Заявка оформлена отелем и сохранена. Видна как черновик до отправки исполнителю." },
            new OrderStatus{ Icon = "tw_launch_black", Name = "Отправлено", Description = "Заявка отправлена в прачечную и ожидает подтверждения принятия." },
            new OrderStatus{ Icon = "tw_users_black", Name = "Принято исполнителем", Description = "Прачечная подтвердила получение заявки и назначила внутренний номер." },
            new OrderStatus{ Icon = "tw_tag", Name = "На приемке", Description = "Бельё принято на складе/приемке. Идёт сортировка и подготовка к обработке." },
            new OrderStatus{ Icon = "tw_chip", Name = "В работе", Description = "Заявка запущена в производственный цикл." },
            new OrderStatus{ Icon = "tw_chem", Name = "Стирка", Description = "Изделия на этапе стирки/моечной обработки." },
            new OrderStatus{ Icon = "tw_chip", Name = "Сушка", Description = "Изделия проходят сушку/подготовку к глажке." },
            new OrderStatus{ Icon = "tw_chip", Name = "Глажение", Description = "Изделия на этапе глажения/прессования." },
            new OrderStatus{ Icon = "tw_location", Name = "Готово к выдаче", Description = "Заказ полностью подготовлен, упакован и ожидает отгрузки." },
            new OrderStatus{ Icon = "tw_location", Name = "Назначен водитель", Description = "Назначен водитель для доставки. Доступны ФИО и контактные данные." },
            new OrderStatus{ Icon = "tw_location", Name = "В доставке", Description = "Заказ передан водителю и находится в пути к клиенту." },
            new OrderStatus{ Icon = "tw_location", Name = "Доставлено", Description = "Заказ передан отелю. Ожидает финального подтверждения и закрытия." },
            new OrderStatus{ Icon = "done", Name = "Завершено", Description = "Заказ закрыт. Работы и расчёты завершены." },

            // служебные ветки
            new OrderStatus{ Icon = "tw_launch_black", Name = "Отложено", Description = "Выполнение заказа временно приостановлено по согласованию с клиентом." },
            new OrderStatus{ Icon = "tw_launch_black", Name = "Отменено", Description = "Заявка отменена (клиентом или исполнителем) до завершения работ." }
          };

          foreach (var status in statuses)
          {
            await context.OrderStatuses.AddAsync(status);
            await context.SaveChangesAsync();
          }


        }


        // Seed OrderItemTypes catalog
        var existingItemTypes = await context.OrderItemTypes.ToArrayAsync();
        if (existingItemTypes.Length == 0)
        {
          var names = new[]
          {
            "Наволочки",
            "Простыня 1,5 сп",
            "Простыня 2-х сп",
            "Пододеяльник 1,5 сп",
            "Пододеяльник 2-х сп",
            "Махра баня",
            "Махра лицевое",
            "Махра ножки",
            "Халат вафельный",
            "Халат махровый",
            "Подушки",
            "Покрывало",
            "Цветное бельё (простыни/под./наволочки)",
            "Серое бельё",
            "Детский комплект",
            "Одеяло",
            "Саше",
            "Плед",
            "Наматрасник"
          };

          var rnd = new Random();
          foreach (var n in names)
          {
            var entity = new OrderItemType
            {
              Name = n,
              Weight = rnd.Next(300, 1101)
            };
            await context.OrderItemTypes.AddAsync(entity);
            await context.SaveChangesAsync();
          }
        }


        /// <summary>
        /// seedeing orders from investors
        /// </summary>
        /// <returns></returns>
        var proposalStatuses = await context.ProposalProfileStatuses.ToArrayAsync();
        // if (requestStatuses.Length == 0)
        if (requestStatuses.Length == 0)
        {
          var itemsData = File.ReadAllText(path + @"/Seed/SeedData/proposal-statuses.json");
          var items = JsonSerializer.Deserialize<List<ProposalProfileStatus>>(itemsData);
          foreach (var item in items)
          {
            context.ProposalProfileStatuses.Add(item);
          }
          await context.SaveChangesAsync();
        }


      }
      catch (Exception ex)
      {
        var logger = loggerFactory.CreateLogger<DataContextSeed>();
        logger.LogError(ex.Message);
      }
    }

    /// <summary>
    /// This project historically shipped without EF Core migrations and relied on EnsureCreated().
    /// EnsureCreated() does not update existing schemas, so we patch required tables/columns
    /// for the massage catalog on startup to avoid 500 errors in production.
    /// </summary>
    private static async Task EnsureMassageCatalogSchemaAsync(AppDbContext context)
    {
      // SQL Server dialect.
      // Create Countries/Cities/MassagePlaceImages tables if missing.
      // Add missing columns to MassagePlaces and relax MainImage nullability (legacy was NOT NULL base64).
      var sql = @"
IF OBJECT_ID(N'[dbo].[Countries]', 'U') IS NULL
BEGIN
  CREATE TABLE [dbo].[Countries] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(128) NOT NULL,
    [CountryIconPath] NVARCHAR(128) NULL
  );
  CREATE UNIQUE INDEX IX_Countries_Name ON [dbo].[Countries]([Name]);
END;

IF OBJECT_ID(N'[dbo].[Cities]', 'U') IS NULL
BEGIN
  CREATE TABLE [dbo].[Cities] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(128) NOT NULL,
    [CountryId] INT NOT NULL
  );
  CREATE UNIQUE INDEX IX_Cities_CountryId_Name ON [dbo].[Cities]([CountryId], [Name]);
END;

IF OBJECT_ID(N'[dbo].[ServiceTypes]', 'U') IS NULL
BEGIN
  CREATE TABLE [dbo].[ServiceTypes] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Code] NVARCHAR(64) NOT NULL,
    [Name] NVARCHAR(256) NOT NULL
  );
  CREATE UNIQUE INDEX IX_ServiceTypes_Code ON [dbo].[ServiceTypes]([Code]);
END;

IF OBJECT_ID(N'[dbo].[OrderStatuses]', 'U') IS NULL
BEGIN
  CREATE TABLE [dbo].[OrderStatuses] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(256) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Icon] NVARCHAR(128) NULL
  );
END;

IF OBJECT_ID(N'[dbo].[OrderItemTypes]', 'U') IS NULL
BEGIN
  CREATE TABLE [dbo].[OrderItemTypes] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Weight] INT NOT NULL CONSTRAINT DF_OrderItemTypes_Weight DEFAULT(0),
    [Name] NVARCHAR(256) NULL
  );
END;

IF OBJECT_ID(N'[dbo].[MassagePlaceImages]', 'U') IS NULL
BEGIN
  CREATE TABLE [dbo].[MassagePlaceImages] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [MassagePlaceId] INT NOT NULL,
    [IsMain] BIT NOT NULL CONSTRAINT DF_MassagePlaceImages_IsMain DEFAULT(0),
    [EnrolledDate] DATETIME2 NOT NULL CONSTRAINT DF_MassagePlaceImages_EnrolledDate DEFAULT(SYSUTCDATETIME()),
    [FileName] NVARCHAR(256) NULL,
    [FileType] NVARCHAR(128) NULL,
    [DocByte] VARBINARY(MAX) NULL,
    [Size] INT NULL
  );
  CREATE INDEX IX_MassagePlaceImages_MassagePlaceId_IsMain ON [dbo].[MassagePlaceImages]([MassagePlaceId], [IsMain]);
END;

IF OBJECT_ID(N'[dbo].[MassageCategories]', 'U') IS NULL
BEGIN
  CREATE TABLE [dbo].[MassageCategories] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(256) NOT NULL,
    [GroupName] NVARCHAR(256) NULL,
    [IsActive] BIT NOT NULL CONSTRAINT DF_MassageCategories_IsActive DEFAULT(1),
    [SortOrder] INT NOT NULL CONSTRAINT DF_MassageCategories_SortOrder DEFAULT(0)
  );
  CREATE UNIQUE INDEX IX_MassageCategories_GroupName_Name ON [dbo].[MassageCategories]([GroupName], [Name]);
END;

IF COL_LENGTH('dbo.MassagePlaces', 'CountryId') IS NULL
BEGIN
  ALTER TABLE [dbo].[MassagePlaces] ADD [CountryId] INT NULL;
END;

IF COL_LENGTH('dbo.MassagePlaces', 'CityId') IS NULL
BEGIN
  ALTER TABLE [dbo].[MassagePlaces] ADD [CityId] INT NULL;
END;

BEGIN TRY
  -- Legacy schema had MainImage NOT NULL (base64). Make it nullable.
  ALTER TABLE [dbo].[MassagePlaces] ALTER COLUMN [MainImage] NVARCHAR(MAX) NULL;
END TRY
BEGIN CATCH
  -- Ignore if column does not exist or type differs.
END CATCH;
";

      await context.Database.ExecuteSqlRawAsync(sql);
    }


  }
}
