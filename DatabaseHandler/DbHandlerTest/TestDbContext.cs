using DatabaseHandler.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DbHandlerTest
{
    public partial class TestDbContext : DbContext<TestDbContext>
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }
    }
    public partial class Test1DbContext : DbContext<Test1DbContext>
    {
        public Test1DbContext(DbContextOptions<Test1DbContext> options) : base(options)
        {
        }
    }
}
