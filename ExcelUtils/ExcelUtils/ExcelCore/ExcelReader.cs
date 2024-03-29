namespace ExcelUtile.ExcelCore;

internal class ExcelReader<T> where T : class, new()
{
    private readonly IWorkbook _workbook;
    private readonly ExcelImportOption<T> _option;
    private readonly KeyValueWrapper<IExactImportCellHandler<T>> _exactHandler;
    private readonly IEnumerable<IImportCellHandler<T>>? _commonHandler;
    private readonly Dictionary<int, IImportCellHandler<T>> _handers = new();
    private int _currentSheetIndex = -1;
    private ISheet? _currentSheet;
    private IRow? _currentRow;
    private readonly Dictionary<int, string> _headers = new();
    private readonly IConverterFactory _factory = new DefaultConverterFactory();
    private int NumberOfSheets => _workbook.NumberOfSheets;

    public ExcelReader(IWorkbook? workbook, ExcelImportOption<T>? option = null)
    {
        if (workbook == null)
        {
            throw new Exception("导入的不存在");
        }
        _workbook = workbook!;
        _option = option ?? new ExcelImportOption<T>();
        _exactHandler = new KeyValueWrapper<IExactImportCellHandler<T>>(_option.Selector.Invoke().Select(x => new PropertyCellHandler<T>(x)), x => x.Info.Name);
        _commonHandler = AddOption(_option);
    }

    private List<IImportCellHandler<T>> AddOption(ExcelImportOption<T> option)
    {
        var handlers = new List<IImportCellHandler<T>>();
        if (option.DynamicImport != null)
        {
            Add(option.DynamicImport);
        }
        if (option.DynamicImports != null)
        {
            foreach (var item in option.DynamicImports)
            {
                Add(item);
            }
        }
        void Add(IImportDynamicRead<T> import)
        {
            if (import is IImportExactRead<T> reacd)
            {
                var exact = ExactImportCellHandler<T>.Create(reacd);
                _exactHandler.AddRange(exact, x => x.Info.Name);
            }
            else
            {
                handlers.Add(DynamicImportCellHandler<T>.Create(import));
            }
        }
        return handlers;
    }

    /// <summary>
    /// 每次切换Sheet重新解析标题行
    /// </summary>
    /// <param name="sheetNum"> </param>
    /// <returns> </returns>
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
        _handers.Clear();
        _headers.Clear();
        foreach (var cell in MergedHeaders().Concat(row.Cells))
        {
            var str = cell.StringCellValue;
            if (string.IsNullOrWhiteSpace(str) || !_exactHandler.ContainKey(str)) continue;
            if (_exactHandler.ContainKey(str))
            {
                _handers[cell.ColumnIndex] = _exactHandler[str]!;
                _headers[cell.ColumnIndex] = str;
            }
            else if (_option.IgnoreField?.Invoke(str) != true && _commonHandler != null)
            {
                foreach (var item in _commonHandler)
                {
                    if (item.Match(str))
                    {
                        _handers[cell.ColumnIndex] = item;
                        _headers[cell.ColumnIndex] = str;
                        break;
                    }
                }
            }
        }
        if (!_headers.Any()) return false;
        _currentRow = _currentSheet.GetRow(_option.StartLineIndex);
        if (_option.ValidImportField)
        {
            Validate();
        }
        return true;
    }

    public IEnumerable<ICell> MergedHeaders()
    {
        foreach (var item in _currentSheet!.MergedRegions.Where(x => x.ContainsRow(_option.HeaderLineIndex)))
        {
            var mergedRow = _currentSheet.GetRow(item.FirstRow);
            if (mergedRow != null)
            {
                var mergedCell = mergedRow.GetCell(item.FirstColumn);
                if (mergedCell != null)
                {
                    yield return mergedCell;
                }
            }
        }
    }

    private void Validate()
    {
        foreach (var info in _exactHandler.Values.Select(x => x.Info).Where(x => x.IsRequired))
        {
            if (_headers.Values.Any(x => x == info.Name))
            {
                continue;
            }
            throw new Exception($"导入失败,必须导入字段缺失:{info.Name}");
        }
    }

    /// <summary>
    /// 后续合并行时,多行推进
    /// </summary>
    /// <returns> </returns>
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
        if (Activator.CreateInstance(typeof(T)) is not T obj) throw new Exception("无法通过Activator创建实例对象");
        var mergedAreas = _currentSheet!.MergedRegions.Where(x => x.ContainsRow(_currentRow!.RowNum)).ToList();
        foreach (var item in _handers.Keys)
        {
            //先查找合单元格
            var mergedArea = mergedAreas.FirstOrDefault(x => x.MinColumn <= item && x.MaxColumn >= item);
            var cell = mergedArea != null ? _currentSheet.GetRow(mergedArea.FirstRow)?.GetCell(mergedArea.FirstColumn) : null;
            cell ??= _currentRow!.GetCell(item);
            var handler = _handers[item];
            if (cell != null)
            {
                handler?.ReadFromCell(cell, obj, _headers[item], _factory);
            }
        }
        return obj;
    }
}