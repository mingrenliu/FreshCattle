namespace ExcelTool.ExcelCore;

/// <summary>
/// 合并区域信息。
/// </summary>
public class MergeRegion
{
    public int FirstRow { get; }
    public int LastRow { get; }
    public int FirstCol { get; }
    public int LastCol { get; }
    public object? Value { get; set; }
    public Action<ICell>? PostAction { get; set; }

    public MergeRegion(int firstRow, int lastRow, int firstCol, int lastCol)
    {
        FirstRow = firstRow;
        LastRow = lastRow;
        FirstCol = firstCol;
        LastCol = lastCol;
    }
}
