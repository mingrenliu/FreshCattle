using DatabaseHandler.DbContexts;
using DatabaseHandler.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatabaseHandler.Extensions
{
    public class DbContextFactory:IDbContextFactory<BaseDbContext>
    {
        private readonly CurrentUser _currentUser;
        private readonly IDbContextFactory<BaseDbContext> _factory;
        public DbContextFactory(CurrentUser user,IDbContextFactory<BaseDbContext> factory)
        {
            _currentUser = user;
            _factory = factory;
        }

        public BaseDbContext CreateDbContext()
        {
            var context=_factory.CreateDbContext();
            context.CurrentUser=_currentUser;
            return context;
        }
    }
}