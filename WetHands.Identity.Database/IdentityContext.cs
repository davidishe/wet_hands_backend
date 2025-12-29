using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Core.Domain;
using Core.Identity;
using Core.Models.Identity;

namespace WetHands.Identity.Database
{
  public class IdentityContext : IdentityDbContext<AppUser, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
  {
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
    {
      // The project historically shipped without EF Core migrations (see AppDbContext).
      // Ensure the Identity schema exists (AspNetUsers/AspNetRoles/...) to avoid runtime 500s.
      Database.EnsureCreated();
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    // public DbSet<Order> Orders { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<UserRole>(userRole =>
      {
        userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

        userRole.HasOne(ur => ur.Role)
          .WithMany(r => r.UserRoles)
          .HasForeignKey(ur => ur.RoleId)
          .IsRequired();

        userRole.HasOne(ur => ur.User)
          .WithMany(r => r.UserRoles)
          .HasForeignKey(ur => ur.UserId)
          .IsRequired();


      });



    }


  }
}