using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MinimalApi.DataAccess.Context
{
    class PostDbContextDesignTimeContextFactory : IDesignTimeDbContextFactory<PostDbContext>
    {
        public PostDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<PostDbContext>();
            options.UseSqlServer("Server=.;Database=MinimalApi.Local;Integrated Security=true;MultipleActiveResultSets=true");
            return new PostDbContext(new DoNothingInterceptor(), options.Options);
        }
    }
    internal sealed class DoNothingInterceptor : DbConnectionInterceptor, ISetSessionContextToUserSubIdInterceptor { }
}