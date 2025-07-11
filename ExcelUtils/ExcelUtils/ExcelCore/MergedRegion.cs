using NPOI.SS.Util;

namespace ExcelUtile.ExcelCore;

public class MergedRegion : CellRangeAddress
{
    public MergedRegion(int firstRow, int lastRow, int firstCol, int lastCol) : base(firstRow, lastRow, firstCol, lastCol)
    {
    }

    /// <summary>
    /// 单元格内容，可选
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// 设置单元格格式
    /// </summary>
    public Func<ICell, ICellStyle>? FormatCellStyle { get; set; }
}