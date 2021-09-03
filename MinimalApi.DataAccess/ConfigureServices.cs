using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.DataAccess.Context;

namespace MinimalApi.DataAccess
{
    public static class ConfigureServices
    {
        public static IServiceCollection ConfigureMinimalApiDataAccessServices(this IServiceCollection services, string postConnectionString) =>
            services
                .AddScoped<ISetSessionContextToUserSubIdInterceptor, SetSessionContextToUserSubIdInterceptor>()
                .AddDbContext<PostDbContext>(config => config.UseSqlServer(postConnectionString))
                .AddScoped<IPostRepository, PostRepository>();
    }
}
