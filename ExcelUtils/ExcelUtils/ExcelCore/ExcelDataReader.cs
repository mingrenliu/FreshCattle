using ExcelUtile.Converters;

namespace ExcelUtile.ExcelCore;

/// <summary>
/// 数组对象读取器，从 Sheet 按表头匹配属性并反序列化为 <c>T</c>。
/// 建立在 <see cref="ExcelSheetReader"/> 之上。
/// 仅依赖 <see cref="ExcelReaderTypeInfo"/>：通过 <see cref="IExcelColumnReader.Match"/> 匹配表头，<see cref="IExcelColumnReader.SetValue"/> 写回数据。
/// </summary>
internal class ExcelDataReader<T> where T : class, new()
{
    private readonly ExcelSheetReader _reader;
    private readonly ExcelSerializerOptions _options;
    private readonly ExcelReaderTypeInfo _typeInfo;
    private readonly ExcelConverter _defaultConverter;

    /// <summary>匹配到的列：(列索引, 列名, 列信息)。</summary>
    private List<(int ColIdx, string ColumnName, IExcelColumnReader Prop)>? _columns;

    public ExcelDataReader(ExcelSheetReader reader, ExcelReaderTypeInfo typeInfo,
        ExcelSerializerOptions options)
    {
        _reader = reader;
        _typeInfo = typeInfo;
        _options = options;
        _defaultConverter = BuiltinConverters.Fallback;
    }

    public IEnumerable<T> Read()
    {
        if (!InitHeader()) yield break;

        for (int rowIdx = _options.EffectiveDataStartRow; rowIdx <= _reader.LastRowNum; rowIdx++)
        {
            var obj = ReadRow(rowIdx);
            if (obj != null) yield return obj;
        }
    }

    private bool InitHeader()
    {
        if (_reader.LastRowNum < _options.HeaderRow) return false;
        if (_options.EffectiveDataStartRow > _reader.LastRowNum) return false;

        var headers = _reader.ReadHeaders(_options.HeaderRow);
        if (headers.Count == 0) return false;

        _columns = new List<(int, string, IExcelColumnReader)>();
        var matchedProps = new HashSet<IExcelColumnReader>();

        foreach (var (colIdx, text) in headers)
        {
            foreach (var prop in _typeInfo.Columns)
            {
                if (prop.Match(text))
                {
                    _columns.Add((colIdx, text, prop));
                    matchedProps.Add(prop);
                    break;
                }
            }
        }

        if (_columns.Count == 0) return false;

        if (_options.ValidateRequired)
        {
            foreach (var prop in _typeInfo.Columns.Where(p => p.Required))
            {
                if (!matchedProps.Contains(prop))
                    throw new InvalidOperationException($"导入失败，缺少必填字段列: {prop}");
            }
        }

        return true;
    }

    private T? ReadRow(int rowIndex)
    {
        var obj = new T();

        foreach (var (colIdx, columnName, prop) in _columns!)
        {
            var cell = _reader.GetMergedCell(rowIndex, colIdx);
            if (cell == null) continue;

            var converter = prop.Converter ?? _defaultConverter;
            var value = converter.ReadObject(cell);
            prop.SetValue(obj, columnName, value);
        }

        return obj;
    }
}
