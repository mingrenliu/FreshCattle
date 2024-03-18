using ExcelUtile.Formats;
using NPOI.SS.Util;

namespace ExcelUtile.ExcelCore
{
    internal class ExcelWriter<T> where T : class
    {
        private const int WidthFactor = 256;
        private const int HeightFactor = 20;
        private readonly IEnumerable<KeyValuePair<string, Tuple<IEnumerable<T>, IEnumerable<MergedRegion>?>>> _data;
        private readonly IWorkbook workbook;
        private ISheet? _currentSheet;
        private IRow? _currentRow;
        private int _rowIndex = 0;
        private int _columnIndex = 0;
        private ExcelSerializeOptions _option;
        private readonly IEnumerable<PropertyTypeInfo> _info;
        private readonly DefaultConverterFactory _factory = new();

        public ExcelWriter(IEnumerable<T> data, ExcelSerializeOptions? option = null, IEnumerable<MergedRegion>? region = null)
        {
            _data = new List<KeyValuePair<string, Tuple<IEnumerable<T>, IEnumerable<MergedRegion>?>>>() { new("sheet", new(data, region)) };
            workbook = ExcelFactory.CreateWorkBook();
            _option = option ?? new ExcelSerializeOptions();
            _info = _option.PropertySelector.Invoke(typeof(T)).OrderBy(x => x.Order).ToList();
        }

        public ExcelWriter(Dictionary<string, Tuple<IEnumerable<T>, IEnumerable<MergedRegion>?>> data, ExcelSerializeOptions? option = null)
        {
            _data = data;
            workbook = ExcelFactory.CreateWorkBook();
            _option = option ?? new ExcelSerializeOptions();
            _info = _option.PropertySelector.Invoke(typeof(T)).OrderBy(x => x.Order).ToList();
        }

        public ExcelWriter(Dictionary<string, IEnumerable<T>> data, ExcelSerializeOptions? option = null)
        {
            _data = data.Select(x => new KeyValuePair<string, Tuple<IEnumerable<T>, IEnumerable<MergedRegion>?>>(x.Key, new(x.Value, null)));
            workbook = ExcelFactory.CreateWorkBook();
            _option = option ?? new ExcelSerializeOptions();
            _info = _option.PropertySelector.Invoke(typeof(T)).OrderBy(x => x.Order).ToList();
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
                    WriteCell(converter, cell, item.Value);
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
            foreach (var item in data)
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
            foreach (var item in _info)
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
                var value = item.Info.GetValue(data);
                var converter = item.GetConverter(_factory);
                WriteCell(converter, cell, value);
            }
        }

        private static void WriteCell(ExcelConverter? converter, ICell cell, object? value)
        {
            if (converter != null)
            {
                converter.WriteToCell(cell, value);
            }
            else
            {
                var str = value?.ToString();
                if (!string.IsNullOrEmpty(str))
                {
                    cell.SetCellValue(str);
                }
            }
        }
    }
}