using NPOI.SS.UserModel;
using ExcelUtile.ExcelCore;

namespace ExcelUtile.Converters;

/// <summary>
/// Excel 转换器非泛型基类，用于 ConverterRegistry 存储。
/// 对应 System.Text.Json 的 JsonConverter。
/// </summary>
public abstract class ExcelConverter
{
    public abstract Type Type { get; }
    public virtual string? ExcelFormat { get; set; }

    public virtual ICellStyle? CreateDefaultStyle(IWorkbook workbook)
    {
        if (string.IsNullOrWhiteSpace(ExcelFormat)) return null;
        var style = workbook.CreateDefaultCellStyle();
        style.DataFormat = workbook.GetDataFormat(ExcelFormat!);
        return style;
    }

    /// <summary>
    /// 非泛型读取（子类重写）。
    /// </summary>
    public abstract object? ReadObject(ICell cell);

    /// <summary>
    /// 非泛型写入（子类重写）。
    /// </summary>
    public abstract void WriteObject(ICell cell, object? value, ICellStyle? style = null);
}

/// <summary>
/// 泛型 Excel 转换器，定义 Read / Write 核心方法。
/// 对应 System.Text.Json 的 JsonConverter&lt;T&gt;。
/// </summary>
public abstract class ExcelConverter<T> : ExcelConverter
{
    /// <inheritdoc />
    public override Type Type => typeof(T);

    /// <summary>
    /// 从单元格读取值。如果单元格为空/无效，返回 default(T)。
    /// </summary>
    public abstract T Read(ICell cell);

    /// <summary>
    /// 将值写入单元格。
    /// </summary>
    public abstract void Write(ICell cell, T value, ICellStyle? style = null);

    /// <inheritdoc />
    public override object? ReadObject(ICell cell)
    {
        if (cell.IsInValid()) return null;
        return Read(cell);
    }

    /// <inheritdoc />
    public override void WriteObject(ICell cell, object? value, ICellStyle? style = null)
    {
        if (value is T t)
            Write(cell, t, style);
    }
}
