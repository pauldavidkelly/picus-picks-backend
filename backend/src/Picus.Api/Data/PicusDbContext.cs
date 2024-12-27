using Microsoft.EntityFrameworkCore;
using Picus.Api.Models;

namespace Picus.Api.Data;

public class PicusDbContext : DbContext
{
    public PicusDbContext(DbContextOptions<PicusDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Pick> Picks => Set<Pick>();
    public DbSet<League> Leagues => Set<League>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Auth0Id)
            .IsUnique();

        // Game configuration
        modelBuilder.Entity<Game>()
            .HasOne(g => g.HomeTeam)
            .WithMany(t => t.HomeGames)
            .HasForeignKey(g => g.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.AwayTeam)
            .WithMany(t => t.AwayGames)
            .HasForeignKey(g => g.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.WinningTeam)
            .WithMany()
            .HasForeignKey(g => g.WinningTeamId)
            .OnDelete(DeleteBehavior.NoAction);

        // Pick configuration
        modelBuilder.Entity<Pick>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Pick>()
            .HasOne(p => p.Game)
            .WithMany()
            .HasForeignKey(p => p.GameId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Pick>()
            .HasOne(p => p.SelectedTeam)
            .WithMany()
            .HasForeignKey(p => p.SelectedTeamId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Pick>()
            .HasOne(p => p.PickedTeam)
            .WithMany()
            .HasForeignKey(p => p.PickedTeamId)
            .OnDelete(DeleteBehavior.NoAction);

        // League configuration
        modelBuilder.Entity<League>()
            .HasOne(l => l.AdminUser)
            .WithMany()
            .HasForeignKey(l => l.AdminUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (
                e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var entity = (BaseEntity)entityEntry.Entity;

            if (entityEntry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
