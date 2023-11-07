using Microsoft.CodeAnalysis.CSharp.Syntax;
using NPOI.XSSF.UserModel;

namespace ExcelUtile.ExcelCore
{
    internal class ExcelReader<T> where T : class
    {
        private readonly XSSFWorkbook _workbook;
        private ExcelSerializeOptions _option;
        private readonly KeyValueWrapper<PropertyTypeInfo> _info;
        private readonly Type _type;
        private int _currentSheetIndex = -1;
        private ISheet? _currentSheet;
        private IRow? _currentRow;
        private SortedWrapper<HeaderInfo>? _headers;
        private DefaultConverterFactory _factory = new();
        private int NumberOfSheets => _workbook.NumberOfSheets;

        public ExcelReader(XSSFWorkbook? workbook, ExcelSerializeOptions? option = null)
        {
            if (workbook == null)
            {
                throw new Exception("导入的不存在");
            }
            _workbook = workbook!;
            _type = typeof(T);
            _option = option ?? new ExcelSerializeOptions();
            _info = new KeyValueWrapper<PropertyTypeInfo>(_option.PropertySelector.Invoke(_type), x => x.Name);
        }

        /// <summary>
        /// 每次切换Sheet重新解析标题行
        /// </summary>
        /// <param name="sheetNum"></param>
        /// <returns></returns>
        private bool SwitchSheet()
        {
            _currentSheetIndex++;
            if (NumberOfSheets <= _currentSheetIndex) return false;
            _currentSheet = _workbook.GetSheetAt(_currentSheetIndex);
            if (_currentSheet == null) return false;
            if (_currentSheet.LastRowNum < _option.HeaderLineIndex) return false;
            if (_option.StartLineIndex > _currentSheet.LastRowNum) return false;
            var row = _currentSheet.GetRow(_option.HeaderLineIndex);
            if (row == null) return false;
            var header = new SortedWrapper<HeaderInfo>();
            foreach (var cell in row.Cells)
            {
                var str = cell.StringCellValue;
                if (string.IsNullOrWhiteSpace(str) || !_info.ContainKey(str)) continue;
                header.Add(cell.ColumnIndex, new HeaderInfo(str.Trim(), cell.ColumnIndex));
            }
            if (!header.Any()) return false;
            _headers = header;
            _currentRow = _currentSheet.GetRow(_option.StartLineIndex);
            if (_option.ValidImportField)
            {
                Validate();
            }
            return true;
        }

        private void Validate()
        {
            foreach (var info in _info.Values.Where(x => x.IsRequired))
            {
                if (_headers!.Any(x => x.Name == info.Name))
                {
                    continue;
                }
                throw new Exception($"导入失败,必须导入字段缺失:{info.Name}");
            }
        }

        /// <summary>
        /// 后续合并行时,多行推进
        /// </summary>
        /// <returns></returns>
        private bool SwitchRow()
        {
            if (_currentSheet!.LastRowNum == _currentRow!.RowNum) return false;
            _currentRow = _currentSheet.GetRow(_currentRow.RowNum + 1);
            return _currentRow != null;
        }

        public Dictionary<string, IEnumerable<T>> ReadMultiSheet()
        {
            var result = new Dictionary<string, IEnumerable<T>>();
            while (SwitchSheet())
            {
                result[_currentSheet!.SheetName] = ReadSheet();
            }
            return result;
        }

        public IEnumerable<T> ReadOneSheet()
        {
            if (SwitchSheet())
            {
                return ReadSheet();
            }
            return Enumerable.Empty<T>();
        }

        private IEnumerable<T> ReadSheet()
        {
            var result = new List<T>();
            do
            {
                var obj = ReadLine();
                if (obj != null) result.Add(obj);
            } while (SwitchRow());
            return result;
        }

        private T? ReadLine()
        {
            if (_headers == null) return null;
            var obj = Activator.CreateInstance(typeof(T));
            foreach (var item in _headers)
            {
                var cell = _currentRow!.GetCell(item.Order);
                var prop = _info[item.Name];
                if (prop != null && cell != null)
                {
                    var converter = prop.GetConverter(_factory);
                    if (converter != null)
                    {
                        var value = converter.ReadFromCell(cell);
                        if (value != null)
                        {
                            prop.Info.SetValue(obj, value);
                        }
                    }
                    else
                    {
                        try
                        {
                            var str = cell.ToString();
                            if (str != null)
                            {
                                prop.Info.SetValue(obj, Convert.ChangeType(str, prop.BaseType));
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            return (T?)obj;
        }
    }
}