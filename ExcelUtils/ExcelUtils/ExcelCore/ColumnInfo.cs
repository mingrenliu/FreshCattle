using ExcelUtile.Formats;
using System.Reflection;

namespace ExcelUtile.ExcelCore;

/// <summary>
/// 列信息
/// </summary>
public class ColumnInfo
{
    /// <summary>
    /// 字段展示名称
    /// </summary>
    public virtual string Name { get; }

    private int? _width;

    /// <summary>
    /// 列宽
    /// </summary>
    public virtual int? Width { get => _width <= 0 ? null : _width; private set => _width = value; }

    /// <summary>
    /// 导出字段顺序
    /// </summary>
    public virtual int Order { get; }

    /// <summary>
    /// 基础类型
    /// </summary>
    public Type BaseType { get; private set; }

    /// <summary>
    /// 动态列宽
    /// </summary>
    public bool DynamicWidth { get; set; } = false;

    /// <summary>
    /// 字段是必须的(导入时没有该字段,会报错)
    /// </summary>
    public virtual bool IsRequired { get; }

    /// <summary>
    /// 设置单元格格式
    /// </summary>
    public Func<ICell, ICellStyle>? FormatCellStyle { get; set; }

    /// <summary>
    /// 自定义类型转换器
    /// </summary>

    public ExcelConverter? Converter { get; set; }

    /// <summary>
    /// </summary>
    /// <param name="name"> 字段导出名称 </param>
    /// <param name="type"> 字段类型 </param>
    /// <param name="order"> 序号 </param>
    /// <param name="isRequired"> 是否必须 </param>
    /// <param name="width"> 列宽 </param>
    public ColumnInfo(string name, Type type, int order = 0, bool isRequired = true, int? width = 0)
    {
        BaseType = Nullable.GetUnderlyingType(type) ?? type;
        Name = name;
        IsRequired = isRequired;
        Order = order;
        Width = width;
    }

    /// <summary>
    /// 获取转换器
    /// </summary>
    /// <param name="_factory"> </param>
    /// <returns> </returns>
    public ExcelConverter? GetConverter(IConverterFactory _factory)
    {
        return Converter ?? _factory.GetDefaultConverter(BaseType);
    }
}

public class PropertyTypeInfo : ColumnInfo
{
    public PropertyInfo Info { get; private set; }

    public PropertyTypeInfo(PropertyInfo info, string name, int order = 0, bool isRequired = true, int? width = 0) : base(name, info.PropertyType, order, isRequired, width)
    {
        Info = info;
        Converter = Info.GetCustomAttribute<DataFormatAttribute>()?.Converter;
    }
}

public class DefaultPropertyInfo : PropertyTypeInfo
{
    public DefaultPropertyInfo(PropertyInfo info, DisplayAttribute attribute, string? name = null) : base(info, name ?? attribute.Name, attribute.Order, attribute.IsRequired, attribute.Width)
    {
        DynamicWidth = attribute.DynamicWidth;
    }
}