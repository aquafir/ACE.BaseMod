namespace Achievements;

public class AchievementsDatabase
{
    //Use a DI DbContextPool to avoid recreation
    //https://learn.microsoft.com/en-us/ef/core/performance/advanced-performance-topics?tabs=with-di%2Cexpression-api-with-constant#dbcontext-pooling    
    private static readonly IDbContextFactory<AchievementsContext> _factory =
        PatchClass.ServiceProvider.GetRequiredService<IDbContextFactory<AchievementsContext>>();

    public static AchievementsContext CreateContext() => _factory.CreateDbContext();
}