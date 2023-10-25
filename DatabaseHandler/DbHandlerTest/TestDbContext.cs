using DatabaseHandler.DbContexts;

namespace Test.DbHandlerTest
{
    public partial class TestDbContext : DbContext<TestDbContext>
    {
        public TestDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<TestDbContext> options) : base(options)
        {
        }
    }

    public partial class Test1DbContext : DbContext<Test1DbContext>
    {
        public Test1DbContext(Microsoft.EntityFrameworkCore.DbContextOptions<Test1DbContext> options) : base(options)
        {
        }
    }
}