using ExcelUtile.Formats;
using NPOI.SS.Util;

namespace ExcelUtile.ExcelCore
{
    internal class ExcelWriter<T> where T : class, new()
    {
        private const int WidthFactor = 256;
        private const int HeightFactor = 20;
        private readonly IEnumerable<KeyValuePair<string, Tuple<IEnumerable<T>, IEnumerable<MergedRegion>?>>> _data;
        private readonly IWorkbook workbook;
        private ISheet? _currentSheet;
        private IRow? _currentRow;
        private int _rowIndex = 0;
        private int _columnIndex = 0;
        private readonly ExcelExportOption<T> _option;
        private readonly IEnumerable<IExportCellHandler<T>> _info;
        private readonly IConverterFactory _factory = new DefaultConverterFactory();

        public ExcelWriter(IEnumerable<T> data, ExcelExportOption<T>? option = null, IEnumerable<MergedRegion>? region = null)
        {
            _data = new List<KeyValuePair<string, Tuple<IEnumerable<T>, IEnumerable<MergedRegion>?>>>() { new("sheet", new(data, region)) };
            workbook = ExcelFactory.CreateWorkBook();
            _option = option ?? new ExcelExportOption<T>();
            _info = CreateInfo(_option).ToList();
        }

        static IEnumerable<IExportCellHandler<T>> CreateInfo(ExcelExportOption<T> option)
        {
            IEnumerable<IExportCellHandler<T>> handlers = option.Selector.Invoke().Select(x => new PropertyCellHandler<T>(x));
            if (option.DynamicExport != null)
            {
                handlers = handlers.Concat(DynamicExportCellHandler<T>.Create(option.DynamicExport));
            }
            return handlers;
        }

        public ExcelWriter(Dictionary<string, Tuple<IEnumerable<T>, IEnumerable<MergedRegion>?>> data, ExcelExportOption<T>? option = null)
        {
            _data = data;
            workbook = ExcelFactory.CreateWorkBook();
            _option = option ?? new ExcelExportOption<T>();
            _info = _option.Selector.Invoke().Select(x => new PropertyCellHandler<T>(x)).OrderBy(x => x.Info.Order).ToList();
        }

        public ExcelWriter(Dictionary<string, IEnumerable<T>> data, ExcelExportOption<T>? option = null)
        {
            _data = data.Select(x => new KeyValuePair<string, Tuple<IEnumerable<T>, IEnumerable<MergedRegion>?>>(x.Key, new(x.Value, null)));
            workbook = ExcelFactory.CreateWorkBook();
            _option = option ?? new ExcelExportOption<T>();
            _info = _option.Selector.Invoke().Select(x => new PropertyCellHandler<T>(x)).OrderBy(x => x.Info.Order).ToList();
        }

        public IWorkbook Write()
        {
            foreach (var item in _data)
            {
                WriteSheet(item.Key, item.Value.Item1);
                WriteMergedRegion(item.Value.Item2);
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
                    var cell = _currentSheet!.GetOrCreateRow(item.RowStartIndex).GetOrCreateCell(item.ColumnStartIndex);
                    var converter = _factory.GetDefaultConverter(item.Value.GetType());
                    converter.WriteCell(cell, item.Value);
                }
                if (item.ColumnEndIndex != item.ColumnStartIndex || item.RowEndIndex != item.RowStartIndex)
                {
                    _currentSheet!.AddMergedRegion(new CellRangeAddress(item.RowStartIndex, item.RowEndIndex, item.ColumnStartIndex, item.ColumnEndIndex));
                }
            }
        }

        private void WriteSheet(string name, IEnumerable<T> data)
        {
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
            return _currentRow!.CreateCell(_columnIndex++);
        }

        private void WriteHeader()
        {
            NextRow(_option.HeaderLineIndex);
            foreach (var item in _info.Select(x => x.Info))
            {
                var cell = NextCell();
                _factory.GetDefaultConverter(nameof(String))!.WriteToCell(cell, item.Name);
                _currentSheet!.SetColumnWidth(_columnIndex - 1, (item.Width.HasValue && item.Width > 0 ? item.Width.Value : _option.DefaultColumnWidth) * WidthFactor);
            }
        }

        private void WriteOneLine(T data)
        {
            NextRow();
            foreach (var item in _info)
            {
                var cell = NextCell();
                item.WriteToCell(cell, data, _factory);
            }
        }
    }
}