using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using WetHands.Core.Models;
using WetHands.Core.Models.Items;
using WetHands.Core.TonModels;
using WetHands.Core;

namespace WetHands.Infrastructure.Database
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
      Database.EnsureCreated();
    }


    public DbSet<PaymentRequest> PaymentRequests { get; set; }
    public DbSet<PaymentRequestStatus> PaymentRequestStatuses { get; set; }
    public DbSet<PaymentRequestType> PaymentRequestTypes { get; set; }
    public DbSet<ProposalProfileStatus> ProposalProfileStatuses { get; set; }
    public DbSet<Favour> Favours { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<TonLocalTransaction> TonLocalTransactions { get; set; }
    public DbSet<UsdtTransferRequest> UsdtTransferRequests { get; set; } // <- ВАЖНО

    public DbSet<TrxPayment> TrxPayments { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderItemType> OrderItemTypes { get; set; }

    public DbSet<MassagePlace> MassagePlaces { get; set; }

    public DbSet<Picture> Pictures { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<NftPlot> NftPlots { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<NftSellRequest> NftSellRequests { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<ServiceType> ServiceTypes { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      var listToJsonConverter = new ValueConverter<List<string>, string>(
        v => JsonSerializer.Serialize(v ?? new List<string>(), (JsonSerializerOptions)null),
        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>());

      var listComparer = new ValueComparer<List<string>>(
        (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
        c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c == null ? new List<string>() : c.ToList());

      modelBuilder.Entity<MassagePlace>(entity =>
      {
        entity.HasIndex(p => p.Name).IsUnique();
        entity.Property(p => p.Name).IsRequired().HasMaxLength(256);
        entity.Property(p => p.Country).HasMaxLength(128);
        entity.Property(p => p.City).HasMaxLength(128);
        entity.Property(p => p.Description).IsRequired();
        entity.Property(p => p.Rating).HasDefaultValue(0);
        entity.Property(p => p.MainImage).IsRequired();

        entity.Property(p => p.Gallery)
          .HasConversion(listToJsonConverter)
          .Metadata.SetValueComparer(listComparer);

        entity.Property(p => p.Attributes)
          .HasConversion(listToJsonConverter)
          .Metadata.SetValueComparer(listComparer);
      });

      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    }

  }

}
