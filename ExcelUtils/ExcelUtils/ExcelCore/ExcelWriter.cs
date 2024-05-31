using NPOI.SS.Util;

namespace ExcelUtile.ExcelCore;

internal class ExcelWriter<T> where T : class
{
    private readonly ICellStyle DefaultCellStyle;
    private const int WidthFactor = 256;
    private const int HeightFactor = 20;
    private readonly IEnumerable<KeyValuePair<string, Tuple<IEnumerable<T>, ExcelExportSheetOption<T>>>> _data;
    private readonly IWorkbook workbook;
    private ISheet? _currentSheet;
    private IRow? _currentRow;
    private int _rowIndex = 0;
    private int _columnIndex = 0;
    private readonly ExcelExportOption<T> _option;
    private readonly IEnumerable<IExportCellHandler<T>> _workbookHandlers;
    private IEnumerable<IExportCellHandler<T>>? _handlers;
    private IEnumerable<IExportCellHandler<T>> Info => _handlers ?? Enumerable.Empty<IExportCellHandler<T>>();
    private readonly IConverterFactory _factory = new DefaultConverterFactory();

    public ExcelWriter(ExcelExportSheetOption<T> sheetOption, IEnumerable<T> data, ExcelExportOption<T>? option = null)
    {
        _data = new List<KeyValuePair<string, Tuple<IEnumerable<T>, ExcelExportSheetOption<T>>>>() { new("sheet", new(data, sheetOption)) };
        workbook = ExcelFactory.CreateWorkBook();
        _option = option ?? new ExcelExportOption<T>();
        _workbookHandlers = CreateHandler(_option);
        DefaultCellStyle = CreateDefaultCellStyle(workbook);
    }

    public ExcelWriter(IEnumerable<T> data, ExcelExportOption<T>? option = null, IEnumerable<MergedRegion>? region = null)
    {
        _data = new List<KeyValuePair<string, Tuple<IEnumerable<T>, ExcelExportSheetOption<T>>>>() { new("sheet", new(data, new ExcelExportSheetOption<T>() { MergedRegions = region })) };
        workbook = ExcelFactory.CreateWorkBook();
        _option = option ?? new ExcelExportOption<T>();
        _workbookHandlers = CreateHandler(_option);
        DefaultCellStyle = CreateDefaultCellStyle(workbook);
    }

    public ExcelWriter(Dictionary<string, Tuple<IEnumerable<T>, IEnumerable<MergedRegion>?>> data, ExcelExportOption<T>? option = null)
    {
        _data = data.Select(x => new KeyValuePair<string, Tuple<IEnumerable<T>, ExcelExportSheetOption<T>>>(x.Key, new Tuple<IEnumerable<T>, ExcelExportSheetOption<T>>(x.Value.Item1, new ExcelExportSheetOption<T>() { MergedRegions = x.Value.Item2 })));
        workbook = ExcelFactory.CreateWorkBook();
        _option = option ?? new ExcelExportOption<T>();
        _workbookHandlers = CreateHandler(_option);
        DefaultCellStyle = CreateDefaultCellStyle(workbook);
    }

    public ExcelWriter(Dictionary<string, Tuple<IEnumerable<T>, ExcelExportSheetOption<T>>> data, ExcelExportOption<T>? option = null)
    {
        _data = data;
        workbook = ExcelFactory.CreateWorkBook();
        _option = option ?? new ExcelExportOption<T>();
        _workbookHandlers = CreateHandler(_option);
        DefaultCellStyle = CreateDefaultCellStyle(workbook);
    }

    public ExcelWriter(Dictionary<string, IEnumerable<T>> data, ExcelExportOption<T>? option = null)
    {
        _data = data.Select(x => new KeyValuePair<string, Tuple<IEnumerable<T>, ExcelExportSheetOption<T>>>(x.Key, new(x.Value, new())));
        workbook = ExcelFactory.CreateWorkBook();
        _option = option ?? new ExcelExportOption<T>();
        _workbookHandlers = CreateHandler(_option);
        DefaultCellStyle = CreateDefaultCellStyle(workbook);
    }

    private static ICellStyle CreateDefaultCellStyle(IWorkbook book)
    {
        return book.CreateDefaultCellStyle();
    }

    private static IEnumerable<IExportCellHandler<T>> CreateHandler(ExcelExportOption<T> option)
    {
        IEnumerable<IExportCellHandler<T>> result = option.Selector.Invoke().Select(x => new PropertyCellHandler<T>(x));
        var dynamics = option.DynamicExports ?? Enumerable.Empty<IExportDynamicWrite<T>>();
        if (option.DynamicExport != null)
        {
            dynamics = dynamics.Concat(new[] { option.DynamicExport });
        }
        if (dynamics.Any())
        {
            result = result.Concat(dynamics.SelectMany(x => DynamicExportCellHandler<T>.Create(x)));
        }
        return result;
    }

    private void ResetHandler(ExcelExportSheetOption<T> option)
    {
        var dynamics = option.DynamicExports ?? Enumerable.Empty<IExportDynamicWrite<T>>();
        if (option.DynamicExport != null)
        {
            dynamics = dynamics.Concat(new[] { option.DynamicExport });
        }
        if (dynamics.Any())
        {
            _handlers = _workbookHandlers.Concat(dynamics.SelectMany(x => DynamicExportCellHandler<T>.Create(x)));
        }
        else
        {
            _handlers = _workbookHandlers;
        }
        _handlers = _handlers.OrderBy(x => x.Order).ToList();
    }

    public IWorkbook Write()
    {
        foreach (var item in _data)
        {
            WriteSheet(item.Key, item.Value.Item1, item.Value.Item2);
            WriteMergedRegion(_option.MergedRegions);
            WriteMergedRegion(item.Value.Item2.MergedRegions);
        }
        return workbook;
    }

    private void WriteMergedRegion(IEnumerable<MergedRegion>? regions)
    {
        if (regions == null)
        {
            return;
        }
        foreach (var item in regions)
        {
            if (item.Value != null)
            {
                var cell = _currentSheet!.GetOrCreateRow(item.RowStartIndex).GetOrCreateCell(item.ColumnStartIndex, DefaultCellStyle);
                var converter = _factory.GetDefaultConverter(item.Value.GetType());
                converter.WriteCell(cell, item.Value, item.FormatCellStyle?.Invoke(cell));
            }
            if (item.ColumnEndIndex != item.ColumnStartIndex || item.RowEndIndex != item.RowStartIndex)
            {
                var region = new CellRangeAddress(item.RowStartIndex, item.RowEndIndex, item.ColumnStartIndex, item.ColumnEndIndex);
                RegionUtil.SetBorderBottom(((int)BorderStyle.Thin), region, _currentSheet);
                RegionUtil.SetBorderRight(((int)BorderStyle.Thin), region, _currentSheet);
                RegionUtil.SetBorderLeft(((int)BorderStyle.Thin), region, _currentSheet);
                RegionUtil.SetBorderTop(((int)BorderStyle.Thin), region, _currentSheet);
                _currentSheet!.AddMergedRegion(region);
            }
        }
    }

    private void WriteSheet(string name, IEnumerable<T> data, ExcelExportSheetOption<T> option)
    {
        ResetHandler(option);
        CreateSheet(name);
        WriteHeader();
        _rowIndex = _option.StartLineIndex;
        foreach (var item in data.Where(x => x != null))
        {
            WriteOneLine(item);
        }
    }

    private void CreateSheet(string name)
    {
        _rowIndex = 0;
        _currentSheet = workbook.CreateSheet(name);
    }

    private void NextRow(int? row = null)
    {
        _columnIndex = 0;
        _rowIndex = row ?? _rowIndex;
        _currentRow = _currentSheet!.CreateRow(_rowIndex++);
        _currentRow.HeightInPoints = 20;
    }

    private ICell NextCell()
    {
        var cell = _currentRow!.CreateCell(_columnIndex++);
        cell.CellStyle = DefaultCellStyle;
        return cell;
    }

    private void WriteHeader()
    {
        NextRow(_option.HeaderLineIndex);
        foreach (var item in Info.Select(x => x.Info))
        {
            var cell = NextCell();
            _factory.GetDefaultConverter(nameof(String))!.WriteToCell(cell, item.Name);
            _currentSheet!.SetColumnWidth(_columnIndex - 1, (item.Width.HasValue && item.Width > 0 ? item.Width.Value : _option.DefaultColumnWidth) * WidthFactor);
        }
    }

    private void WriteOneLine(T data)
    {
        NextRow();
        foreach (var item in Info)
        {
            var cell = NextCell();
            item.WriteToCell(cell, data, _factory);
        }
    }
}