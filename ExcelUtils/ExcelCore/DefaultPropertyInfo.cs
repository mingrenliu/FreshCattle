﻿using System.Reflection;

namespace ExcelUtile.ExcelCore;

internal class DefaultPropertyInfo : PropertyTypeInfo
{
    private readonly DisplayAttribute _display;

    public DefaultPropertyInfo(PropertyInfo info, DisplayAttribute attribute) : base(info, attribute.Name)
    {
        _display = attribute;
    }
    public override int? Width => _display.Width;
    public override bool IsRequired => _display.IsRequired;

    public override int Order => _display.Order;
}