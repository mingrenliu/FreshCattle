using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System.ComponentModel;
using System.Reflection;

namespace DatabaseHandler.Conventions
{
    public class DefaultValueConversion : PropertyAttributeConventionBase<DefaultValueAttribute>
    {
        public DefaultValueConversion(ProviderConventionSetBuilderDependencies dependencies) : base(dependencies)
        {
        }

        protected override void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, DefaultValueAttribute attribute, MemberInfo clrMember, IConventionContext context)
        {
            propertyBuilder.HasDefaultValue(attribute.Value, true);
        }
    }
}