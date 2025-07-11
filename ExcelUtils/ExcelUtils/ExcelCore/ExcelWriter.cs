using NPOI.SS.Util;

namespace ExcelUtile.ExcelCore;

internal class ExcelWriter<T> where T : class
{
    private readonly ICellStyle DefaultCellStyle;
    private const int WidthFactor = 256;
    private readonly IEnumerable<T> _data;
    private readonly ISheet _sheet;
    private IRow? _currentRow;
    private int _rowIndex = 0;
    private int _columnIndex = 0;
    private readonly ExcelExportOption<T> _option;
    private readonly IEnumerable<IExportCellHandler<T>> _handlers;
    private readonly IConverterFactory _factory = new DefaultConverterFactory();

    public ExcelWriter(ISheet sheet, IEnumerable<T> data, ExcelExportOption<T>? option = null)
    {
        _sheet = sheet;
        _data = data;
        _option = option ?? new ExcelExportOption<T>();
        _handlers = CreateHandler(_option);
        DefaultCellStyle = CreateDefaultCellStyle(sheet.Workbook);
    }

    private static ICellStyle CreateDefaultCellStyle(IWorkbook book)
    {
        return book.CreateDefaultCellStyle();
    }

    private static IEnumerable<IExportCellHandler<T>> CreateHandler(ExcelExportOption<T> option)
    {
        IEnumerable<IExportCellHandler<T>> result = option.Selector.Invoke().Select(x => new DefaultCellHandler<T>(x));
        var dynamics = option.DynamicExports ?? Enumerable.Empty<ICellWriter<T>>();
        if (dynamics.Any())
        {
            result = result.Concat(dynamics.SelectMany(x => DynamicExportCellHandler<T>.Create(x)));
        }
        return result.OrderBy(x=>x.Order).ToList();
    }

    public ISheet Write()
    {
        WriteSheet(_data);
        WriteMergedRegion(_option.MergedRegions);
        return _sheet;
    }

    private void WriteMergedRegion(IEnumerable<MergedRegion>? regions)
    {
        if (regions == null)
        {
            return;
        }
        foreach (var region in regions)
        {
            if (region.FirstRow!=region.LastRow || region.FirstColumn != region.LastColumn)
            {
                RegionUtil.SetBorderBottom(BorderStyle.Thin, region, _sheet);
                RegionUtil.SetBorderRight(BorderStyle.Thin, region, _sheet);
                RegionUtil.SetBorderLeft(BorderStyle.Thin, region, _sheet);
                RegionUtil.SetBorderTop(BorderStyle.Thin, region, _sheet);
                _sheet.AddMergedRegion(region);
            }
            if (region.Value != null)
            {
                var cell = _sheet.GetOrCreateRow(region.FirstRow).GetOrCreateCell(region.FirstColumn, DefaultCellStyle);
                var converter = _factory.GetDefaultConverter(region.Value.GetType());
                converter.WriteCell(cell, region.Value, region.FormatCellStyle?.Invoke(cell));
            }
        }
    }

    private void WriteSheet(IEnumerable<T> data)
    {
        _rowIndex = _option.StartLineIndex;
        foreach (var item in data.Where(x => x != null))
        {
            WriteOneLine(item);
        }
        WriteHeader();
    }

    private void NextRow(int? row = null)
    {
        _columnIndex = 0;
        _rowIndex = row ?? _rowIndex;
        _currentRow = _sheet.CreateRow(_rowIndex++);
        _currentRow.HeightInPoints = _option.DefaultColumnWidth;
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
        foreach (var item in _handlers.Select(x => x.Info))
        {
            var cell = NextCell();
            _factory.GetDefaultConverter(nameof(String))!.WriteToCell(cell, item.Name);
            if (item.DynamicWidth)
            {
                _sheet.AutoSizeColumn(_columnIndex - 1);
            }
            else
            {
                _sheet.SetColumnWidth(_columnIndex - 1, ConvertWidth(item.Width));
            }
        }
    }

    double ConvertWidth(int? value)
    {
        return (value.HasValue && value > 0 ? value.Value : _option.DefaultColumnWidth) * WidthFactor;
    }

    private void WriteOneLine(T data)
    {
        NextRow();
        foreach (var item in _handlers)
        {
            var cell = NextCell();
            item.WriteToCell(cell, data, _factory);
        }
    }
}