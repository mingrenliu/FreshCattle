namespace ExcelUtile.ExcelCore;

internal class ExcelReader<T> where T : class, new()
{
    private readonly ExcelImportOption<T> _option;
    private readonly Dictionary<int, IImportCellHandler<T>> _handers = new();
    private readonly ISheet _currentSheet;
    private IRow? _currentRow;
    private readonly Dictionary<int, string> _headers = new();
    private readonly IConverterFactory _factory = new DefaultConverterFactory();

    public ExcelReader(ISheet sheet, ExcelImportOption<T>? option = null)
    {
        _currentSheet = sheet;
        _option = option ?? new ExcelImportOption<T>();
    }

    private (DictionaryWrapper<IExactImportCellHandler<T>>, List<IImportCellHandler<T>>) CreateHandler(ExcelImportOption<T> option)
    {
        var exactHandler = new DictionaryWrapper<IExactImportCellHandler<T>>(_option.Selector.Invoke().Select(x => new DefaultCellHandler<T>(x)), x => x.Info.Name);
        var handlers = new List<IImportCellHandler<T>>();
        if (option.DynamicImports != null)
        {
            foreach (var item in option.DynamicImports)
            {
                Add(item);
            }
        }
        void Add(IDynamicCellReader<T> import)
        {
            if (import is IExactCellReader<T> read)
            {
                var exact = ExactImportCellHandler<T>.Create(read);
                exactHandler.AddRange(exact, x => x.Info.Name);
            }
            else
            {
                handlers.Add(DynamicImportCellHandler<T>.Create(import));
            }
        }
        return (exactHandler, handlers);
    }

    /// <summary>
    /// 每次切换Sheet重新解析标题行
    /// </summary>
    /// <param name="sheetNum"> </param>
    /// <returns> </returns>
    private bool Init()
    {
        if (_currentSheet.LastRowNum < _option.HeaderLineIndex) return false;
        if (_option.StartLineIndex > _currentSheet.LastRowNum) return false;
        var row = _currentSheet.GetRow(_option.HeaderLineIndex);
        if (row == null) return false;
        var (exactHandler, commonHandler) = CreateHandler(_option);
        foreach (var cell in MergedHeaders().Concat(row.Cells))
        {
            var str = cell.StringCellValue;
            if (string.IsNullOrWhiteSpace(str) || !exactHandler.ContainKey(str)) continue;
            if (exactHandler.ContainKey(str))
            {
                _handers[cell.ColumnIndex] = exactHandler[str]!;
                _headers[cell.ColumnIndex] = str;
            }
            else if (_option.IgnoreField?.Invoke(str) != true && commonHandler != null)
            {
                foreach (var item in commonHandler)
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
            Validate(exactHandler);
        }
        return true;
    }

    public IEnumerable<ICell> MergedHeaders()
    {
        foreach (var item in _currentSheet.MergedRegions.Where(x => x.ContainsRow(_option.HeaderLineIndex)))
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

    private void Validate(DictionaryWrapper<IExactImportCellHandler<T>> exactHandler)
    {
        foreach (var info in exactHandler.Values.Select(x => x.Info).Where(x => x.IsRequired))
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
        if (_currentSheet.LastRowNum == _currentRow!.RowNum) return false;
        _currentRow = _currentSheet.GetRow(_currentRow.RowNum + 1);
        return _currentRow != null;
    }

    public IEnumerable<T> Read()
    {
        var result = new List<T>();
        if (Init())
        {
            do
            {
                var obj = ReadLine();
                if (obj != null) result.Add(obj);
            } while (SwitchRow());
        }
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