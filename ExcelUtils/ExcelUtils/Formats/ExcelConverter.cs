namespace ExcelUtile.Formats;

public abstract class ExcelConverter
{
    protected ICellStyle? _cellStyle;
    protected virtual string? Format { get; }

    protected readonly Type _type;

    public ExcelConverter(Type type)
    {
        _type = type;
    }

    public virtual bool CanConvert(Type type)
    {
        return _type == type;
    }

    /// <summary>
    /// 读取
    /// </summary>
    /// <param name="cell"> </param>
    /// <returns> </returns>
    public abstract object? ReadFromCell(ICell cell);

    /// <summary>
    /// 写入
    /// </summary>
    /// <param name="cell"> </param>
    /// <param name="obj"> </param>
    public void WriteToCell(ICell cell, object? obj)
    {
        WriteAsObject(cell, obj);
        cell.CellStyle = _cellStyle ?? CreateCellType(cell);
    }

    /// <summary>
    /// 默认写入方法
    /// </summary>
    /// <param name="cell"> </param>
    /// <param name="obj"> </param>
    public virtual void WriteAsObject(ICell cell, object? obj)
    {
        cell.SetCellValue(obj?.ToString());
    }

    public virtual ICellStyle? CreateCellType(ICell cell)
    {
        var style = cell.Sheet.Workbook.CreateCellStyle();
        style.Alignment = HorizontalAlignment.Center;
        style.VerticalAlignment = VerticalAlignment.Center;
        style.BorderBottom = BorderStyle.Thin;
        style.BorderLeft = BorderStyle.Thin;
        style.BorderRight = BorderStyle.Thin;
        style.BorderTop = BorderStyle.Thin;
        style.WrapText = true;
        if (!string.IsNullOrWhiteSpace(Format))
        {
            var format = cell.Sheet.Workbook.CreateDataFormat();
            var formatIndex = format.GetFormat(Format);
            style.DataFormat = formatIndex;
        }
        _cellStyle = style;
        return style;
    }
}