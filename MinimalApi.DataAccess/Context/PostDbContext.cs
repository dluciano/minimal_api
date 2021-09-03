using Microsoft.EntityFrameworkCore;
using MinimalApi.DataAccess.Context.Models;

namespace MinimalApi.DataAccess.Context
{
    internal sealed class PostDbContext : DbContext
    {
        private readonly ISetSessionContextToUserSubIdInterceptor _sessionInterceptor;

        public PostDbContext(ISetSessionContextToUserSubIdInterceptor sessionInterceptor,
            DbContextOptions<PostDbContext> dbContextOptions) : base(dbContextOptions) =>
            _sessionInterceptor = sessionInterceptor;

        public DbSet<Post> Posts => Set<Post>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.AddInterceptors(_sessionInterceptor);

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
