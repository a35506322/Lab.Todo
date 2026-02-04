using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace TodoAPI.IntegrationTest.Helpers;

/// <summary>
/// 自訂 WebApplicationFactory，用於整合測試
/// </summary>
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTest");

        builder.ConfigureServices(services =>
        {
            // fix: EF 8 to 9 migration database provider exception
            // https://jackyasgar.net/solved-ef-8-to-9-migration-database-provider-exception/
            var descriptor = services.SingleOrDefault(s =>
                s.ServiceType == typeof(IDbContextOptionsConfiguration<LabContext>)
            );
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<LabContext>(options =>
            {
                options.UseInMemoryDatabase("TodoAPI_IntegrationTest");
            });

            SeedTestData(services);
        });
    }

    private void SeedTestData(IServiceCollection services)
    {
        // 建立 Seed 測試用資料
        using var scope = services.BuildServiceProvider().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LabContext>();
        context.Database.EnsureCreated();

        // Seed 測試用 User 資料
        if (!context.User.Any())
        {
            context.User.Add(
                new User
                {
                    UserId = "admin",
                    Password = "admin123",
                    Role = RoleNames.Admin,
                }
            );
            context.SaveChanges();
        }
    }
}
