namespace ExcelUtils;

[AttributeUsage(AttributeTargets.Property)]
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


    #region Width

    private const int unitWidth = 256;

    /// <summary>
    /// 单元格宽度(默认0)
    /// </summary>
    public int _width;

    /// <summary>
    /// 获取单元格单位
    /// </summary>
    /// <returns></returns>
    public int Width => _width * unitWidth;

    #endregion Width

    #region Height

    //private const short unitHeight = 20;
    //private const float defaultHeight = 14.25F;
    /// <summary>
    /// 单元格高度(默认14.25)
    /// </summary>
    //public float _height= defaultHeight;

    /// <summary>
    /// 获取单元格高度（excel默认short类型）
    /// </summary>
    /// <returns></returns>
    //public short Height => (short)(_height * unitHeight);

    #endregion Height

    /// <summary>
    /// 宽度自适应
    /// </summary>
    private readonly bool _autoAdapt;
    public bool AutoAdapt => _autoAdapt;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="name"></param>
    public DisplayAttribute(string name, bool autoAdapt = true, int width = 0)
    {
        _name = name;
        _autoAdapt = autoAdapt;
        _width = width;
    }
}