using HMS.Core.Contracts;
using HMS.InfraStructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HMS.Api.Extensions
{
    public static class WebApplicationRegister
    {
        public async static Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<HMSDbContext>();

            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
                await dbContext.Database.MigrateAsync();

            return app;
        }

        public static async Task<WebApplication> SeedingIdentityDataAsync(this WebApplication app)
        {
           await  using var scope = app.Services.CreateAsyncScope();
           var dataInitializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();
           await dataInitializer.InitializeDataAsync();

            return app;
        } 
    }
}
