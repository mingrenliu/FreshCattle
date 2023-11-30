using DatabaseHandler.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseHandler.Services
{
    public class MigrationFactory
    {
        public static void Generate<T>(string name, string rootNamespace) where T : DbContext<T>
        {
            // Create design-time services
            var db = new ApplicationDbContextFactory<T>().CreateDbContext(null);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddEntityFrameworkDesignTimeServices();
            serviceCollection.AddDbContextDesignTimeServices(db);
            serviceCollection.AddEntityFrameworkMySql();
            serviceCollection.AddSingleton<IMigrationsCodeGenerator, CustomCSharpMigrationsGenerator>();
            serviceCollection.AddSingleton<IMigrationsScaffolder, CustomMigrationsScaffolder>();
            new EntityFrameworkRelationalDesignServicesBuilder(serviceCollection).TryAddCoreServices();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Add a migration
            var migrationsScaffolder = serviceProvider.GetService<IMigrationsScaffolder>();
            var migration = migrationsScaffolder.ScaffoldMigration(name, rootNamespace, "Migrations.MySql");
            migrationsScaffolder.Save("./", migration, "./Migrations/MySql");
        }

        public class ApplicationDbContextFactory<T> : IDesignTimeDbContextFactory<T> where T : DbContext<T>
        {
            public T CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<T>();
                optionsBuilder.UseMySql("Data Source=192.168.0.205;Initial Catalog=mes;User ID=mes;Password=test", MySqlServerVersion.LatestSupportedServerVersion);
                var result = Activator.CreateInstance(typeof(T), optionsBuilder.Options);
                return (T)result!;
            }
        }
    }
}