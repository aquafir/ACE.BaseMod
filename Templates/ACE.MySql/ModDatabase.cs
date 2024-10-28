namespace ACE.MySql;

public class ModDatabase
{
    //Use a DI DbContextPool to avoid recreation
    //https://learn.microsoft.com/en-us/ef/core/performance/advanced-performance-topics?tabs=with-di%2Cexpression-api-with-constant#dbcontext-pooling    
    private static readonly IDbContextFactory<ModContext> _factory =
        PatchClass.ServiceProvider.GetRequiredService<IDbContextFactory<ModContext>>();

        public static ModContext CreateContext() => _factory.CreateDbContext();
}