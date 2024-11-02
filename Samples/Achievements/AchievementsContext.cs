namespace Achievements;

public class AchievementsContext(DbContextOptions<AchievementsContext> options) : DbContext(options)
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Set up model relationships if needed: https://learn.microsoft.com/en-us/ef/core/modeling/relationships
        base.OnModelCreating(modelBuilder);
    }
}

