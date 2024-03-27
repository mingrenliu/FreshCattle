﻿namespace ExcelUtile.ExcelCore
{
    internal class ExcelReader<T> where T : class, new()
    {
        private readonly IWorkbook _workbook;
        private readonly ExcelImportOption<T> _option;
        private readonly KeyValueWrapper<IImportCellHandler<T>> _info;
        private int _currentSheetIndex = -1;
        private ISheet? _currentSheet;
        private IRow? _currentRow;
        private SortedWrapper<HeaderInfo>? _headers;
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
            _info = CreateInfo(_option);
        }

        static KeyValueWrapper<IImportCellHandler<T>> CreateInfo(ExcelImportOption<T> option)
        {
            IEnumerable<IImportCellHandler<T>> handlers = option.Selector.Invoke().Select(x => new PropertyCellHandler<T>(x));
            if (option.DynamicImport != null)
            {
                handlers = handlers.Concat(DynamicImportCellHandler<T>.Create(option.DynamicImport));
            }
            return new KeyValueWrapper<IImportCellHandler<T>>(handlers, x => x.Info.Name);
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
            var header = new SortedWrapper<HeaderInfo>();
            foreach (var cell in MergedHeaders().Concat(row.Cells))
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
            foreach (var info in _info.Values.Select(x => x.Info).Where(x => x.IsRequired))
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
            foreach (var item in _headers)
            {
                //先查找合单元格
                var mergedArea = mergedAreas.FirstOrDefault(x => x.MinColumn <= item.Order && x.MaxColumn >= item.Order);
                var cell = mergedArea != null ? _currentSheet.GetRow(mergedArea.FirstRow)?.GetCell(mergedArea.FirstColumn) : null;
                cell ??= _currentRow!.GetCell(item.Order);
                var handler = _info[item.Name];
                if (cell != null)
                {
                    handler?.ReadFromCell(cell, obj, _factory);
                }
            }
            return obj;
        }
    }
}