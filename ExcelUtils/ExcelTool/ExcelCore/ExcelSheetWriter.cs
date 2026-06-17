namespace ExcelTool.ExcelCore;

/// <summary>
/// 低层 Excel 写入器，提供自由定位、自由写入单元格的能力。
/// 数组对象的导入导出（ExcelDataWriter）建立在此之上。
///
/// <para>使用示例：</para>
/// <code>
/// var writer = new ExcelSheetWriter(sheet);
/// writer.MoveTo(0, 0).Write("标题").NextRow();
/// writer.Write(123).SetPrevWidth(15);
/// writer.NextColumn().Write("备注");
/// writer.Merge(new MergedRegionInfo(0, 0, 0, 3) { Value = "日报表" });
/// </code>
/// </summary>
public class ExcelSheetWriter
{
    public const int WidthFactor = 256;

    private ISheet _sheet;
    private int _rowIndex;
    private int _colIndex;
    private IRow? _currentRow;

    protected ICellStyle? _defaultStyle;
    private readonly Dictionary<string, ICellStyle> _namedStyles = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>默认行高（磅），子类可重写。</summary>
    protected virtual float DefaultRowHeight => 20;

    // ==================== 构造 ====================

    public ExcelSheetWriter(ISheet sheet)
    {
        _sheet = sheet;
    }

    // ==================== Sheet 切换 ====================
    public ISheet Sheet => _sheet;

    public IWorkbook Workbook => _sheet.Workbook;
    public int RowIndex => _rowIndex;
    public int ColIndex => _colIndex;

    /// <summary>切换到指定名称的 Sheet（不存在则创建），重置游标到 (0,0)。</summary>
    public ExcelSheetWriter UseSheet(string name)
    {
        _sheet = _sheet.Workbook.CreateNewSheet(name);
        _rowIndex = 0;
        _colIndex = 0;
        _currentRow = null;
        return this;
    }

    // ==================== 样式 ====================

    public ICellStyle DefaultStyle
    {
        get
        {
            _defaultStyle ??= Workbook.CreateDefaultCellStyle();
            return _defaultStyle;
        }
    }

    /// <summary>创建新样式（继承默认样式）。</summary>
    public ICellStyle Style()
    {
        var style = Workbook.CreateCellStyle();
        style.CloneStyleFrom(DefaultStyle);
        return style;
    }

    /// <summary>获取或创建命名样式。已存在则返回缓存，否则新建。</summary>
    public ICellStyle Style(string name)
    {
        if (_namedStyles.TryGetValue(name, out var cached))
            return cached;
        return Style(name, null);
    }

    /// <summary>定义命名样式（首次调用时用 <paramref name="setup"/> 初始化，后续返回缓存）。</summary>
    public ICellStyle Style(string name, Action<ICellStyle>? setup)
    {
        if (_namedStyles.TryGetValue(name, out var cached))
            return cached;

        var style = Style();
        setup?.Invoke(style);
        _namedStyles[name] = style;
        return style;
    }

    /// <summary>判断命名样式是否存在。</summary>
    public bool HasStyle(string name) => _namedStyles.ContainsKey(name);

    // ==================== 游标 ====================

    /// <summary>移动游标到指定位置。</summary>
    public ExcelSheetWriter MoveTo(int row, int col = 0)
    {
        _rowIndex = row;
        _colIndex = col;
        _currentRow = null;
        return this;
    }

    public ExcelSheetWriter NextCol(int step = 1)
    {
        _colIndex += step;
        return this;
    }

    public ExcelSheetWriter NextRow(int step = 1)
    {
        _rowIndex += step;
        _colIndex = 0;
        _currentRow = null;
        return this;
    }

    // ==================== 行/单元格 ====================

    public IRow RowAt(int rowIndex, float? height = null)
    {
        return _sheet.GetOrCreateRow(rowIndex, height ?? DefaultRowHeight);
    }

    public ICell CellAt(int rowIndex, int colIndex, ICellStyle? style = null)
    {
        var row = RowAt(rowIndex);
        return row.GetOrCreateCell(colIndex, style ?? DefaultStyle);
    }

    public IRow CurrentRow()
    {
        if (_currentRow == null || _currentRow.RowNum != _rowIndex)
            _currentRow = _sheet.GetOrCreateRow(_rowIndex, DefaultRowHeight);
        return _currentRow;
    }

    public ICell CurrentCell(ICellStyle? style = null)
    {
        var row = CurrentRow();
        return row.GetOrCreateCell(_colIndex, style ?? DefaultStyle);
    }

    // ==================== 写入 ====================

    /// <summary>在当前位置写入值（自动选转换器），列号+1。</summary>
    /// <param name="converter">提前获取Converter,避免每次查找导致的性能开销</param>

    public ICell Write<T>(T? value, ICellStyle? style = null)
    {
        return Write(value, null, style);
    }

    /// <summary>在当前位置写入值（自动选转换器），列号+1。</summary>
    /// <param name="converter">提前获取Converter,避免每次查找导致的性能开销</param>
    public ICell Write<T>(T? value, ExcelConverter<T>? converter, ICellStyle? style = null)
    {
        var cell = CurrentCell(style);
        if (value != null)
        {
            var temp = converter ?? BuiltInConverters.GetConverter(typeof(T));
            if (temp != null)
                temp.WriteObject(cell, value, style);
            else
                cell.SetCellValue(value.ToString());
        }
        _colIndex++;
        return cell;
    }

    /// <summary>在当前位置写入值（指定转换器），列号+1。</summary>
    public ICell WriteObject(object? value, ICellStyle? style = null)
    {
        return WriteObject(value, null, style);
    }

    /// <summary>在当前位置写入值（指定转换器），列号+1。</summary>
    /// <param name="converter">提前获取Converter,避免每次查找导致的性能开销</param>
    public ICell WriteObject(object? value, ExcelConverter? converter, ICellStyle? style = null)
    {
        // style在这里已经设置了
        var cell = CurrentCell(style);
        if (value != null)
        {
            converter ??= BuiltInConverters.GetConverter(value.GetType());
            if (converter != null)
                converter.WriteObject(cell, value, style);
            else
                cell.SetCellValue(value.ToString());
        }
        _colIndex++;
        return cell;
    }

    /// <summary>在绝对位置写入值（不移动游标）。</summary>
    /// <param name="converter">提前获取Converter,避免每次查找导致的性能开销</param>
    public ICell WriteAt<T>(int row, int col, T? value, ExcelConverter<T>? converter = null, ICellStyle? style = null)
    {
        var cell = CellAt(row, col, style);
        if (value != null)
        {
            var temp = converter ?? BuiltInConverters.GetConverter(typeof(T));
            if (temp != null)
                temp.WriteObject(cell, value, style);
            else
                cell.SetCellValue(value.ToString());
        }
        return cell;
    }

    /// <summary>在绝对位置写入值（不移动游标）。</summary>
    /// <param name="converter">提前获取Converter,避免每次查找导致的性能开销</param>
    public ICell WriteObjectAt(int row, int col, object? value, ExcelConverter? converter = null, ICellStyle? style = null)
    {
        var cell = CellAt(row, col, style);
        if (value != null)
        {
            converter ??= BuiltInConverters.GetConverter(value.GetType());
            if (converter != null)
                converter.WriteObject(cell, value, style);
            else
                cell.SetCellValue(value.ToString());
        }
        return cell;
    }

    // ==================== 列宽 ====================

    /// <summary>设置刚写入列的列宽（字符数），配合 Write 链式使用，操作的是上一列而非当前列。</summary>
    public ExcelSheetWriter PrevWidth(double chars)
    {
        var targetCol = Math.Max(0, _colIndex - 1);
        if (chars < 0)
            _sheet.AutoSizeColumn(targetCol);
        else if (chars > 0)
            _sheet.SetColumnWidth(targetCol, chars * WidthFactor);
        return this;
    }

    /// <summary>设置指定列宽（字符数）。</summary>
    public ExcelSheetWriter WidthAt(int col, double chars)
    {
        if (chars < 0)
            _sheet.AutoSizeColumn(col);
        else if (chars > 0)
            _sheet.SetColumnWidth(col, chars * WidthFactor);
        return this;
    }

    // ==================== 合并区域 ====================

    /// <summary>写入合并区域。</summary>
    public ICell Merge(MergeRegion region)
    {
        if (region.FirstRow != region.LastRow || region.FirstCol != region.LastCol)
        {
            var address = new CellRangeAddress(region.FirstRow, region.LastRow, region.FirstCol, region.LastCol);
            RegionUtil.SetBorderBottom(BorderStyle.Thin, address, _sheet);
            RegionUtil.SetBorderRight(BorderStyle.Thin, address, _sheet);
            RegionUtil.SetBorderLeft(BorderStyle.Thin, address, _sheet);
            RegionUtil.SetBorderTop(BorderStyle.Thin, address, _sheet);
            _sheet.AddMergedRegion(address);

            // 清除非左上角单元格内容，确保合并后其余位置为空
            for (var r = region.FirstRow; r <= region.LastRow; r++)
            {
                var row = _sheet.GetRow(r);
                if (row == null) continue;
                for (var c = region.FirstCol; c <= region.LastCol; c++)
                {
                    if (r == region.FirstRow && c == region.FirstCol) continue;
                    row.GetCell(c)?.SetBlank();
                }
            }
        }

        var cell = CellAt(region.FirstRow, region.FirstCol);
        if (region.Value != null)
        {
            cell.SetCellValue(region.Value.ToString());
            region.PostAction?.Invoke(cell);
        }
        return cell;
    }

    /// <summary>批量写入合并区域。</summary>
    public void Merge(IEnumerable<MergeRegion>? regions)
    {
        if (regions == null) return;
        foreach (var region in regions)
            Merge(region);
    }
}