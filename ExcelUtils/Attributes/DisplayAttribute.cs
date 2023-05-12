namespace ExcelUtils;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class DisplayAttribute : Attribute
{
    private readonly string _name;

    /// <summary>
    /// 字段展示名称
    /// </summary>
    public string Name { get => _name; }

    /// <summary>
    /// 导出字段顺序
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// 字段是必须的(导入时没有该字段,会报错)
    /// </summary>
    public bool IsRequird { get; set; } = true;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="name"></param>
    public DisplayAttribute(string name, int order = 0, bool isRequird = true)
    {
        _name = name;
        IsRequird = isRequird;
        Order = order;
    }
}