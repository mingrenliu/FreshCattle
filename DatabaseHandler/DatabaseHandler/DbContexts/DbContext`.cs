using DatabaseHandler.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DatabaseHandler.DbContexts;

public class DbContext<TContext> : DbContext where TContext : DbContext
{
    public ICurrentUser? CurrentUser { get; set; }
    public DbContext(DbContextOptions<TContext> options) : base(options)
    {
    }
    protected override sealed void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TContext).Assembly);
        var nameConversion = NameConversionFactory.Create(GetDbType(Database));
        foreach (var type in modelBuilder.Model.GetEntityTypes())
        {
            type.SetTableName(nameConversion.TableName(type.GetTableName()));
            foreach (var prop in type.GetProperties())
            {
                prop.SetColumnName(nameConversion.ColumnName(prop.GetColumnName()));
            }
            foreach (var item in type.GetKeys())
            {
                foreach (var prop in item.Properties)
                {
                    prop.ValueGenerated = ValueGenerated.OnAdd;
                }
            }
        }
        AfterOnModelCreating(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
    internal static DbType GetDbType(DatabaseFacade database)
    {
        return database.IsMySql() ? DbType.MySql : database.IsOracle() ? DbType.Oracle : DbType.Undefined;
    }

    public virtual void AfterOnModelCreating(ModelBuilder modelBuilder)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {

        base.ConfigureConventions(configurationBuilder);
    }
}