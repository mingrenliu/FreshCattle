using NPOI.SS.Util;

namespace ExcelUtile.ExcelCore;

public class ExcelWriter
{
    public const int WidthFactor = 256;
    protected ISheet _sheet;
    protected IRow? _currentRow;
    protected int _currentRowIndex = 0;
    protected int _currentColumnIndex = 0;
    protected readonly IConverterFactory _factory = new DefaultConverterFactory();

    private ICellStyle? cellStyle;
    public ICellStyle DefaultCellStyle => cellStyle ??= _sheet.Workbook.CreateDefaultCellStyle();

    public ExcelWriter(ISheet sheet)
    {
        _sheet = sheet;
    }

    public virtual void CheckOut(string name)
    {
        _sheet = _sheet.CreateNewSheet(name);
        _currentColumnIndex = 0;
        _currentRowIndex = 0;
    }

    public void WriteMergedRegion(IEnumerable<MergedRegion>? regions)
    {
        if (regions == null)
        {
            return;
        }
        foreach (var region in regions)
        {
            _ = WriteMergedRegion(region);
        }
    }

    public ICell WriteMergedRegion(MergedRegion region)
    {
        if (region.FirstRow != region.LastRow || region.FirstColumn != region.LastColumn)
        {
            RegionUtil.SetBorderBottom(BorderStyle.Thin, region, _sheet);
            RegionUtil.SetBorderRight(BorderStyle.Thin, region, _sheet);
            RegionUtil.SetBorderLeft(BorderStyle.Thin, region, _sheet);
            RegionUtil.SetBorderTop(BorderStyle.Thin, region, _sheet);
            _sheet.AddMergedRegion(region);
        }
        var cell = Cell(region.FirstRow, region.FirstColumn);
        if (region.Value != null)
        {
            var converter = _factory.GetDefaultConverter(region.Value.GetType());
            converter.WriteCell(cell, region.Value, region.FormatCellStyle?.Invoke(cell));
        }
        return cell;
    }

    public void SetPosition(int rowIndex, int columnIndex = 0)
    {
        _currentRowIndex = rowIndex;
        _currentColumnIndex = columnIndex;
    }

    public void NextColumn(int step = 1)
    {
        _currentColumnIndex += step;
    }

    public void NextRow(int step = 1)
    {
        _currentRowIndex += step;
        _currentColumnIndex = 0;
    }

    protected virtual int DefaultRowHeight { get; } = 20;

    public IRow Row(int rowIndex, int? height = null)
    {
        return _sheet.GetOrCreateRow(rowIndex, height ?? DefaultRowHeight);
    }

    public ICell Cell(int rowIndex, int columnIndex)
    {
        var row = Row(rowIndex);
        return row.GetOrCreateCell(columnIndex, DefaultCellStyle);
    }

    public IRow CurrentRow()
    {
        _currentRow ??= Row(_currentRowIndex);
        return _currentRow;
    }

    public ICell CurrentCell()
    {
        var row = CurrentRow();
        return row.GetOrCreateCell(_currentColumnIndex, DefaultCellStyle);
    }

    public ICell WriteCell<Value>(Value data, bool moveNext = true, ICellStyle? style = null) where Value : notnull
    {
        var cell = CurrentCell();
        _factory.GetDefaultConverter(typeof(Value)).WriteToCell(cell, data, style);
        if (moveNext)
        {
            NextColumn();
        }
        return cell;
    }

    public void SetColumnWidth(int? columnIndex = null, int? width = null)
    {
        columnIndex ??= _currentColumnIndex;
        if (width.HasValue)
        {
            _sheet.SetColumnWidth(columnIndex.Value, width.Value);
        }
        else
        {
            _sheet.AutoSizeColumn(columnIndex.Value);
        }
    }
}

public class ExcelWriter<T> : ExcelWriter where T : class
{
    private readonly IEnumerable<T> _data;
    private readonly ExcelExportOption<T> _option;
    private readonly IEnumerable<IExportCellHandler<T>> _handlers;

    public ExcelWriter(ISheet sheet, IEnumerable<T> data, ExcelExportOption<T>? option = null) : base(sheet)
    {
        _data = data;
        _option = option ?? new ExcelExportOption<T>();
        _handlers = CreateHandler(_option);
    }

    public ExcelWriter(ISheet sheet) : this(sheet, Enumerable.Empty<T>())
    {
    }

    private static IEnumerable<IExportCellHandler<T>> CreateHandler(ExcelExportOption<T> option)
    {
        IEnumerable<IExportCellHandler<T>> result = option.Selector.Invoke().Select(x => new DefaultCellHandler<T>(x));
        var dynamics = option.DynamicExports ?? Enumerable.Empty<ICellWriter<T>>();
        if (dynamics.Any())
        {
            result = result.Concat(dynamics.SelectMany(x => DynamicExportCellHandler<T>.Create(x)));
        }
        return result.OrderBy(x => x.Order).ToList();
    }

    protected override int DefaultRowHeight => _option.DefaultRowHeight;

    public ISheet Write()
    {
        WriteSheet(_data);
        WriteMergedRegion(_option.MergedRegions);
        return _sheet;
    }

    // 从开始行索引开始写入数据，写入完成后再写入表头，保证表头在最上面
    public void WriteSheet(IEnumerable<T> data)
    {
        SetPosition(_option.StartLineIndex);
        foreach (var item in data.Where(x => x != null))
        {
            WriteLine(item);
        }
        WriteHeader();
    }

    private void WriteHeader()
    {
        SetPosition(_option.HeaderLineIndex);
        foreach (var item in _handlers.Select(x => x.Info))
        {
            _factory.GetDefaultConverter(nameof(String)).WriteToCell(CurrentCell(), item.Name);
            SetColumnWidth(_currentColumnIndex, item.DynamicWidth ? null : ConvertWidth(item.Width));
            NextColumn();
        }
    }

    public int ConvertWidth(int? value)
    {
        return (value.HasValue && value > 0 ? value.Value : _option.DefaultColumnWidth) * WidthFactor;
    }

    public IRow WriteLine(T data, bool moveNext = true, Action<ICell>? action = null)
    {
        var currentRow = CurrentRow();
        foreach (var item in _handlers)
        {
            var cell = CurrentCell();
            item.WriteToCell(cell, data, _factory);
            action?.Invoke(cell);
            NextColumn();
        }
        if (moveNext)
        {
            NextRow();
        }
        return currentRow;
    }
}