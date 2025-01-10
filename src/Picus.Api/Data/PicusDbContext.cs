using Microsoft.EntityFrameworkCore;
using Picus.Api.Models;
using Picus.Api.Models.Enums;
using Picus.Api.Models.SportsDb;

namespace Picus.Api.Data;

public class PicusDbContext : DbContext
{
    private readonly bool _seedData;

    public PicusDbContext(DbContextOptions<PicusDbContext> options, bool seedData = true) : base(options)
    {
        _seedData = seedData;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Models.Game> Games => Set<Models.Game>();
    public DbSet<Pick> Picks => Set<Pick>();
    public DbSet<League> Leagues => Set<League>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Auth0Id)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Game configuration
        modelBuilder.Entity<Models.Game>()
            .HasOne(g => g.HomeTeam)
            .WithMany(t => t.HomeGames)
            .HasForeignKey(g => g.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Game>()
            .HasOne(g => g.AwayTeam)
            .WithMany(t => t.AwayGames)
            .HasForeignKey(g => g.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Game>()
            .HasOne(g => g.WinningTeam)
            .WithMany();

        // Pick configuration
        modelBuilder.Entity<Pick>()
            .HasOne(p => p.User)
            .WithMany(u => u.Picks)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pick>()
            .HasOne(p => p.Game)
            .WithMany(g => g.Picks)
            .HasForeignKey(p => p.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pick>()
            .HasOne(p => p.SelectedTeam)
            .WithMany(t => t.Picks)
            .HasForeignKey(p => p.SelectedTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pick>()
            .Property(p => p.Notes)
            .HasColumnType("text")
            .IsRequired(false);

        // League configuration
        modelBuilder.Entity<League>()
            .HasOne(l => l.AdminUser)
            .WithMany()
            .HasForeignKey(l => l.AdminUserId)
            .OnDelete(DeleteBehavior.Restrict);

        if (_seedData)
        {
            SeedTeams(modelBuilder);
        }
    }

    private void SeedTeams(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Team>().HasData(
            // AFC North
            new Team { Id = 1, ExternalTeamId = "134922", Name = "Ravens", Abbreviation = "BAL", City = "Baltimore", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/bal.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/bal.png", PrimaryColor = "#241773", SecondaryColor = "#000000", TertiaryColor = "#9E7C0C", Conference = ConferenceType.AFC, Division = DivisionType.North },
            new Team { Id = 2, ExternalTeamId = "134923", Name = "Bengals", Abbreviation = "CIN", City = "Cincinnati", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/cin.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/cin.png", PrimaryColor = "#FB4F14", SecondaryColor = "#000000", TertiaryColor = "#FFFFFF", Conference = ConferenceType.AFC, Division = DivisionType.North },
            new Team { Id = 3, ExternalTeamId = "134924", Name = "Browns", Abbreviation = "CLE", City = "Cleveland", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/cle.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/cle.png", PrimaryColor = "#311D00", SecondaryColor = "#FF3C00", TertiaryColor = "#FFFFFF", Conference = ConferenceType.AFC, Division = DivisionType.North },
            new Team { Id = 4, ExternalTeamId = "134925", Name = "Steelers", Abbreviation = "PIT", City = "Pittsburgh", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/pit.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/pit.png", PrimaryColor = "#FFB612", SecondaryColor = "#101820", TertiaryColor = "#A5ACAF", Conference = ConferenceType.AFC, Division = DivisionType.North },

            // AFC South
            new Team { Id = 5, ExternalTeamId = "134926", Name = "Texans", Abbreviation = "HOU", City = "Houston", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/hou.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/hou.png", PrimaryColor = "#03202F", SecondaryColor = "#A71930", TertiaryColor = "#FFFFFF", Conference = ConferenceType.AFC, Division = DivisionType.South },
            new Team { Id = 6, ExternalTeamId = "134927", Name = "Colts", Abbreviation = "IND", City = "Indianapolis", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/ind.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/ind.png", PrimaryColor = "#002C5F", SecondaryColor = "#A2AAAD", TertiaryColor = "#FFFFFF", Conference = ConferenceType.AFC, Division = DivisionType.South },
            new Team { Id = 7, ExternalTeamId = "134928", Name = "Jaguars", Abbreviation = "JAX", City = "Jacksonville", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/jax.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/jax.png", PrimaryColor = "#006778", SecondaryColor = "#9F792C", TertiaryColor = "#000000", Conference = ConferenceType.AFC, Division = DivisionType.South },
            new Team { Id = 8, ExternalTeamId = "134929", Name = "Titans", Abbreviation = "TEN", City = "Tennessee", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/ten.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/ten.png", PrimaryColor = "#0C2340", SecondaryColor = "#4B92DB", TertiaryColor = "#C8102E", Conference = ConferenceType.AFC, Division = DivisionType.South },

            // AFC East
            new Team { Id = 9, ExternalTeamId = "134918", Name = "Bills", Abbreviation = "BUF", City = "Buffalo", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/buf.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/buf.png", PrimaryColor = "#00338D", SecondaryColor = "#C60C30", TertiaryColor = "#FFFFFF", Conference = ConferenceType.AFC, Division = DivisionType.East },
            new Team { Id = 10, ExternalTeamId = "134919", Name = "Dolphins", Abbreviation = "MIA", City = "Miami", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/mia.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/mia.png", PrimaryColor = "#008E97", SecondaryColor = "#FC4C02", TertiaryColor = "#005778", Conference = ConferenceType.AFC, Division = DivisionType.East },
            new Team { Id = 11, ExternalTeamId = "134920", Name = "Patriots", Abbreviation = "NE", City = "New England", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/ne.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/ne.png", PrimaryColor = "#002244", SecondaryColor = "#C60C30", TertiaryColor = "#B0B7BC", Conference = ConferenceType.AFC, Division = DivisionType.East },
            new Team { Id = 12, ExternalTeamId = "134921", Name = "Jets", Abbreviation = "NYJ", City = "New York", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/nyj.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/nyj.png", PrimaryColor = "#125740", SecondaryColor = "#000000", TertiaryColor = "#FFFFFF", Conference = ConferenceType.AFC, Division = DivisionType.East },

            // AFC West
            new Team { Id = 13, ExternalTeamId = "134930", Name = "Broncos", Abbreviation = "DEN", City = "Denver", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/den.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/den.png", PrimaryColor = "#FB4F14", SecondaryColor = "#002244", TertiaryColor = "#FFFFFF", Conference = ConferenceType.AFC, Division = DivisionType.West },
            new Team { Id = 14, ExternalTeamId = "134931", Name = "Chiefs", Abbreviation = "KC", City = "Kansas City", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/kc.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/kc.png", PrimaryColor = "#E31837", SecondaryColor = "#FFB81C", TertiaryColor = "#FFFFFF", Conference = ConferenceType.AFC, Division = DivisionType.West },
            new Team { Id = 15, ExternalTeamId = "134932", Name = "Raiders", Abbreviation = "LV", City = "Las Vegas", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/lv.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/lv.png", PrimaryColor = "#000000", SecondaryColor = "#A5ACAF", TertiaryColor = "#FFFFFF", Conference = ConferenceType.AFC, Division = DivisionType.West },
            new Team { Id = 16, ExternalTeamId = "135908", Name = "Chargers", Abbreviation = "LAC", City = "Los Angeles", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/lac.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/lac.png", PrimaryColor = "#0080C6", SecondaryColor = "#FFC20E", TertiaryColor = "#FFFFFF", Conference = ConferenceType.AFC, Division = DivisionType.West },

            // NFC North
            new Team { Id = 17, ExternalTeamId = "134938", Name = "Bears", Abbreviation = "CHI", City = "Chicago", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/chi.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/chi.png", PrimaryColor = "#0B162A", SecondaryColor = "#C83803", TertiaryColor = "#FFFFFF", Conference = ConferenceType.NFC, Division = DivisionType.North },
            new Team { Id = 18, ExternalTeamId = "134939", Name = "Lions", Abbreviation = "DET", City = "Detroit", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/det.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/det.png", PrimaryColor = "#0076B6", SecondaryColor = "#B0B7BC", TertiaryColor = "#000000", Conference = ConferenceType.NFC, Division = DivisionType.North },
            new Team { Id = 19, ExternalTeamId = "134940", Name = "Packers", Abbreviation = "GB", City = "Green Bay", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/gb.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/gb.png", PrimaryColor = "#203731", SecondaryColor = "#FFB612", TertiaryColor = "#FFFFFF", Conference = ConferenceType.NFC, Division = DivisionType.North },
            new Team { Id = 20, ExternalTeamId = "134941", Name = "Vikings", Abbreviation = "MIN", City = "Minnesota", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/min.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/min.png", PrimaryColor = "#4F2683", SecondaryColor = "#FFC62F", TertiaryColor = "#FFFFFF", Conference = ConferenceType.NFC, Division = DivisionType.North },

            // NFC South
            new Team { Id = 21, ExternalTeamId = "134942", Name = "Falcons", Abbreviation = "ATL", City = "Atlanta", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/atl.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/atl.png", PrimaryColor = "#A71930", SecondaryColor = "#000000", TertiaryColor = "#A5ACAF", Conference = ConferenceType.NFC, Division = DivisionType.South },
            new Team { Id = 22, ExternalTeamId = "134943", Name = "Panthers", Abbreviation = "CAR", City = "Carolina", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/car.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/car.png", PrimaryColor = "#0085CA", SecondaryColor = "#101820", TertiaryColor = "#BFC0BF", Conference = ConferenceType.NFC, Division = DivisionType.South },
            new Team { Id = 23, ExternalTeamId = "134944", Name = "Saints", Abbreviation = "NO", City = "New Orleans", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/no.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/no.png", PrimaryColor = "#D3BC8D", SecondaryColor = "#101820", TertiaryColor = "#FFFFFF", Conference = ConferenceType.NFC, Division = DivisionType.South },
            new Team { Id = 24, ExternalTeamId = "134945", Name = "Buccaneers", Abbreviation = "TB", City = "Tampa Bay", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/tb.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/tb.png", PrimaryColor = "#D50A0A", SecondaryColor = "#34302B", TertiaryColor = "#B1BABF", Conference = ConferenceType.NFC, Division = DivisionType.South },

            // NFC East
            new Team { Id = 25, ExternalTeamId = "134946", Name = "Cowboys", Abbreviation = "DAL", City = "Dallas", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/dal.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/dal.png", PrimaryColor = "#003594", SecondaryColor = "#869397", TertiaryColor = "#FFFFFF", Conference = ConferenceType.NFC, Division = DivisionType.East },
            new Team { Id = 26, ExternalTeamId = "134935", Name = "Giants", Abbreviation = "NYG", City = "New York", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/nyg.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/nyg.png", PrimaryColor = "#0B2265", SecondaryColor = "#A71930", TertiaryColor = "#A5ACAF", Conference = ConferenceType.NFC, Division = DivisionType.East },
            new Team { Id = 27, ExternalTeamId = "134936", Name = "Eagles", Abbreviation = "PHI", City = "Philadelphia", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/phi.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/phi.png", PrimaryColor = "#004C54", SecondaryColor = "#A5ACAF", TertiaryColor = "#ACC0C6", Conference = ConferenceType.NFC, Division = DivisionType.East },
            new Team { Id = 28, ExternalTeamId = "134949", Name = "Commanders", Abbreviation = "WAS", City = "Washington", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/was.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/was.png", PrimaryColor = "#5A1414", SecondaryColor = "#FFB612", TertiaryColor = "#FFFFFF", Conference = ConferenceType.NFC, Division = DivisionType.East },

            // NFC West
            new Team { Id = 29, ExternalTeamId = "134946", Name = "Cardinals", Abbreviation = "ARI", City = "Arizona", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/ari.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/ari.png", PrimaryColor = "#97233F", SecondaryColor = "#000000", TertiaryColor = "#FFB612", Conference = ConferenceType.NFC, Division = DivisionType.West },
            new Team { Id = 30, ExternalTeamId = "135907", Name = "Rams", Abbreviation = "LAR", City = "Los Angeles", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/lar.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/lar.png", PrimaryColor = "#003594", SecondaryColor = "#FFA300", TertiaryColor = "#FFFFFF", Conference = ConferenceType.NFC, Division = DivisionType.West },
            new Team { Id = 31, ExternalTeamId = "134948", Name = "49ers", Abbreviation = "SF", City = "San Francisco", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/sf.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/sf.png", PrimaryColor = "#AA0000", SecondaryColor = "#B3995D", TertiaryColor = "#FFFFFF", Conference = ConferenceType.NFC, Division = DivisionType.West },
            new Team { Id = 32, ExternalTeamId = "134949", Name = "Seahawks", Abbreviation = "SEA", City = "Seattle", IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/sea.png", BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/sea.png", PrimaryColor = "#002244", SecondaryColor = "#69BE28", TertiaryColor = "#A5ACAF", Conference = ConferenceType.NFC, Division = DivisionType.West }
        );
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
