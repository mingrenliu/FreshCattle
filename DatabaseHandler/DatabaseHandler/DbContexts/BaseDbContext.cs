using DatabaseHandler.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Security.Cryptography.X509Certificates;

namespace DatabaseHandler.DbContexts
{
    public class BaseDbContext : DbContext
    {
        public CurrentUser?  CurrentUser { get; set; }

        public BaseDbContext(DbContextOptions option):base(option)
        {
            SavingChanges += DbContextEvents.UpdateTimestamps;
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
    public class DbContextEvents
    {
        public static void UpdateTimestamps(object? sender, SavingChangesEventArgs e)
        {
            
        }
    }

}