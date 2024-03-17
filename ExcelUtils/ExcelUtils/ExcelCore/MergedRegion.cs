namespace ExcelUtile.ExcelCore;

public class MergedRegion
{
    /// <summary>
    /// 起始行
    /// </summary>
    public int RowStartIndex { get; set; }

    /// <summary>
    /// 结束行
    /// </summary>
    public int RowEndIndex { get; set; }

    /// <summary>
    /// 起始列
    /// </summary>
    public int ColumnStartIndex { get; set; }

    /// <summary>
    /// 结束列
    /// </summary>
    public int ColumnEndIndex { get; set; }

    /// <summary>
    /// 单元格内容，可选
    /// </summary>
    public object? Value { get; set; }
}