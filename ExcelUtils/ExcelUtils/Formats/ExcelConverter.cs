using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public abstract class ExcelConverter
{
    /// <summary>
    /// 单元格样式
    /// </summary>
    protected ICellStyle? _cellStyle;

    /// <summary>
    /// 单元格格式
    /// </summary>
    protected virtual string? Format { get; }

    /// <summary>
    /// 数据类型
    /// </summary>

    protected readonly Type _type;

    public ExcelConverter(Type type)
    {
        _type = type;
    }

    /// <summary>
    /// 是否可以转换
    /// </summary>
    /// <param name="type"> </param>
    /// <returns> </returns>
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
    public void WriteToCell(ICell cell, object? obj, ICellStyle? style = null)
    {
        WriteAsObject(cell, obj);
        FormatCell(cell, style);
    }

    /// <summary>
    /// 默认写入方法
    /// </summary>
    /// <param name="cell"> </param>
    /// <param name="obj"> </param>
    protected virtual void WriteAsObject(ICell cell, object? obj)
    {
        if (obj != null)
        {
            cell.SetCellValue(obj.ToString());
        }
    }

    /// <summary>
    /// 规范单元格样式
    /// </summary>
    /// <param name="cell"> </param>
    protected void FormatCell(ICell cell, ICellStyle? style = null)
    {
        cell.CellStyle = style ?? _cellStyle ?? DefaultCellStyle(cell);
    }

    /// <summary>
    /// 默认样式
    /// </summary>
    /// <param name="cell"> </param>
    /// <returns> </returns>
    public virtual ICellStyle DefaultCellStyle(ICell cell)
    {
        if (_cellStyle == null)
        {
            _cellStyle ??= cell.Sheet.Workbook.CreateDefaultCellStyle();
            if (!string.IsNullOrWhiteSpace(Format))
            {
                _cellStyle.DataFormat = cell.Sheet.Workbook.GetDataFormat(Format);
            }
        }
        return _cellStyle;
    }
}