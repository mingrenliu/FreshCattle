namespace ExcelUtile.ExcelCore;

/// <summary>
/// 列基础信息
/// </summary>
internal interface IColumnBaseInfo
{
    ColumnInfo Info { get; }
}

/// <summary>
/// 导出单元格处理器,导出必须知道字段的顺序和字段名称
/// </summary>
/// <typeparam name="T"> </typeparam>
internal interface IExportCellHandler<T> : IColumnBaseInfo where T : class
{
    public int Order { get; }

    void WriteToCell(ICell cell, T value, IConverterFactory factory, ICellStyle? style = null);
}

/// <summary>
/// 导入单元格处理器,字段的顺序和字段名称飞必须，只需根据匹配条件进行判断
/// </summary>
/// <typeparam name="T"> </typeparam>
internal interface IImportCellHandler<T> : IDynamicHeader<T> where T : class
{
    void ReadFromCell(ICell cell, T value, string title, IConverterFactory factory);
}

/// <summary>
/// 根据字段名称匹配的导入单元格处理器
/// </summary>
/// <typeparam name="T"> </typeparam>
internal interface IExactImportCellHandler<T> : IImportCellHandler<T>, IColumnBaseInfo where T : class
{
}