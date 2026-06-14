using NPOI.SS.UserModel;

namespace ExcelUtile.ExcelCore;

/// <summary>
/// 低层 Excel 读取器，提供自由读取单元格、解析表头等能力。
/// 数组对象的导入（ExcelDataReader）建立在此之上。
/// </summary>
public class ExcelSheetReader
{
    private readonly ISheet _sheet;

    // ==================== 构造 ====================

    public ExcelSheetReader(ISheet sheet)
    {
        _sheet = sheet;
    }

    // ==================== Sheet ====================

    /// <summary>底层 NPOI ISheet，可直接操作。</summary>
    public ISheet Sheet => _sheet;

    /// <summary>Sheet 总行数。</summary>
    public int LastRowNum => _sheet.LastRowNum;

    // ==================== 行 / 单元格访问 ====================

    /// <summary>获取指定行，不存在返回 null。</summary>
    public IRow? GetRow(int rowIndex)
    {
        return _sheet.GetRow(rowIndex);
    }

    /// <summary>获取指定单元格，不存在返回 null。</summary>
    public ICell? GetCell(int rowIndex, int colIndex)
    {
        var row = _sheet.GetRow(rowIndex);
        return row?.GetCell(colIndex);
    }

    /// <summary>解析合并单元格的值（如果 row,col 在合并区域内，返回左上角单元格）。</summary>
    public ICell? GetMergedCell(int rowIndex, int colIndex)
    {
        var cell = GetCell(rowIndex, colIndex);
        if (cell != null) return cell;

        var merged = _sheet.MergedRegions.FirstOrDefault(
            r => r.ContainsRow(rowIndex) && r.FirstColumn <= colIndex && r.LastColumn >= colIndex);
        if (merged != null)
        {
            var firstRow = _sheet.GetRow(merged.FirstRow);
            return firstRow?.GetCell(merged.FirstColumn);
        }
        return null;
    }

    /// <summary>获取单元格的字符串值。</summary>
    public string? GetString(int rowIndex, int colIndex)
    {
        return GetMergedCell(rowIndex, colIndex)?.GetString();
    }

    /// <summary>遍历所有行。</summary>
    public IEnumerable<IRow> EnumerateRows(int startRow = 0)
    {
        for (int i = startRow; i <= _sheet.LastRowNum; i++)
        {
            var row = _sheet.GetRow(i);
            if (row != null)
                yield return row;
        }
    }

    // ==================== 表头解析 ====================

    /// <summary>
    /// 解析指定行作为表头，返回 列索引 → 列名 的字典。
    /// 支持合并单元格。
    /// </summary>
    public Dictionary<int, string> ReadHeaders(int headerRowIndex)
    {
        var headers = new Dictionary<int, string>();
        var headerRow = _sheet.GetRow(headerRowIndex);
        if (headerRow == null) return headers;

        foreach (var cell in headerRow.Cells)
        {
            var text = cell.StringCellValue?.Trim();
            if (!string.IsNullOrWhiteSpace(text))
                headers[cell.ColumnIndex] = text;
        }

        // 合并单元格的标题
        foreach (var region in _sheet.MergedRegions.Where(r => r.ContainsRow(headerRowIndex)))
        {
            var mergedRow = _sheet.GetRow(region.FirstRow);
            var mergedCell = mergedRow?.GetCell(region.FirstColumn);
            var text = mergedCell?.StringCellValue?.Trim();
            if (!string.IsNullOrWhiteSpace(text) && !headers.ContainsKey(region.FirstColumn))
                headers[region.FirstColumn] = text;
        }

        return headers;
    }

    /// <summary>
    /// 读取一行数据，返回 列索引 → 文本值 的字典。
    /// 合并单元格值会被解析到所有覆盖列。
    /// </summary>
    public Dictionary<int, string?> ReadRowAsText(int rowIndex, IEnumerable<int> columnIndexes)
    {
        var result = new Dictionary<int, string?>();
        var row = _sheet.GetRow(rowIndex);
        foreach (var col in columnIndexes)
        {
            var cell = GetMergedCell(rowIndex, col);
            result[col] = cell?.GetString();
        }
        return result;
    }
}
