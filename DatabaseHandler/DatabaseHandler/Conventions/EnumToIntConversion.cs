﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;

namespace DatabaseHandler.Conventions;

public class EnumToIntConversion : PropertyAttributeConventionBase<EnumToIntAttribute>
{
    public EnumToIntConversion(ProviderConventionSetBuilderDependencies dependencies) : base(dependencies)
    {
    }

    protected override void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, EnumToIntAttribute attribute, MemberInfo clrMember, IConventionContext context)
    {
        if ((propertyBuilder.Metadata.ClrType is null) || (propertyBuilder.Metadata.ClrType.IsEnum is false))
        {
            return;
        }
        var valueConverter = Activator.CreateInstance(typeof(EnumToNumberConverter<,>).MakeGenericType(propertyBuilder.Metadata.ClrType, typeof(int)));
        if (valueConverter is not null && valueConverter is ValueConverter converter)
        {
            propertyBuilder.HasConversion(converter);
        }
    }
}