using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.DataAccess.Context;

namespace MinimalApi.DataAccess
{
    public static class DbConfigurationHelper
    {
        public static async Task EnsureDatabaseMigrated(this IServiceCollection services, CancellationToken cancellationToken = default)
        {
            services.AddScoped<ISetSessionContextToUserSubIdInterceptor, DoNothingInterceptor>();
            using var serviceProvider = services.BuildServiceProvider();
            await using var context = serviceProvider.CreateAsyncScope();
            await using var dbContext = context.ServiceProvider.GetService<PostDbContext>();
            if (dbContext is null) throw new NullReferenceException("Cannot create database to check if migrations are applied");
            var pending = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false);
            var anyPending = pending.Any();
            if (anyPending) throw new Exception("The database has not been migrated correctly");
        }
    }
}
