namespace TodoAPI.Infrastructures.Data;

public static class EFCoreConfig
{
    public static void AddEFCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LabContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Lab"))
        );
        services.AddDatabaseDeveloperPageExceptionFilter();
    }
}
