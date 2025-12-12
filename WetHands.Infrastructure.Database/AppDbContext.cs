using Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WetHands.Core.Models;
using WetHands.Core.TonModels;
using WetHands.Core;
using WetHands.Core.Models.Items;

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

    public DbSet<Picture> Pictures { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<NftPlot> NftPlots { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<NftSellRequest> NftSellRequests { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<ServiceType> ServiceTypes { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    }

  }

}
