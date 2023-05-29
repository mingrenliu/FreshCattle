using DatabaseHandler.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DatabaseHandler.DbContexts
{
    public abstract class BaseDbContext : DbContext
    {
        public ICurrentUser? CurrentUser { get; set; }

        public BaseDbContext(DbContextOptions option) : base(option)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var type in modelBuilder.Model.GetEntityTypes())
            {
                type.SetTableName(type.GetTableName());
                foreach (var prop in type.GetProperties())
                {
                    prop.SetColumnName(prop.GetColumnName().ToUpper());
                    prop.ValueGenerated = ValueGenerated.OnAdd;
                }
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}