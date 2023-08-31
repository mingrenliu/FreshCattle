using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace DatabaseHandler.Conventions
{
    public class EntityFilterByContextConvention : EntityTypeAttributeConventionBase<DbContextAttribute>
    {
        public EntityFilterByContextConvention(ProviderConventionSetBuilderDependencies dependencies) : base(dependencies)
        {
        }

        protected override void ProcessEntityTypeAdded(IConventionEntityTypeBuilder entityTypeBuilder, DbContextAttribute attribute, IConventionContext<IConventionEntityTypeBuilder> context)
        {
            if (attribute.ContextType != Dependencies.ContextType && entityTypeBuilder.ModelBuilder.Ignore(entityTypeBuilder.Metadata.Name, fromDataAnnotation: true) != null)
            {
                context.StopProcessing();
            }
        }
    }
}