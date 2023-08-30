using DatabaseHandler.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DbHandlerTest
{
    public class TestDbContext : DbContext<TestDbContext>
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }
    }
}
