﻿namespace ExcelUtile.ExcelCore
{
    internal class ExcelWriter<T> where T : class
    {
        private readonly Dictionary<string, IEnumerable<T>> _data;
        private readonly IWorkbook workbook;
        private ISheet? _currentSheet;
        private IRow? _currentRow;
        private int _rowIndex = 0;
        private int _columnIndex = 0;
        private ExcelSerializeOptions _option;
        private readonly IEnumerable<PropertyTypeInfo> _info;

        public ExcelWriter(IEnumerable<T> data, ExcelSerializeOptions? option = null)
        {
            _data = new Dictionary<string, IEnumerable<T>>() { ["sheet"] = data };
            workbook = ExcelFactory.CreateWorkBook();
            _option = option ?? new ExcelSerializeOptions();
            _info = _option.PropertySelector.Invoke(typeof(T)).OrderBy(x => x.Order).ToList();
        }

        public ExcelWriter(Dictionary<string, IEnumerable<T>> data, ExcelSerializeOptions? option = null)
        {
            _data = data;
            workbook = ExcelFactory.CreateWorkBook();
            _option = option ?? new ExcelSerializeOptions();
            _info = _option.PropertySelector.Invoke(typeof(T)).OrderBy(x => x.Order).ToList();
        }

        public IWorkbook Write()
        {
            foreach (var item in _data)
            {
                WriteSheet(item.Key, item.Value);
            }
            return workbook;
        }

        private void WriteSheet(string name, IEnumerable<T> data)
        {
            CreateSheet(name);
            WriteHeader();
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

        private void CreateRow()
        {
            _columnIndex = 0;
            _currentRow = _currentSheet!.CreateRow(_rowIndex++);
        }

        private ICell CreateCell()
        {
            return _currentRow!.CreateCell(_columnIndex++);
        }

        private void WriteHeader()
        {
            CreateRow();
            foreach (var item in _info)
            {
                var cell = CreateCell();
                cell.SetCellValue(item.Name);
            }
        }

        private void WriteOneLine(T data)
        {
            CreateRow();
            foreach (var item in _info)
            {
                var cell = CreateCell();
                var value = item.Info.GetValue(data);
                var converter = item.GetConverter();
                if (converter != null)
                {
                    converter?.WriteAsObject(cell, value);
                }
                else
                {
                    cell.SetCellValue(value?.ToString());
                }
            }
        }
    }
}