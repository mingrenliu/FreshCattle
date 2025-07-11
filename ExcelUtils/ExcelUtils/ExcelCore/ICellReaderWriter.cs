namespace ExcelUtile.ExcelCore;

/// <summary>
/// 表头匹配
/// </summary>
/// <typeparam name="T"> </typeparam>
public interface IDynamicHeader<T>
{
    bool Match(string title);
}

/// <summary>
/// 表头信息
/// </summary>
/// <typeparam name="T"> </typeparam>
public interface IExactHeader<T> : IDynamicHeader<T>
{
    IEnumerable<ColumnInfo> Headers();
}

/// <summary>
/// 导入读取
/// </summary>
/// <typeparam name="T"> </typeparam>
public interface IDynamicCellReader<T> : ICellReader<T>
{
}

/// <summary>
/// 导入时根据字段信息准确读取
/// </summary>
/// <typeparam name="T"> </typeparam>
public interface IExactCellReader<T> : IExactHeader<T>, ICellReader<T>
{
}

/// <summary>
/// 导入(导入必须匹配Title Header)
/// </summary>
/// <typeparam name="T"> </typeparam>
public interface ICellReader<T> : IDynamicHeader<T>
{
    void ReadFromCell(T obj, ICell cell, string title, IConverterFactory factory);
}

/// <summary>
/// 导出写入(导出必须要有Title Header)
/// </summary>
/// <typeparam name="T"> </typeparam>
public interface ICellWriter<T> : IExactHeader<T>
{
    void WriteToCell(T obj, ICell cell, string title, IConverterFactory factory, ICellStyle? style = null);
}