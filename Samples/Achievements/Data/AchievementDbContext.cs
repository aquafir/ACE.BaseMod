using Microsoft.EntityFrameworkCore;
using Achievements.Domain;

namespace Achievements.Data;

public class AchievementDbContext : DbContext
{
    public DbSet<Adventurer> Adventurers { get; set; }
    public DbSet<Dungeon> Dungeons { get; set; }
    public DbSet<Kill> Kills { get; set; }
    public DbSet<Land> Lands { get; set; }

    public AchievementDbContext()
    {
    }

    public AchievementDbContext(DbContextOptions<AchievementDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        if (!optionsBuilder.IsConfigured)
        {
            //var config = Common.ConfigManager.Settings.MySql.World;

            var connectionString = $"server={Settings.Host};port={Settings.Port};user={Settings.Username};password={Settings.Password};database={Settings.Database};TreatTinyAsBoolean=False;SslMode=None;AllowPublicKeyRetrieval=true;ApplicationName=ACEmulator";

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), builder =>
            {
                builder.EnableRetryOnFailure(10);
            });
        }
    }
}
