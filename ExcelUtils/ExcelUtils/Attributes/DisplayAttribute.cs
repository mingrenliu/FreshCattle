namespace ExcelUtile;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class DisplayAttribute : Attribute
{
    /// <summary>
    /// 字段展示名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 列宽
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// 导出字段顺序
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// 字段是必须的(导入时没有该字段,会报错)
    /// </summary>
    public bool IsRequired { get; } = true;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="name"></param>
    public DisplayAttribute(string name, int order = 0, bool isRequired = true, int width = 0)
    {
        Name = name;
        IsRequired = isRequired;
        Order = order;
        Width = width;
    }
}