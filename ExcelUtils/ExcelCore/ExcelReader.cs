using NPOI.SS.Formula.Functions;

namespace ExcelUtils.ExcelCore
{
    internal class ExcelReader<T> where T : class
    {
        private readonly IWorkbook _workbook;
        private ExcelSerializerOptions _option;
        private InfoWrapper<PropertyTypeInfo> _infos;
        private readonly Type _type;
        private int _currentSheetIndex=-1;
        private ISheet? _currentSheet;
        private IRow? _currentRow;
        private Dictionary<int, string>? _headers;
        private int NumberOfSheets => _workbook.NumberOfSheets;

        public ExcelReader(IWorkbook? workbook, ExcelSerializerOptions? option = null)
        {
            if (workbook == null) throw new ArgumentNullException("导入的不存在");
            _workbook = workbook!;
            _type = typeof(T);
            _option = option ?? new ExcelSerializerOptions();
            _infos = _option.PropertySelector.Invoke(_type);
        }

        /// <summary>
        /// 每次切换sheet重新解析标题行
        /// </summary>
        /// <param name="sheetNum"></param>
        /// <returns></returns>
        private bool SwitchSheet()
        {
            _currentSheetIndex++;
            if (NumberOfSheets <= _currentSheetIndex) return false;
            _currentSheet = _workbook.GetSheetAt(_currentSheetIndex);
            if (_currentSheet.LastRowNum < _option.HeaderLineIndex) return false;
            if (_option.StartLineIndex > _currentSheet.LastRowNum) return false;
            var row = _currentSheet.GetRow(_option.HeaderLineIndex);
            var header = new Dictionary<int, string>();
            foreach (var cell in row.Cells)
            {
                var str = cell.StringCellValue;
                if (string.IsNullOrWhiteSpace(str)) continue;
                header[cell.ColumnIndex] = str.Trim();
            }
            if (!header.Any()) return false;
            _headers = header;
            _currentRow = _currentSheet.GetRow(_option.StartLineIndex);
            return true;
        }
        /// <summary>
        /// 后续合并行时,多行推进
        /// </summary>
        /// <returns></returns>
        private bool SwitchRow()
        {
            if (_currentSheet!.LastRowNum == _currentRow!.RowNum) return false;
            _currentRow=_currentSheet.GetRow(_currentRow.RowNum+1);
            return default;
        }
        public Dictionary<string, IEnumerable<T>> ReadMultiSheet()
        {
            var result= new Dictionary<string, IEnumerable<T>>();
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
        public IEnumerable<T> ReadSheet()
        {
            var result=new List<T>();
            do
            {
                var obj = ReadLine();
                if (obj!=null) result.Add(obj);
            } while (SwitchRow());
            return result;
        }

        public T? ReadLine()
        {
           var obj=Activator.CreateInstance(typeof(T));
           var enumerator= _infos.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var current= enumerator.Current;
                //current.Info.SetValue(obj,)

            }
            return default(T?);
        }

        public enum SupportTypeName
        {
            Boolean,
            Double,
            Decimal,
            Int32,
            Int64,
            Single,
            String,
            DateTime,
            TimeSpan,
            DateOnly,
            DateTimeOffset,
            TimeOnly
        }
        
    }
}