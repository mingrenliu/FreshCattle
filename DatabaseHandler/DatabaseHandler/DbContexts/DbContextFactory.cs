using DatabaseHandler.DbContexts;
using DatabaseHandler.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatabaseHandler..DbContexts
{
    public class ScopeDbContextFactory<T>:IDbContextFactory<T> where T : DbContext<T>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IDbContextFactory<T> _factory;
        public ScopeDbContextFactory(ICurrentUser user,IDbContextFactory<T> factory)
        {
            _currentUser = user;
            _factory = factory;
        }

        public T CreateDbContext()
        {
            var context=_factory.CreateDbContext();
            context.CurrentUser=_currentUser;
            return context;
        }
    }
}