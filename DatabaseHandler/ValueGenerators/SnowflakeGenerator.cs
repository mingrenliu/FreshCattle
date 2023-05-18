using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace DatabaseHandler.ValueGenerators
{
    internal class SnowflakeGenerator : ValueGenerator
    {
        private readonly SnowflakeAlgorithm _snowflakeId = new();
        public override bool GeneratesTemporaryValues => false;

        protected override object? NextValue(EntityEntry entry)
        {
            return _snowflakeId.NextId().ToString();
        }
    }
}