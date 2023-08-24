using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace DatabaseHandler.ValueGenerators
{
    internal class CustomValueGeneratorSelector : ValueGeneratorSelector
    {
        public CustomValueGeneratorSelector(ValueGeneratorSelectorDependencies dependencies) : base(dependencies)
        {
        }

        protected override ValueGenerator? FindForType(IProperty property, IEntityType entityType, Type clrType)
        {
            if (clrType == typeof(string))
            {
                return new SnowflakeGenerator();
            }
            return base.FindForType(property, entityType, clrType);
        }
    }
}