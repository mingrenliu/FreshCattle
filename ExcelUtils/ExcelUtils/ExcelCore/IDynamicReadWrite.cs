namespace ExcelUtile.ExcelCore;

/// <summary>
/// 导入导出表头匹配
/// </summary>
/// <typeparam name="T"> </typeparam>
public interface IDynamicHeader<T>
{
    bool Match(string field);
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
public interface IImportDynamicRead<T> : IDynamicHeader<T>
{
    void ReadFromCell(T obj, ICell cell, string field, IConverterFactory factory);
}

/// <summary>
/// 导入时根据字段信息准确读取
/// </summary>
/// <typeparam name="T"> </typeparam>
public interface IImportExactRead<T> : IExactHeader<T>, IImportDynamicRead<T>
{
}

/// <summary>
/// 导出写入
/// </summary>
/// <typeparam name="T"> </typeparam>
public interface IExportDynamicWrite<T> : IExactHeader<T>
{
    void WriteToCell(T obj, ICell cell, string field, IConverterFactory factory, ICellStyle? style = null);
}